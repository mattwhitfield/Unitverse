using EnvDTE;
using System;
using System.Collections.Generic;
using Unitverse.Core.Assets;
using Unitverse.Core.Options;

namespace Unitverse.Helper
{
    public class ProjectMapping
    {
        public ProjectMapping(Project sourceProject, Project targetProject, IUnitTestGeneratorOptions options)
        {
            SourceProject = sourceProject ?? throw new ArgumentNullException(nameof(sourceProject));
            TargetProject = targetProject ?? throw new ArgumentNullException(nameof(targetProject));
            Options = options ?? throw new ArgumentNullException(nameof(options));
            TargetAssets = new HashSet<TargetAsset>();
        }

        public Project SourceProject { get; }
        public Project TargetProject { get; }
        public IUnitTestGeneratorOptions Options { get; }
        public HashSet<TargetAsset> TargetAssets { get; }
    }
}
