using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Unitverse.Core.Models;
using Unitverse.Core.Options;
using Unitverse.Core.Options.Editing;
using Unitverse.Helper;

namespace Unitverse.Views
{
    /// <summary>
    /// Interaction logic for NewProjectDialog.xaml
    /// </summary>
    public partial class NewProjectDialog : System.Windows.Window
    {
        int _scale;

        public NewProjectDialog(Project sourceProject, IUnitTestGeneratorOptions projectOptions)
        {
            InitializeComponent();
            DataContext = _viewModel = new NewProjectDialogViewModel(sourceProject, projectOptions);

            PreviewMouseWheel += NewProjectDialog_PreviewMouseWheel;
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Ideal);

            _scale = ZoomTracker.Get();
            RootScale.ScaleY = RootScale.ScaleX = 1 + (_scale / 100.0);
            _sourceProject = sourceProject;
        }

        private void NewProjectDialog_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                _scale += e.Delta / 12;
                _scale = Math.Max(0, _scale);
                _scale = Math.Min(100, _scale);

                RootScale.ScaleY = RootScale.ScaleX = 1 + (_scale / 100.0);
                ZoomTracker.Save(_scale);
            }
        }

        private readonly NewProjectDialogViewModel _viewModel;
        private readonly Project _sourceProject;

        public ProjectCreationManifest Manifest => _viewModel.Manifest;

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OnFolderSelect(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = _viewModel.Location;

                var result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    _viewModel.Location = fbd.SelectedPath;
                }
            }
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (string.IsNullOrWhiteSpace(_viewModel.Name))
            {
                System.Windows.MessageBox.Show("You must enter a name for the target project.", Constants.ExtensionName, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(_viewModel.Location) || !Directory.Exists(_viewModel.Location))
            {
                System.Windows.MessageBox.Show("You must enter a location for the target project and the directory must already exist.", Constants.ExtensionName, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (VsProjectHelper.FindProjects(_sourceProject.DTE.Solution).Any(x => 
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                return string.Equals(x.Name, _viewModel.Name, StringComparison.OrdinalIgnoreCase);
            }))
            {
                System.Windows.MessageBox.Show("There is already a project in the solution named '" + _viewModel.Name + "'. Please choose a unique name for the project.", Constants.ExtensionName, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var filename = _viewModel.Manifest.ProjectFileName;
            if (File.Exists(filename))
            {
                System.Windows.MessageBox.Show("The file '" + filename + "' already exists on disk. Please remove the existing file before continuing.", Constants.ExtensionName, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
        }
    }
}
