namespace Unitverse.Helper
{
    using System;
    using System.IO;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;

    public class ProjectItemModel
    {
        public ProjectItemModel(ProjectItem projectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Item = projectItem ?? throw new ArgumentNullException(nameof(projectItem));
            FilePath = projectItem.FileCount > 0 ? projectItem.FileNames[1] : string.Empty;
            SourceProjectName = Item.ContainingProject.Name;

            TransformableName = Path.GetFileNameWithoutExtension(FilePath);
            TransformableSuffix = Path.GetExtension(FilePath);

            if (Item.Collection.Parent is ProjectItem parent)
            {
                if (Guid.TryParse(parent.Kind, out Guid parentKind) &&
                    parentKind == VsProjectHelper.FileKind) 
                {
                    var parentFilePath = parent.FileCount > 0 ? parent.FileNames[1] : string.Empty;
                    var parentFileName = Path.GetFileNameWithoutExtension(parentFilePath);

                    // if the item we're looking at has a file name that starts with the same root as it's parent,
                    // then we generate the tests name from the parent, and include the part after the root as the 
                    // suffix
                    if (TransformableName.StartsWith(parentFileName + "."))
                    {
                        TransformableSuffix = TransformableName.Substring(parentFileName.Length) + TransformableSuffix;
                        TransformableName = parentFileName;
                    }
                }
            }
        }

        public string FilePath { get; }

        public string TransformableName { get; }

        public string TransformableSuffix { get; }

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