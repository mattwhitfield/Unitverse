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

#if VS2022
            // if there is an existing model, pick the strategy that matches that model (i.e. block scoped if there are any block scoped name spaces)
            ICompilationUnitStrategy strategy;

            bool blockScopedNamespaceExists = false;
            if (targetModel != null)
            {
                var targetTree = await targetModel.SyntaxTree.GetRootAsync();
                if (targetTree != null)
                {
                    blockScopedNamespaceExists = targetTree.DescendantNodes().OfType<NamespaceDeclarationSyntax>().Any();
                }
            }

            if (!generationItem.Options.GenerationOptions.GenerateFileScopedNamespaces || blockScopedNamespaceExists)
            {
                strategy = new BlockScopedNamespaceStrategy(sourceModel, targetModel, generationItem, solution, documentOptions, sourceNamespace, targetNamespace);
            }
            else
            {
                strategy = new FileScopedNamespaceStrategy(sourceModel, targetModel, generationItem, solution, documentOptions, sourceNamespace, targetNamespace);
            }

#else
            var strategy = new BlockScopedNamespaceStrategy(sourceModel, targetModel, generationItem, solution, documentOptions, sourceNamespace, targetNamespace);
#endif

            await strategy.Initialize();

            return strategy;
        }
    }
}
