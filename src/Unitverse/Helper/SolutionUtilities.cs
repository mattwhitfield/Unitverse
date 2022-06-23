namespace Unitverse.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.Shell;

    public static class SolutionUtilities
    {
        public static Project GetSupportedProject(DTE2 dte)
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
                return selectedItemObjects.OfType<Project>().FirstOrDefault(x => x.FileName.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase));
#pragma warning restore VSTHRD010
            }

            return null;
        }

        public static IEnumerable<ProjectItemModel> GetSupportedFiles(DTE2 dte, bool recursive)
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
                var project = selectedItemObjects.OfType<Project>().FirstOrDefault();
                if (project != null)
                {
                    // we pass isRoot = false here, because we're already descending into the project
                    return GetSupportedFiles(project.ProjectItems.OfType<ProjectItem>(), recursive, false);
                }
                
                return GetSupportedFiles(selectedItemObjects.OfType<ProjectItem>(), recursive, true);
#pragma warning restore VSTHRD010
            }

            return Enumerable.Empty<ProjectItemModel>();
        }

        private static IEnumerable<ProjectItemModel> GetSupportedFiles(IEnumerable<ProjectItem> items, bool recursive, bool isRoot)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            foreach (var item in items)
            {
                var model = new ProjectItemModel(item);

                if (model.IsSupported)
                {
                    yield return model;
                }

                if (item.ProjectItems.Count > 0 && recursive)
                {
                    // if it's a supported model, and isRoot = true (i.e. we're not already in a recursive descent) then we just want to return
                    // the selected model without recursing into any dependent class partials.
                    if (model.IsSupported && isRoot)
                    {
                        continue;
                    }

                    foreach (var projectItemSummary in GetSupportedFiles(item.ProjectItems.OfType<ProjectItem>(), true, false))
                    {
                        yield return projectItemSummary;
                    }
                }
            }
        }
    }
}