#if VS2022
namespace Unitverse.Core.Generation
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Options;

    public class FileScopedNamespaceStrategyBootstrapper : CompilationUnitStrategyBootstrapper
    {
        public FileScopedNamespaceStrategyBootstrapper(SemanticModel sourceModel, SemanticModel? targetModel, IGenerationItem generationItem, DocumentOptionSet? documentOptions, string targetNamespaceName)
            : base(sourceModel, targetModel, generationItem, documentOptions, targetNamespaceName)
        {
        }

        protected override ICompilationUnitStrategy InitializeInternal(SyntaxNode? targetTree)
        {
            CompilationUnitSyntax? compilation = null;
            FileScopedNamespaceDeclarationSyntax? targetNamespace = null;
            FileScopedNamespaceDeclarationSyntax? originalTargetNamespace = null;

            if (targetTree != null)
            {
                compilation = targetTree.AncestorsAndSelf().OfType<CompilationUnitSyntax>().FirstOrDefault();
                originalTargetNamespace = targetNamespace = targetTree.DescendantNodesAndSelf().OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault(x => string.Equals(x.Name.ToString(), TargetNamespaceName, StringComparison.OrdinalIgnoreCase)) ?? targetTree.DescendantNodesAndSelf().OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault();
            }

            CompilationUnitSyntax targetCompilation = compilation ?? SyntaxFactory.CompilationUnit();
            FileScopedNamespaceDeclarationSyntax resolvedTargetNamespace = targetNamespace ?? SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.IdentifierName(TargetNamespaceName));

            return new FileScopedNamespaceStrategy(SourceModel, targetTree, GenerationItem, DocumentOptions, targetCompilation, resolvedTargetNamespace, originalTargetNamespace);
        }
    }
}
#endif