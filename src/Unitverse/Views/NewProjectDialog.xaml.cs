using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
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

        private NewProjectDialogViewModel _viewModel;

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

            if (string.IsNullOrWhiteSpace(_viewModel.Location))
            {
                System.Windows.MessageBox.Show("You must enter a location for the target project.", Constants.ExtensionName, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
        }
    }
}
