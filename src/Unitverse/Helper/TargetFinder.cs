namespace Unitverse.Helper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using Unitverse.Core.Helpers;

    internal static class TargetFinder
    {
        public static FindTargetStatus FindExistingTargetItem(ProjectItemModel source, ProjectMapping mapping, out ProjectItem targetItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

#pragma warning disable VSTHRD010
            var nameParts = VsProjectHelper.GetNameParts(source.Item);
            targetItem = null;

            var targetProject = mapping.TargetProject;
            if (targetProject == null)
            {
                return FindTargetStatus.ProjectNotFound;
            }

            var targetProjectItems = FindTargetFolder(targetProject, nameParts, false, out _);
            if (targetProjectItems == null)
            {
                return FindTargetStatus.FolderNotFound;
            }

            var extension = Path.GetExtension(source.FilePath);
            var baseName = Path.GetFileNameWithoutExtension(source.FilePath);

            var testFileName = mapping.Options.GenerationOptions.GetTargetFileName(baseName);
            targetItem = targetProjectItems.OfType<ProjectItem>().FirstOrDefault(x => string.Equals(x.Name, testFileName + extension, StringComparison.OrdinalIgnoreCase));
            return targetItem == null ? FindTargetStatus.FileNotFound : FindTargetStatus.Found;
#pragma warning restore VSTHRD010
        }

        public static ProjectItems FindTargetFolder(Project targetProject, List<string> nameParts, bool createMissingFolders, out string targetPath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (targetProject == null)
            {
                targetPath = null;
                return null;
            }

            string fileName;
            try
            {
                fileName = targetProject.FileName;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                targetPath = null;
                return null;
            }

#pragma warning disable VSTHRD010
            var targetProjectItems = targetProject.ProjectItems;
            targetPath = Path.GetDirectoryName(fileName);
            for (var i = nameParts.Count - 1; i > 0; i--)
            {
                var currentNamePart = nameParts[i];
                var item = targetProjectItems.OfType<ProjectItem>().FirstOrDefault(x => string.Equals(x.Name, currentNamePart, StringComparison.OrdinalIgnoreCase));
                if (item != null)
                {
                    targetProjectItems = item.ProjectItems;
                    targetPath = item.FileNames[0];
                }
                else
                {
                    if (!createMissingFolders)
                    {
                        return null;
                    }

                    if (string.IsNullOrWhiteSpace(targetPath))
                    {
                        return null;
                    }

                    var targetDirectoryInfo = new DirectoryInfo(Path.Combine(targetPath, currentNamePart));
                    targetDirectoryInfo.Refresh();

                    var newItem = targetDirectoryInfo.Exists ?
                        targetProjectItems.AddFromDirectory(targetDirectoryInfo.FullName) :
                        targetProjectItems.AddFolder(currentNamePart, string.Empty);

                    targetProjectItems = newItem.ProjectItems;
                    targetPath = newItem.FileNames[0];
                }
            }

            return targetProjectItems;
#pragma warning restore VSTHRD010
        }
    }
}
