namespace Unitverse.Helper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using EnvDTE;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.VisualStudio.Shell;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    internal static class TargetFinder
    {
        public static FindTargetStatus FindExistingTargetItem(ISymbol symbol, ProjectItemModel source, ProjectMapping mapping, IUnitTestGeneratorPackage package, IMessageLogger messageLogger, out ProjectItem targetItem, out bool wasRedirection)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

#pragma warning disable VSTHRD010
            var nameParts = VsProjectHelper.GetFolderParts(source.Item);
            targetItem = null;
            wasRedirection = false;

            var targetProject = mapping.TargetProject;
            if (targetProject == null)
            {
                return FindTargetStatus.ProjectNotFound;
            }

            var targetProjectItems = FindTargetFolder(targetProject, nameParts, false, out _);
            if (targetProjectItems == null)
            {
                if (mapping.Options.GenerationOptions.FallbackTargetFinding == FallbackTargetFindingMethod.None)
                {
                    return FindTargetStatus.FolderNotFound;
                }
            }

            if (targetProjectItems != null)
            {
                var testFileName = mapping.Options.GenerationOptions.GetTargetFileName(source.TransformableName) + source.TransformableSuffix;

                targetItem = VsProjectHelper.Find(targetProjectItems, testFileName);

                if (targetItem != null)
                {
                    return FindTargetStatus.Found;
                }
                else if (mapping.Options.GenerationOptions.FallbackTargetFinding == FallbackTargetFindingMethod.None)
                {
                    return FindTargetStatus.FileNotFound;
                }
            }

            try
            {
                var namespaceTransform = mapping.CreateNamespaceTransform();
                var targetProjectName = targetProject.Name;
                var findTargetByTypeTask = package.JoinableTaskFactory.RunAsync(async () => await FindTargetByTypeAsync(symbol, source, mapping, package, targetProjectName, namespaceTransform).ConfigureAwait(true));
                var targetFileName = findTargetByTypeTask.Join();

                if (!string.IsNullOrWhiteSpace(targetFileName))
                {
                    targetItem = VsProjectHelper.GetProjectItem(targetFileName);
                    if (targetItem != null)
                    {
                        wasRedirection = true;
                        return FindTargetStatus.Found;
                    }
                }
            }
            catch (Exception ex)
            {
                // we'd like the info logged out, but errors here will revert to previous behaviour
                messageLogger.LogMessage("Error while finding type by name: " + ex.Message + "\r\n" + ex.StackTrace);
                targetItem = null;
            }

            return FindTargetStatus.FileNotFound;
#pragma warning restore VSTHRD010
        }

        private static async Task<string> FindTargetByTypeAsync(ISymbol symbol, ProjectItemModel source, ProjectMapping mapping, IUnitTestGeneratorPackage package, string targetProjectName, Func<string, string> namespaceTransform)
        {
            if (symbol == null)
            {
                var semanticModel = await CodeGenerator.GetSemanticModelAsync(package.Workspace.CurrentSolution, source).ConfigureAwait(true);

                var tree = await semanticModel.SyntaxTree.GetRootAsync().ConfigureAwait(true);

                var firstType = tree.DescendantNodes().FirstOrDefault(node => node is ClassDeclarationSyntax || node is StructDeclarationSyntax || node is RecordDeclarationSyntax) as TypeDeclarationSyntax;

                if (firstType != null && !firstType.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
                {
                    symbol = semanticModel.GetDeclaredSymbol(firstType);
                }
            }

            if (symbol == null)
            {
                return null;
            }

            string fileName = null;

            var project = package.Workspace.CurrentSolution.Projects.FirstOrDefault(x => x.Name == targetProjectName);
            if (project != null)
            {
                // navigate up to a named type
                while (symbol != null && !(symbol is INamedTypeSymbol))
                {
                    symbol = symbol.ContainingSymbol;
                }

                if (symbol is INamedTypeSymbol typeSymbol)
                {
                    var locations = symbol.Locations
                                          .Select(x => x.SourceTree?.FilePath)
                                          .Where(x => x != null)
                                          .Distinct(StringComparer.OrdinalIgnoreCase)
                                          .ToList();

                    if (locations.Count == 1)
                    {
                        var compilation = await project.GetCompilationAsync().ConfigureAwait(true);

                        var typeSymbolProvider = new TypeSymbolProvider(typeSymbol);
                        ISymbol targetSymbol = null;
                        if (mapping.Options.GenerationOptions.FallbackTargetFinding == FallbackTargetFindingMethod.TypeInAnyNamespace)
                        {
                            var definedSymbol = compilation.GetSymbolsWithName(x => x != null && x.EndsWith(mapping.Options.GenerationOptions.GetTargetTypeName(typeSymbolProvider), StringComparison.OrdinalIgnoreCase), SymbolFilter.Type);

                            targetSymbol = definedSymbol?.FirstOrDefault();
                        }
                        else if (mapping.Options.GenerationOptions.FallbackTargetFinding == FallbackTargetFindingMethod.TypeInCorrectNamespace)
                        {
                            // derive typeName using namespace transform - code generator must already do this
                            var typeName = mapping.Options.GenerationOptions.GetFullyQualifiedTargetTypeName(typeSymbolProvider, namespaceTransform);
                            targetSymbol = compilation.GetTypeByMetadataName(typeName);
                        }

                        if (targetSymbol != null)
                        {
                            fileName = targetSymbol.Locations.Where(x => x.IsInSource).Select(x => x.SourceTree?.FilePath).FirstOrDefault(f => !string.IsNullOrWhiteSpace(f));
                        }
                    }
                }
            }

            return fileName;
        }

        public static ProjectItems FindTargetFolder(EnvDTE.Project targetProject, List<string> folderParts, bool createMissingFolders, out string targetPath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (targetProject == null)
            {
                targetPath = null;
                return null;
            }

            var fileName = targetProject.SafeFileName();
            if (string.IsNullOrEmpty(fileName))
            {
                targetPath = null;
                return null;
            }

#pragma warning disable VSTHRD010
            var targetProjectItems = targetProject.ProjectItems;
            targetPath = Path.GetDirectoryName(fileName);
            for (var i = folderParts.Count - 1; i >= 0; i--)
            {
                var currentNamePart = folderParts[i];
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
