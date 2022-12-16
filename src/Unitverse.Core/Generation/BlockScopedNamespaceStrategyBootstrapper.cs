namespace Unitverse.Core.Generation
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Options;

    public class BlockScopedNamespaceStrategyBootstrapper : CompilationUnitStrategyBootstrapper
    {
        public BlockScopedNamespaceStrategyBootstrapper(SemanticModel sourceModel, SemanticModel? targetModel, IGenerationItem generationItem, DocumentOptionSet? documentOptions, string targetNamespaceName)
            : base(sourceModel, targetModel, generationItem, documentOptions, targetNamespaceName)
        {
        }

        protected override ICompilationUnitStrategy InitializeInternal(SyntaxNode? targetTree)
        {
            CompilationUnitSyntax? compilation = null;
            NamespaceDeclarationSyntax? targetNamespace = null;
            NamespaceDeclarationSyntax? originalTargetNamespace = null;

            if (targetTree != null)
            {
                compilation = targetTree.AncestorsAndSelf().OfType<CompilationUnitSyntax>().FirstOrDefault();
                originalTargetNamespace = targetNamespace = targetTree.DescendantNodesAndSelf().OfType<NamespaceDeclarationSyntax>().FirstOrDefault(x => string.Equals(x.Name.ToString(), TargetNamespaceName, StringComparison.OrdinalIgnoreCase)) ?? targetTree.DescendantNodesAndSelf().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
            }

            CompilationUnitSyntax targetCompilation = compilation ?? SyntaxFactory.CompilationUnit();
            NamespaceDeclarationSyntax resolvedTargetNamespace = targetNamespace ?? SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(TargetNamespaceName));

            return new BlockScopedNamespaceStrategy(SourceModel, targetTree, GenerationItem, DocumentOptions, targetCompilation, resolvedTargetNamespace, originalTargetNamespace);
        }
    }
}
