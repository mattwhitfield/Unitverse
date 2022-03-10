using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Unitverse.Core.Options;
using Unitverse.Core.Options.Editing;
using Unitverse.Helper;

namespace Unitverse.Views
{
    /// <summary>
    /// Interaction logic for GenerationDialog.xaml
    /// </summary>
    public partial class GenerationDialog : System.Windows.Window
    {
        int _scale;

        public GenerationDialog(Project sourceProject, IUnitTestGeneratorOptions projectOptions)
        {
            InitializeComponent();
            DataContext = _viewModel = new GenerationDialogViewModel(sourceProject, projectOptions);

            PreviewMouseWheel += GenerationDialog_PreviewMouseWheel;
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Ideal);
        }

        private void GenerationDialog_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                _scale += e.Delta / 12;
                _scale = Math.Max(0, _scale);
                _scale = Math.Min(100, _scale);

                RootScale.ScaleY = RootScale.ScaleX = 1 + (_scale / 100.0);
            }
        }

        private GenerationDialogViewModel _viewModel;

        public ProjectMapping ResultingMapping => _viewModel.ResultingMapping;

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (ResultingMapping.TargetProject == null)
            {
                MessageBox.Show("You must select a target project in which the test should be generated.", Constants.ExtensionName, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_viewModel.RememberProjectSelection)
            {
                TargetSelectionRegister.Instance.SetTargetFor(ResultingMapping.SourceProject.UniqueName, ResultingMapping.TargetProjectName);
            }

            DialogResult = true;
        }
    }
}
