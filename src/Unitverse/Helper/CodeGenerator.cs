namespace Unitverse.Helper
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
    using Unitverse.Commands;
    using Unitverse.Core;
    using Unitverse.Core.Assets;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using Unitverse.Properties;
    using Project = EnvDTE.Project;
    using Solution = Microsoft.CodeAnalysis.Solution;
    using Task = System.Threading.Tasks.Task;

    internal static class CodeGenerator
    {
        public static async Task GenerateCodeAsync(IReadOnlyCollection<GenerationItem> generationItems, bool withRegeneration, IUnitTestGeneratorPackage package, IMessageLogger messageLogger)
        {
            var solution = package.Workspace.CurrentSolution;

            var isSingleItemGeneration = generationItems.Count == 1;
            foreach (var generationItem in generationItems)
            {
                await GenerateItemAsync(withRegeneration, solution, generationItem, isSingleItemGeneration, messageLogger).ConfigureAwait(true);
            }

            await package.JoinableTaskFactory.SwitchToMainThreadAsync();

            Attempt.Action(
                () =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                if (!isSingleItemGeneration && generationItems.All(x => !x.AnyMethodsEmitted))
                {
                    var message = "No new methods were generated for any selected item. If you want to re-generate existing tests, please hold left shift while opening the context menu and select the 'Regenerate tests' option.";
                    messageLogger.LogMessage(message);
                    VsMessageBox.Show(message, false, package);
                    return;
                }

                messageLogger.LogMessage("Adding required assets to target project...");
                var mapping = generationItems.FirstOrDefault()?.Mapping;
                if (mapping != null)
                {
                    AddTargetAssets(mapping.Options, mapping.TargetProject, mapping.TargetAssets);
                }

                if (generationItems.All(x => string.IsNullOrWhiteSpace(x.TargetContent)))
                {
                    throw new InvalidOperationException("None of the selected targets contained a testable type. Tests can only be generated for classes and structs.");
                }

                messageLogger.LogMessage("Adding generated items to target project...");

                foreach (var generationItem in generationItems.Where(x => !string.IsNullOrWhiteSpace(x.TargetContent)))
                {
                    if (generationItem.TargetProjectItems != null)
                    {
                        AddTargetItem(package, generationItem, generationItems.Count == 1, messageLogger);
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
                var header = string.IsNullOrWhiteSpace(generationItem.Mapping.TargetProjectName) ?
                     Strings.ElectiveDisconnectedFileHeader :
                     Strings.DisconnectedFileHeader.Replace("$$TARGETNAME$$", generationItem.Mapping.TargetProjectName);

                File.WriteAllText(tempFile, header + generationItem.TargetContent);
                var dte = (DTE2)package.GetService(typeof(DTE));
                if (dte != null)
                {
                    var window = dte.ItemOperations.OpenFile(tempFile, EnvDTE.Constants.vsViewKindCode);
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

        private static void AddTargetItem(IUnitTestGeneratorPackage package, GenerationItem generationItem, bool withActivation, IMessageLogger logger)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (TargetFinder.FindExistingTargetItem(generationItem.SourceSymbol, generationItem.Source, generationItem.Mapping, package, logger, out var targetItem) != FindTargetStatus.Found)
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

            if (withActivation)
            {
                VsProjectHelper.ActivateItem(targetItem);
            }
        }

        private static void AddTargetAssets(IUnitTestGeneratorOptions options, Project project, HashSet<TargetAsset> targetAssets)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            foreach (var targetAsset in targetAssets)
            {
                var asset = AssetFactory.Create(targetAsset);
                if (asset != null)
                {
                    var targetProjectFileName = project.SafeFileName();

                    if (string.IsNullOrWhiteSpace(targetProjectFileName))
                    {
                        continue;
                    }

                    var targetProjectPath = Path.GetDirectoryName(targetProjectFileName);

                    if (string.IsNullOrWhiteSpace(targetProjectPath))
                    {
                        continue;
                    }

#pragma warning disable VSTHRD010
                    if (!project.ProjectItems.OfType<ProjectItem>().Any(x => string.Equals(x.Name, asset.AssetFileName, StringComparison.OrdinalIgnoreCase)))
#pragma warning restore VSTHRD010
                    {
                        var nameSpace = VsProjectHelper.GetProjectRootNamespace(project);
                        var fileName = Path.Combine(targetProjectPath, asset.AssetFileName);
                        File.WriteAllText(fileName, asset.Content(nameSpace, options.GenerationOptions.FrameworkType));
                        project.ProjectItems.AddFromFile(fileName);
                    }
                }
            }
        }

        public static Task<SemanticModel> GetSemanticModelAsync(Solution solution, ProjectItemModel source)
        {
            var sourceProject = solution.Projects.FirstOrDefault(x => string.Equals(x.Name, source.SourceProjectName, StringComparison.Ordinal));
            var documents = solution.GetDocumentIdsWithFilePath(source.FilePath);
            var documentId = documents.FirstOrDefault(x => x.ProjectId == sourceProject?.Id) ?? documents.FirstOrDefault();
            if (documentId == null)
            {
                throw new InvalidOperationException("Could not find document in solution with file path '" + source.FilePath + "'");
            }

            var document = solution.GetDocument(documentId);

            return document.GetSemanticModelAsync();
        }

        private static async Task GenerateItemAsync(bool withRegeneration, Solution solution, GenerationItem generationItem, bool isSingleItemGeneration, IMessageLogger messageLogger)
        {
            var semanticModel = await GetSemanticModelAsync(solution, generationItem.Source).ConfigureAwait(true);

            var tree = await semanticModel.SyntaxTree.GetRootAsync().ConfigureAwait(true);
            if (!tree.DescendantNodes().Any(node => node is ClassDeclarationSyntax || node is StructDeclarationSyntax || node is RecordDeclarationSyntax))
            {
                return;
            }

            var result = await GenerateAsync(withRegeneration, solution, generationItem, semanticModel, isSingleItemGeneration, messageLogger).ConfigureAwait(true);

            if (generationItem.Options.StatisticsCollectionEnabled)
            {
                StatisticsTracker.Track(result);
            }

            generationItem.TargetContent = result.FileContent;
            generationItem.AnyMethodsEmitted = result.AnyMethodsEmitted;

            foreach (var asset in result.RequiredAssets)
            {
                generationItem.RequiredAssets.Add(asset);
            }
        }

        private static async Task<GenerationResult> GenerateAsync(bool withRegeneration, Solution solution, GenerationItem generationItem, SemanticModel semanticModel, bool isSingleItemGeneration, IMessageLogger messageLogger)
        {
            if (File.Exists(generationItem.TargetFileName))
            {
                var documentIds = solution.GetDocumentIdsWithFilePath(generationItem.TargetFileName);
                if (documentIds.FirstOrDefault() is DocumentId targetDocumentId)
                {
                    var targetDocument = solution.GetDocument(targetDocumentId);

                    var targetSemanticModel = await targetDocument.GetSemanticModelAsync().ConfigureAwait(true);

                    return await CoreGenerator.Generate(semanticModel, generationItem.SourceNode, targetSemanticModel, withRegeneration, generationItem.Options, generationItem.NamespaceTransform, isSingleItemGeneration, messageLogger).ConfigureAwait(true);
                }
            }

            return await CoreGenerator.Generate(semanticModel, generationItem.SourceNode, null, withRegeneration, generationItem.Options, generationItem.NamespaceTransform, isSingleItemGeneration, messageLogger).ConfigureAwait(true);
        }
    }
}
