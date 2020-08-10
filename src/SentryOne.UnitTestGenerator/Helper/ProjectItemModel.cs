namespace SentryOne.UnitTestGenerator.Helper
{
    using System;
    using System.Globalization;
    using System.Linq;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Options;
    using SentryOne.UnitTestGenerator.Properties;

    public class ProjectItemModel
    {
        private Project _targetProject;

        public ProjectItemModel(ProjectItem projectItem, IGenerationOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            ThreadHelper.ThrowIfNotOnUIThread();

            Item = projectItem ?? throw new ArgumentNullException(nameof(projectItem));
            FilePath = projectItem.FileNames[1];
            TargetProjectName = options.GetTargetProjectName(Item.ContainingProject.Name);
            SourceProjectName = Item.ContainingProject.Name;
            try
            {
                TargetProjectName = string.Format(CultureInfo.CurrentCulture, options.TestProjectNaming, Item.ContainingProject.Name);
            }
            catch (FormatException)
            {
                throw new InvalidOperationException(Strings.ProjectItemModel_ProjectItemModel_Cannot_not_derive_target_project_name__please_check_the_test_project_naming_setting_);
            }
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

        public Project TargetProject
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                return _targetProject ?? (_targetProject = VsProjectHelper.FindProject(Item.DTE.Solution, TargetProjectName));
            }
        }

        public string SourceProjectName { get; }

        public string TargetProjectName { get; }

        public static bool IsSupported(ProjectItemModel item)
        {
            return item?.FilePath != null && item.FilePath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase);
        }
    }
}