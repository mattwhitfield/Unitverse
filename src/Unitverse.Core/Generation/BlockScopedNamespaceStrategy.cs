namespace Unitverse.Core.Generation
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Options;

    public class BlockScopedNamespaceStrategy : CompilationUnitStrategy
    {
        public BlockScopedNamespaceStrategy(SemanticModel sourceModel, SyntaxNode? targetNode, IGenerationItem generationItem, DocumentOptionSet? documentOptions, CompilationUnitSyntax targetCompilationUnit, NamespaceDeclarationSyntax targetNamespace, NamespaceDeclarationSyntax? originalTargetNamespace)
            : base(sourceModel, targetNode, generationItem, documentOptions, targetCompilationUnit, targetNamespace, originalTargetNamespace)
        {
        }

        public override SyntaxNode TargetRoot => TargetNamespace;

        public override void AddTypeToTarget(TypeDeclarationSyntax targetType, TypeDeclarationSyntax? originalTargetType)
        {
            if (originalTargetType == null)
            {
                originalTargetType = TargetNamespace.DescendantNodes().OfType<TypeDeclarationSyntax>().FirstOrDefault(x => x.Identifier.ValueText == targetType.Identifier.ValueText);
            }

            if (originalTargetType != null)
            {
                var newTargetNamespace = TargetNamespace.RemoveNode(originalTargetType, SyntaxRemoveOptions.KeepNoTrivia);
                TargetNamespace = newTargetNamespace ?? TargetNamespace;
            }

            TargetNamespace = TargetNamespace.AddMembers(targetType);
        }

        public override CompilationUnitSyntax RenderCompilationUnit()
        {
            EmitUsingStatements();

            if (OriginalTargetNamespace != null)
            {
                return Compilation.ReplaceNode(OriginalTargetNamespace, TargetNamespace);
            }

            return Compilation.AddMembers(TargetNamespace);
        }
    }
}
