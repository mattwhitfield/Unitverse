namespace Unitverse.Helper
{
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using System.Windows.Interop;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;
    using Unitverse.Options;
    using Unitverse.Views;

    internal class ProjectMappingFactory
    {
        public static ProjectMapping CreateMappingFor(Project sourceProject, IUnitTestGeneratorOptions baseOptions, bool interactive, bool skipManuallySelectedTargetResolution, IMessageLogger logger, IUnitTestGeneratorPackage package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            logger = logger ?? new InertLogger();

            foreach (var mapping in package.ManualProjectMappings)
            {
                baseOptions.ProjectMappings[mapping.Key] = mapping.Value;
            }

            // resolve the options for this project
            var projectOptions = UnitTestGeneratorOptionsFactory.Create(sourceProject.SafeFileName(), baseOptions);

            logger.LogMessage("Creating unit test project mapping for '" + sourceProject.Name + "'.");
            foreach (var source in projectOptions.SourceCounts)
            {
                logger.LogMessage(source.Value + " option(s) loaded from " + source.Key);
            }

            // resolve the target from a session selected project, if any
            string resolvedTarget = ResolveTargetProject(sourceProject, skipManuallySelectedTargetResolution, projectOptions, logger, out var targetProject);

            // now show the UI if in interactive mode, and return the resulting mapping if we need to
            if (interactive && HandleUserInterface(sourceProject, projectOptions, resolvedTarget, targetProject, package, out var resultingMapping))
            {
                return resultingMapping;
            }

            // now resolve the frameworks in use by the target project (if target project is not null and options allow)
            var generationOptions = OptionsResolver.DetectFrameworks(targetProject, projectOptions.GenerationOptions, logger);

            // now create the final options, including resolved frameworks
            var finalOptions = new UnitTestGeneratorOptions(generationOptions, projectOptions.NamingOptions, projectOptions.StrategyOptions, projectOptions.StatisticsCollectionEnabled, new Dictionary<string, ConfigurationSource>());

            // now return the mapping from source to target, along with the per-project options
            return new ProjectMapping(sourceProject, targetProject, resolvedTarget, finalOptions);
        }

        private static bool HandleUserInterface(Project sourceProject, IUnitTestGeneratorOptions projectOptions, string resolvedTarget, Project targetProject, IUnitTestGeneratorPackage package, out ProjectMapping resultingMapping)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            resultingMapping = null;

            var shouldShowUi = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

            switch (projectOptions.GenerationOptions.UserInterfaceMode)
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
                var window = new GenerationDialog(sourceProject, projectOptions, resolvedTarget, new ConfigurationWriterFactory(package));
                var helper = new WindowInteropHelper(window);
#if VS2022
                helper.Owner = sourceProject.DTE.MainWindow.HWnd;
#elif VS2019
                helper.Owner = new System.IntPtr(sourceProject.DTE.MainWindow.HWnd);
#endif

                var result = window.ShowDialog();
                if (result.HasValue)
                {
                    if (result.Value)
                    {
                        resultingMapping = window.ResultingMapping;
                    }

                    return true;
                }
            }

            return false;
        }

        private static string ResolveTargetProject(Project sourceProject, bool skipManuallySelectedTargetResolution, IUnitTestGeneratorOptions projectOptions, IMessageLogger logger, out Project targetProject)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // look to see if we have a mapping for the source project
            if (!skipManuallySelectedTargetResolution && projectOptions.ProjectMappings.TryGetValue(sourceProject.Name, out var configuredTargetProjectName))
            {
                targetProject = VsProjectHelper.FindProject(sourceProject.DTE.Solution, configuredTargetProjectName);
                if (targetProject != null)
                {
                    logger.LogMessage("Using the target project '" + configuredTargetProjectName + "' because it was resolved through configured mappings.");
                    return configuredTargetProjectName;
                }
            }

            // derive the target project name
            var targetProjectNames = projectOptions.GenerationOptions.GetTargetProjectNames(sourceProject.Name).ToList();

            if (targetProjectNames.Count == 0)
            {
                throw new InvalidOperationException("No target project names could be derived from the souce name '" + sourceProject.Name + "'");
            }

            foreach (var targetProjectName in targetProjectNames)
            {
                var project = VsProjectHelper.FindProject(sourceProject.DTE.Solution, targetProjectName);
                if (project != null)
                {
                    logger.LogMessage("Found target project '" + targetProjectName + "'.");
                    targetProject = project;
                    return targetProjectName;
                }
                else
                {
                    logger.LogMessage("Could not find project '" + targetProjectName + "'...");
                }
            }

            logger.LogMessage("No project naming patterns matched a project. Using '" + targetProjectNames[0] + "'.");
            targetProject = null;
            return targetProjectNames[0];
        }
    }
}
