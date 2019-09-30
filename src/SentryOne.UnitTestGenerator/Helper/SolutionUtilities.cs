namespace SentryOne.UnitTestGenerator.Helper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using EnvDTE;
    using EnvDTE100;
    using EnvDTE80;
    using Microsoft.VisualStudio.Shell;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Options;
    using SentryOne.UnitTestGenerator.Properties;

    public static class SolutionUtilities
    {
        public static IEnumerable<ProjectItemModel> GetSelectedFiles(DTE2 dte, bool recursive, IGenerationOptions options)
        {
            if (dte == null)
            {
                throw new ArgumentNullException(nameof(dte));
            }

            ThreadHelper.ThrowIfNotOnUIThread();

            if (dte.ToolWindows.SolutionExplorer.SelectedItems is Array selectedItems)
            {
#pragma warning disable VSTHRD010
                var selectedItemObjects = selectedItems.Cast<UIHierarchyItem>().Select(x => x.Object).ToList();
                var items = selectedItemObjects.OfType<ProjectItem>().Concat(selectedItemObjects.OfType<Project>().Select(x => x.ProjectItems).SelectMany(x => x.OfType<ProjectItem>()));
#pragma warning restore VSTHRD010

                return GetSelectedFiles(items, recursive, options);
            }

            return Enumerable.Empty<ProjectItemModel>();
        }

        public static Project CreateTestProjectInCurrentSolution(DTE2 dte, Project sourceProject, IGenerationOptions options)
        {
            if (dte == null)
            {
                throw new ArgumentNullException(nameof(dte));
            }

            if (sourceProject == null)
            {
                throw new ArgumentNullException(nameof(sourceProject));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            ThreadHelper.ThrowIfNotOnUIThread();

            // ReSharper disable once SuspiciousTypeConversion.Global
            var solution = (Solution4)dte.Solution;
            var testProjectName = options.GetTargetProjectName(sourceProject.Name);

            var directoryName = Path.GetDirectoryName(solution.FileName);

            if (string.IsNullOrWhiteSpace(directoryName))
            {
                throw new InvalidOperationException(Strings.SolutionUtilities_CreateTestProjectInCurrentSolution_Cannot_find_source_project_location);
            }

            var destination = Path.Combine(directoryName, testProjectName);

            var templatePath = solution.GetProjectTemplate("ClassLibrary.zip", "CSharp");
            var testProject = dte.Solution.AddFromTemplate(templatePath, destination, testProjectName);
            foreach (var p in solution.Projects.OfType<Project>())
            {
                if (p.Name == testProjectName)
                {
                    (p.Object as VSLangProj.VSProject)?.References.AddProject(sourceProject);

                    foreach (var projectItem in p.ProjectItems.OfType<ProjectItem>())
                    {
                        try
                        {
                            if (projectItem.FileCount > 0)
                            {
                                File.Delete(projectItem.FileNames[0]);
                            }
                        }
                        catch (IOException)
                        {
                        }

                        try
                        {
                            projectItem.Remove();
                        }
#pragma warning disable CA1031 // Do not catch general exception types
                        catch
#pragma warning restore CA1031 // Do not catch general exception types
                        {
                            // ignored - VS project system is extensible and we don't know exception types up-front
                        }
                    }

                    return p;
                }
            }

            return testProject;
        }

        private static IEnumerable<ProjectItemModel> GetSelectedFiles(IEnumerable<ProjectItem> items, bool recursive, IGenerationOptions options)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            foreach (var item in items)
            {
                if (item.ProjectItems.Count > 0)
                {
                    if (recursive)
                    {
                        foreach (var projectItemSummary in GetSelectedFiles(item.ProjectItems.OfType<ProjectItem>(), true, options))
                        {
                            yield return projectItemSummary;
                        }
                    }
                }
                else
                {
                    yield return new ProjectItemModel(item, options);
                }
            }
        }
    }
}