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
                var items = selectedItemObjects.OfType<ProjectItem>().Concat(selectedItemObjects.OfType<Project>().Select(x => x.ProjectItems).SelectMany(x => x.OfType<ProjectItem>()));
#pragma warning restore VSTHRD010

                return GetSupportedFiles(items, recursive);
            }

            return Enumerable.Empty<ProjectItemModel>();
        }

        private static IEnumerable<ProjectItemModel> GetSupportedFiles(IEnumerable<ProjectItem> items, bool recursive)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            foreach (var item in items)
            {
                if (item.ProjectItems.Count > 0)
                {
                    if (recursive)
                    {
                        foreach (var projectItemSummary in GetSupportedFiles(item.ProjectItems.OfType<ProjectItem>(), true))
                        {
                            yield return projectItemSummary;
                        }
                    }
                }
                else
                {
                    var model = new ProjectItemModel(item);
                    if (model.IsSupported)
                    {
                        yield return model;
                    }
                }
            }
        }
    }
}