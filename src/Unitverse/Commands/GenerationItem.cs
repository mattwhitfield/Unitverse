namespace Unitverse.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using EnvDTE;
    using Microsoft.CodeAnalysis;
    using Unitverse.Core.Assets;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;
    using Unitverse.Helper;

    public class GenerationItem
    {
        private readonly string _targetPath;

        public GenerationItem(ProjectItemModel source, SyntaxNode sourceSymbol, ProjectItems targetProjectItems, string targetPath, HashSet<TargetAsset> requiredAssets, Func<string, string> namespaceTransform, IUnitTestGeneratorOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Source = source ?? throw new ArgumentNullException(nameof(source));
            SourceSymbol = sourceSymbol;
            TargetProjectItems = targetProjectItems;
            _targetPath = targetPath;
            RequiredAssets = requiredAssets ?? throw new ArgumentNullException(nameof(requiredAssets));
            NamespaceTransform = namespaceTransform ?? throw new ArgumentNullException(nameof(namespaceTransform));
        }

        public Func<string, string> NamespaceTransform { get; }

        public HashSet<TargetAsset> RequiredAssets { get; }

        public ProjectItemModel Source { get; }

        public SyntaxNode SourceSymbol { get; }

        public string TargetContent { get; set; }

        public string TargetFileName
        {
            get
            {
                var targetFileName = Options.GenerationOptions.GetTargetFileName(Path.GetFileNameWithoutExtension(Source.FilePath)) + Path.GetExtension(Source.FilePath);
                if (string.IsNullOrEmpty(_targetPath))
                {
                    return targetFileName;
                }

                return Path.Combine(_targetPath, targetFileName);
            }
        }

        public ProjectItems TargetProjectItems { get; }

        public IUnitTestGeneratorOptions Options { get; }
    }
}