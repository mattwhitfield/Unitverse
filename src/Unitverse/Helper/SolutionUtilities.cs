namespace Unitverse.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.Shell;
    using Unitverse.Core.Options;

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