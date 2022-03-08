﻿using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.Windows;
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
        public GenerationDialog(Project sourceProject, IUnitTestGeneratorOptions projectOptions)
        {
            InitializeComponent();
            DataContext = _viewModel = new GenerationDialogViewModel(sourceProject, projectOptions);
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
