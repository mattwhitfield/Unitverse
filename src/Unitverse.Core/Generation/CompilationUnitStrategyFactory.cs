namespace Unitverse.Core.Generation
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Options;
    using Unitverse.Core.Helpers;

    internal static class CompilationUnitStrategyFactory
    {
        public static async Task<ICompilationUnitStrategy> CreateAsync(SemanticModel sourceModel, SemanticModel? targetModel, IGenerationItem generationItem, Solution? solution)
        {
            Func<string, string> nameSpaceTransform = generationItem.NamespaceTransform;

            string sourceNamespace = (await sourceModel.GetNamespace()) ?? "Namespace";
            string targetNamespace = nameSpaceTransform(sourceNamespace);

            DocumentOptionSet? documentOptions = null;
            if (solution != null)
            {
                var document = solution.GetDocument(sourceModel.SyntaxTree);
                if (document != null)
                {
                    documentOptions = await document.GetOptionsAsync();
                }
            }

            ICompilationUnitStrategyBootstrapper strategyBootstrapper;

#if VS2022
            // if there is an existing model, pick the strategy that matches that model (i.e. block scoped if there are any block scoped name spaces)
            bool blockScopedNamespaceExists = false, fileScopedNamespaceExists = false;
            if (targetModel != null)
            {
                var targetTree = await targetModel.SyntaxTree.GetRootAsync();
                if (targetTree != null)
                {
                    blockScopedNamespaceExists = targetTree.DescendantNodes().OfType<NamespaceDeclarationSyntax>().Any();
                    fileScopedNamespaceExists = targetTree.DescendantNodes().OfType<FileScopedNamespaceDeclarationSyntax>().Any();
                }
            }

            bool shouldUseBlockScopedNamespace = false;

            if (fileScopedNamespaceExists || blockScopedNamespaceExists)
            {
                shouldUseBlockScopedNamespace = blockScopedNamespaceExists;
            }
            else
            {
                shouldUseBlockScopedNamespace = !generationItem.Options.GenerationOptions.GenerateFileScopedNamespaces;
            }

            if (shouldUseBlockScopedNamespace)
            {
                strategyBootstrapper = new BlockScopedNamespaceStrategyBootstrapper(sourceModel, targetModel, generationItem, documentOptions, targetNamespace);
            }
            else
            {
                strategyBootstrapper = new FileScopedNamespaceStrategyBootstrapper(sourceModel, targetModel, generationItem, documentOptions, targetNamespace);
            }

#else
            strategyBootstrapper = new BlockScopedNamespaceStrategyBootstrapper(sourceModel, targetModel, generationItem, documentOptions, targetNamespace);
#endif

            var strategy = await strategyBootstrapper.Initialize();

            return strategy;
        }
    }
}
