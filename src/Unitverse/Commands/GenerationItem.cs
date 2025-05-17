namespace Unitverse.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using EnvDTE;
    using Microsoft.CodeAnalysis;
    using Microsoft.VisualStudio.Shell;
    using Unitverse.Core.Assets;
    using Unitverse.Core.Generation;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;
    using Unitverse.Helper;

    public class GenerationItem : IGenerationItem
    {
        private readonly string _targetPath;

        public GenerationItem(ProjectItemModel source, ProjectMapping mapping)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Mapping = mapping ?? throw new ArgumentNullException(nameof(mapping));

            ThreadHelper.ThrowIfNotOnUIThread();

            var nameParts = VsProjectHelper.GetFolderParts(source.Item);

            if (mapping.Options.GenerationOptions.IncludeSourceProjectAsFolder)
            {
                var sourceName = mapping.SourceProject.Name;
                var sourceNameParts = sourceName.Split('.');
                foreach (var sourceNamePart in sourceNameParts.Reverse())
                {
                    nameParts.Add(sourceNamePart);
                }
            }

            var targetProject = mapping.TargetProject;
            TargetProjectItems = TargetFinder.FindTargetFolder(targetProject, nameParts, true, out _targetPath);

            if (TargetProjectItems == null && !mapping.Options.GenerationOptions.AllowGenerationWithoutTargetProject)
            {
                // we asked to create targetProjectItems - so if it's null we effectively had a problem getting to the target project
                throw new InvalidOperationException("Cannot create tests for '" + Path.GetFileName(source.FilePath) + "' because there is no project '" + mapping.TargetProjectName + "'.");
            }

            NamespaceTransform = mapping.CreateNamespaceTransform();
        }

        public Func<string, string> NamespaceTransform { get; }

        public HashSet<TargetAsset> RequiredAssets => Mapping.TargetAssets;

        public ProjectItemModel Source { get; }
        public ProjectMapping Mapping { get; }
        public ISymbol SourceSymbol { get; set; }
        public SyntaxNode SourceNode { get; set; }

        public string TargetContent { get; set; }

        public string OverrideTargetFileName { get; set; }

        public string TargetFileName
        {
            get
            {
                var targetFileName = OverrideTargetFileName ?? Mapping.Options.GenerationOptions.GetTargetFileName(Source.TransformableName) + Source.TransformableSuffix;
                if (string.IsNullOrEmpty(_targetPath))
                {
                    return targetFileName;
                }

                return Path.Combine(_targetPath, targetFileName);
            }
        }

        public ProjectItems TargetProjectItems { get; }

        public IUnitTestGeneratorOptions Options => Mapping.Options;

        public bool AnyMethodsEmitted { get; set; }
    }
}