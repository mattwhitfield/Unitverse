#if VS2022
namespace Unitverse.Core.Generation
{
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

            if (targetTree != null)
            {
                compilation = targetTree.AncestorsAndSelf().OfType<CompilationUnitSyntax>().FirstOrDefault() ?? SyntaxFactory.CompilationUnit();
            }

            CompilationUnitSyntax targetCompilation = compilation ?? SyntaxFactory.CompilationUnit();

            if (!targetCompilation.DescendantNodesAndSelf().OfType<FileScopedNamespaceDeclarationSyntax>().Any())
            {
                targetCompilation = targetCompilation.AddMembers(SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.IdentifierName(TargetNamespaceName)));
            }

            return new FileScopedNamespaceStrategy(SourceModel, targetTree, GenerationItem, DocumentOptions, targetCompilation);
        }
    }
}
#endif