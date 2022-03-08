namespace Unitverse.Helper
{
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using System.Windows.Input;
    using System.Windows.Interop;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;
    using Unitverse.Core.Options.Editing;
    using Unitverse.Views;

    internal class ProjectMappingFactory
    {
        public static ProjectMapping CreateMappingFor(Project sourceProject, IUnitTestGeneratorOptions baseOptions, bool interactive, bool skipManuallySelectedTargetResolution)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // resolve the options for this project
            var projectOptions = UnitTestGeneratorOptionsFactory.Create(sourceProject.FileName, baseOptions.GenerationOptions, baseOptions.NamingOptions, baseOptions.StrategyOptions, baseOptions.StatisticsCollectionEnabled);

            // derive the target project name
            var targetProjectName = projectOptions.GenerationOptions.GetTargetProjectName(sourceProject.Name);

            // resolve the target from a session selected project, if any
            var sessionSelectedProject = TargetSelectionRegister.Instance.GetTargetFor(sourceProject.UniqueName);
            var resolvedTarget = string.IsNullOrWhiteSpace(sessionSelectedProject) || skipManuallySelectedTargetResolution ? targetProjectName : sessionSelectedProject;

            // find the target project
            var targetProject = VsProjectHelper.FindProject(sourceProject.DTE.Solution, resolvedTarget);

            // now determine if we should show the UI
            if (interactive)
            {
                var shouldShowUi = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

                switch (baseOptions.GenerationOptions.UserInterfaceMode)
                {
                    case UserInterfaceModes.WhenTargetNotFound:
                        shouldShowUi |= targetProject == null;
                        break;
                    case UserInterfaceModes.Always:
                        shouldShowUi = true;
                        break;
                }

                if (shouldShowUi)
                {
                    var window = new GenerationDialog(sourceProject, projectOptions);
                    var helper = new WindowInteropHelper(window);
#if VS2022
                    helper.Owner = sourceProject.DTE.MainWindow.HWnd;
#elif VS2019
                    helper.Owner = new System.IntPtr(sourceProject.DTE.MainWindow.HWnd);
#endif

                    var result = window.ShowDialog();
                    if (result.HasValue && !result.Value)
                    {
                        return null;
                    }

                    if (result.HasValue)
                    {
                        return window.ResultingMapping;
                    }
                }
            }

            // now resolve the frameworks in use by the target project (if target project is not null and options allow)
            var generationOptions = OptionsResolver.DetectFrameworks(targetProject, projectOptions.GenerationOptions);

            // now create the final options, including resolved frameworks
            var finalOptions = new UnitTestGeneratorOptions(generationOptions, projectOptions.NamingOptions, projectOptions.StrategyOptions, projectOptions.StatisticsCollectionEnabled);

            // now return the mapping from source to target, along with the per-project options
            return new ProjectMapping(sourceProject, targetProject, targetProjectName, finalOptions);
        }
    }
}
