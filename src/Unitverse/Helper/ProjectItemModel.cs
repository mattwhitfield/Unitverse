namespace Unitverse.Helper
{
    using System;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;

    public class ProjectItemModel
    {
        public ProjectItemModel(ProjectItem projectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Item = projectItem ?? throw new ArgumentNullException(nameof(projectItem));
            FilePath = projectItem.FileNames[1];
            SourceProjectName = Item.ContainingProject.Name;
        }

        public string FilePath { get; }

        public ProjectItem Item { get; }

        public Project Project
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                return Item.ContainingProject;
            }
        }

        public string SourceProjectName { get; }

        public bool IsSupported => FilePath != null && FilePath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase);
    }
}