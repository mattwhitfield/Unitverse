namespace SentryOne.UnitTestGenerator.Helper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.VisualStudio.Shell;
    using SentryOne.UnitTestGenerator.Commands;
    using SentryOne.UnitTestGenerator.Core;
    using SentryOne.UnitTestGenerator.Core.Assets;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Options;
    using SentryOne.UnitTestGenerator.Properties;
    using Project = EnvDTE.Project;
    using Solution = Microsoft.CodeAnalysis.Solution;
    using Task = System.Threading.Tasks.Task;

    internal static class CodeGenerator
    {
        public static async Task GenerateCodeAsync(IReadOnlyCollection<GenerationItem> generationItems, bool withRegeneration, IUnitTestGeneratorPackage package, Dictionary<Project, Tuple<HashSet<TargetAsset>, HashSet<IReferencedAssembly>>> requiredAssetsByProject, IMessageLogger messageLogger)
        {
            var solution = package.Workspace.CurrentSolution;
            var options = package.Options;

            foreach (var generationItem in generationItems)
            {
                await GenerateItemAsync(withRegeneration, options, solution, generationItem).ConfigureAwait(true);
            }

            await package.JoinableTaskFactory.SwitchToMainThreadAsync();

            Attempt.Action(
                () =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                messageLogger.LogMessage("Adding required assets to target project...");
                foreach (var pair in requiredAssetsByProject)
                {
                    AddTargetAssets(options, pair);

                    if (options.GenerationOptions.AddReferencesAutomatically)
                    {
                        ReferencesHelper.AddReferencesToProject(pair.Key, pair.Value.Item2.ToList(), messageLogger.LogMessage);
                    }
                }

                if (generationItems.All(x => string.IsNullOrWhiteSpace(x.TargetContent)))
                {
                    throw new InvalidOperationException("None of the selected targets contained a testable type. Tests can only be generated for classes and structs");
                }

                messageLogger.LogMessage("Adding generated items to target project...");
                foreach (var generationItem in generationItems.Where(x => !string.IsNullOrWhiteSpace(x.TargetContent)))
                {
                    if (generationItem.TargetProjectItems != null)
                    {
                        AddTargetItem(generationItems, options, generationItem);
                    }
                    else
                    {
                        CreateUnsavedFile(package, generationItem);
                    }
                }

                messageLogger.LogMessage("Generation complete.");
            }, package);
        }

        private static void CreateUnsavedFile(IUnitTestGeneratorPackage package, GenerationItem generationItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var tempFile = Path.Combine(Path.GetTempPath(), Path.GetFileName(generationItem.TargetFileName));
            try
            {
                File.WriteAllText(tempFile, Strings.DisconnectedFileHeader + generationItem.TargetContent);
                var dte = (DTE2)package.GetService(typeof(DTE));
                if (dte != null)
                {
                    var window = dte.ItemOperations.OpenFile(tempFile, Constants.vsViewKindCode);
                    window.Document.Saved = false;
                }
            }
            finally
            {
                try
                {
                    File.Delete(tempFile);
                }
                catch (IOException)
                {
                }
            }
        }

        private static void AddTargetItem(IReadOnlyCollection<GenerationItem> generationItems, IUnitTestGeneratorOptions options, GenerationItem generationItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (TargetFinder.FindExistingTargetItem(generationItem.Source, options.GenerationOptions, out var targetItem) != FindTargetStatus.Found)
            {
                File.WriteAllText(generationItem.TargetFileName, generationItem.TargetContent);
                targetItem = generationItem.TargetProjectItems.AddFromFile(generationItem.TargetFileName);
            }
            else
            {
                if (targetItem.Document is null)
                {
                    File.WriteAllText(generationItem.TargetFileName, generationItem.TargetContent);
                }
                else
                {
                    var textSelection = (TextSelection)targetItem.Document.Selection;
                    textSelection.SelectAll();
                    textSelection.Insert(generationItem.TargetContent);
                    targetItem.Document.Save();
                }
            }

            if (generationItems.Count == 1)
            {
                VsProjectHelper.ActivateItem(targetItem);
            }
        }

        private static void AddTargetAssets(IUnitTestGeneratorOptions options, KeyValuePair<Project, Tuple<HashSet<TargetAsset>, HashSet<IReferencedAssembly>>> pair)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            foreach (var targetAsset in pair.Value.Item1)
            {
                var asset = AssetFactory.Create(targetAsset);
                if (asset != null)
                {
                    var targetProjectFileName = pair.Key.FileName;
                    var targetProjectPath = Path.GetDirectoryName(targetProjectFileName);

                    if (string.IsNullOrWhiteSpace(targetProjectPath))
                    {
                        continue;
                    }

#pragma warning disable VSTHRD010
                    if (!pair.Key.ProjectItems.OfType<ProjectItem>().Any(x => string.Equals(x.Name, asset.AssetFileName, StringComparison.OrdinalIgnoreCase)))
#pragma warning restore VSTHRD010
                    {
                        var nameSpace = VsProjectHelper.GetProjectRootNamespace(pair.Key);
                        var fileName = Path.Combine(targetProjectPath, asset.AssetFileName);
                        File.WriteAllText(fileName, asset.Content(nameSpace, options.GenerationOptions.FrameworkType));
                        pair.Key.ProjectItems.AddFromFile(fileName);
                    }
                }
            }
        }

        private static async Task GenerateItemAsync(bool withRegeneration, IUnitTestGeneratorOptions options, Solution solution, GenerationItem generationItem)
        {
            var targetProject = solution.Projects.FirstOrDefault(x => string.Equals(x.Name, generationItem.Source.SourceProjectName, StringComparison.Ordinal));
            var documents = solution.GetDocumentIdsWithFilePath(generationItem.Source.FilePath);
            var documentId = documents.FirstOrDefault(x => x.ProjectId == targetProject?.Id) ?? documents.FirstOrDefault();
            if (documentId == null)
            {
                throw new InvalidOperationException("Could not find document in solution with file path '" + generationItem.Source.FilePath + "'");
            }

            var document = solution.GetDocument(documentId);

            var semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(true);

            var tree = await semanticModel.SyntaxTree.GetRootAsync().ConfigureAwait(true);
            if (!tree.DescendantNodes().OfType<ClassDeclarationSyntax>().Any() && !tree.DescendantNodes().OfType<StructDeclarationSyntax>().Any())
            {
                return;
            }

            var result = await GenerateAsync(withRegeneration, options, solution, generationItem, semanticModel).ConfigureAwait(true);

            generationItem.TargetContent = result.FileContent;

            foreach (var asset in result.RequiredAssets)
            {
                generationItem.RequiredAssets.Add(asset);
            }

            foreach (var reference in result.AssemblyReferences)
            {
                if (!generationItem.AssemblyReferences.Any(x => string.Equals(x.Name, reference.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    generationItem.AssemblyReferences.Add(reference);
                }
            }
        }

        private static async Task<GenerationResult> GenerateAsync(bool withRegeneration, IUnitTestGeneratorOptions options, Solution solution, GenerationItem generationItem, SemanticModel semanticModel)
        {
            GenerationResult result;
            if (File.Exists(generationItem.TargetFileName))
            {
                var documentIds = solution.GetDocumentIdsWithFilePath(generationItem.TargetFileName);
                if (documentIds.FirstOrDefault() is DocumentId targetDocumentId)
                {
                    var targetDocument = solution.GetDocument(targetDocumentId);

                    var targetSemanticModel = await targetDocument.GetSemanticModelAsync().ConfigureAwait(true);

                    result = await CoreGenerator.Generate(semanticModel, generationItem.SourceSymbol, targetSemanticModel, withRegeneration, options, generationItem.NamespaceTransform).ConfigureAwait(true);
                }
                else
                {
                    result = await CoreGenerator.Generate(semanticModel, generationItem.SourceSymbol, null, withRegeneration, options, generationItem.NamespaceTransform).ConfigureAwait(true);
                }
            }
            else
            {
                result = await CoreGenerator.Generate(semanticModel, generationItem.SourceSymbol, null, withRegeneration, options, generationItem.NamespaceTransform).ConfigureAwait(true);
            }

            return result;
        }
    }
}
