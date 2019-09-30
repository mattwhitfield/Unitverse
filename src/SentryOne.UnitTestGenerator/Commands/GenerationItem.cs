namespace SentryOne.UnitTestGenerator.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using EnvDTE;
    using Microsoft.CodeAnalysis;
    using SentryOne.UnitTestGenerator.Core.Assets;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Options;
    using SentryOne.UnitTestGenerator.Helper;

    public class GenerationItem
    {
        private readonly IGenerationOptions _options;

        private readonly string _targetPath;

        public GenerationItem(ProjectItemModel source, SyntaxNode sourceSymbol, ProjectItems targetProjectItems, string targetPath, HashSet<TargetAsset> requiredAssets, HashSet<IReferencedAssembly> assemblyReferences, Func<string, string> namespaceTransform, IGenerationOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            Source = source ?? throw new ArgumentNullException(nameof(source));
            SourceSymbol = sourceSymbol;
            TargetProjectItems = targetProjectItems ?? throw new ArgumentNullException(nameof(targetProjectItems));
            _targetPath = targetPath;
            RequiredAssets = requiredAssets ?? throw new ArgumentNullException(nameof(requiredAssets));
            NamespaceTransform = namespaceTransform ?? throw new ArgumentNullException(nameof(namespaceTransform));
            AssemblyReferences = assemblyReferences ?? throw new ArgumentNullException(nameof(assemblyReferences));
        }

        public HashSet<IReferencedAssembly> AssemblyReferences { get; }

        public Func<string, string> NamespaceTransform { get; }

        public HashSet<TargetAsset> RequiredAssets { get; }

        public ProjectItemModel Source { get; }

        public SyntaxNode SourceSymbol { get; }

        public string TargetContent { get; set; }

        public string TargetFileName => Path.Combine(_targetPath, _options.GetTargetFileName(Path.GetFileNameWithoutExtension(Source.FilePath)) + Path.GetExtension(Source.FilePath));

        public ProjectItems TargetProjectItems { get; }
    }
}