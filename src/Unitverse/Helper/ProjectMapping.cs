using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using Unitverse.Core.Assets;
using Unitverse.Core.Helpers;
using Unitverse.Core.Options;

namespace Unitverse.Helper
{
    public class ProjectMapping
    {
        public ProjectMapping(Project sourceProject, Project targetProject, string targetProjectName, IUnitTestGeneratorOptions options)
        {
            SourceProject = sourceProject ?? throw new ArgumentNullException(nameof(sourceProject));
            TargetProject = targetProject;
            TargetProjectName = targetProjectName;
            Options = options ?? throw new ArgumentNullException(nameof(options));
            TargetAssets = new HashSet<TargetAsset>();
        }

        public Project SourceProject { get; }
        public Project TargetProject { get; }
        public string TargetProjectName { get; }
        public IUnitTestGeneratorOptions Options { get; }
        public HashSet<TargetAsset> TargetAssets { get; }

        public Func<string, string> CreateNamespaceTransform()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var sourceNameSpaceRoot = VsProjectHelper.GetProjectRootNamespace(SourceProject);

            if (TargetProject != null)
            {
                var targetNameSpaceRoot = VsProjectHelper.GetProjectRootNamespace(TargetProject);
                return NamespaceTransform.Create(sourceNameSpaceRoot, targetNameSpaceRoot);
            }

            return x => x + ".Tests";
        }
    }
}
