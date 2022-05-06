namespace Unitverse.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using EnvDTE;
    using Microsoft;
    using Microsoft.VisualStudio.OperationProgress;
    using Microsoft.VisualStudio.Shell;
    using Unitverse.Core.Models;
    using VSLangProj;
    using VSLangProj80;

    internal static class ReferencesHelper
    {
        public static void AddNugetPackagesAndProjectReferences(IUnitTestGeneratorPackage package, Project source, IList<INugetPackageReference> packagesToInstall, IList<Project> projectsToReference)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var logger = new AggregateLogger();

            package.JoinableTaskFactory.Run(async () =>
            {
                await WaitableActionHelper.RunWaitableActionAsync(package, logger, "Creating test project", async logMessage =>
                {
                    logMessage("Waiting for project initialization...");

                    var operationProgressStatusService = package.GetService(typeof(SVsOperationProgressStatusService)) as IVsOperationProgressStatusService;
                    Assumes.Present(operationProgressStatusService);
                    var stageStatus = operationProgressStatusService.GetStageStatus(CommonOperationProgressStageIds.Intellisense);

                    await package.JoinableTaskFactory.RunAsync(stageStatus.WaitForCompletionAsync);

                    await package.JoinableTaskFactory.SwitchToMainThreadAsync();

                    foreach (var installablePackage in packagesToInstall)
                    {
                        var message = string.Format(CultureInfo.CurrentCulture, "Installing package '{0}'...", installablePackage.Name);
                        logMessage(message);

                        InstallPackage(source, package, installablePackage);
                    }

                    var vsLangProj = source.Object as VSProject;
                    var existingReferences = new HashSet<string>(vsLangProj.References.OfType<Reference3>().Select(x => x.SourceProject?.Name).Where(x => !string.IsNullOrWhiteSpace(x)), StringComparer.OrdinalIgnoreCase);

                    foreach (var project in projectsToReference)
                    {
                        if (!existingReferences.Contains(project.Name))
                        {
                            var message = string.Format(CultureInfo.CurrentCulture, "Referencing project '{0}'...", project.Name);
                            logMessage(message);

                            vsLangProj.References.AddProject(project);
                        }
                    }
                });
            });


        }

        private static void InstallPackage(Project currentProject, IUnitTestGeneratorPackage generatorPackage, INugetPackageReference package)
        {
            try
            {
                generatorPackage.PackageInstaller.InstallPackage("All", currentProject, package.Name, package.Version, false);
            }
            catch (InvalidOperationException)
            {
                generatorPackage.PackageInstaller.InstallPackage("https://www.nuget.org/api/v2/", currentProject, package.Name, package.Version, false);
            }
        }
    }
}
