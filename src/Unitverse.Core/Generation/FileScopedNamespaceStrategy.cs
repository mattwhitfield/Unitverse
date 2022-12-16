#if VS2022
namespace Unitverse.Core.Generation
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Options;

    public class FileScopedNamespaceStrategy : CompilationUnitStrategy
    {
        public FileScopedNamespaceStrategy(SemanticModel sourceModel, SyntaxNode? targetTree, IGenerationItem generationItem, DocumentOptionSet? documentOptions, CompilationUnitSyntax targetCompilationUnit, FileScopedNamespaceDeclarationSyntax targetNamespace, FileScopedNamespaceDeclarationSyntax? originalTargetNamespace)
            : base(sourceModel, targetTree, generationItem, documentOptions, targetCompilationUnit, targetNamespace, originalTargetNamespace)
        {
        }

        public override SyntaxNode TargetRoot => Compilation;

        public override void AddTypeToTarget(TypeDeclarationSyntax targetType, TypeDeclarationSyntax? originalTargetType)
        {
            if (originalTargetType == null)
            {
                originalTargetType = Compilation.DescendantNodes().OfType<TypeDeclarationSyntax>().FirstOrDefault(x => x.Identifier.ValueText == targetType.Identifier.ValueText);
            }

            if (originalTargetType != null)
            {
                var newCompilation = Compilation.RemoveNode(originalTargetType, SyntaxRemoveOptions.KeepNoTrivia);
                Compilation = newCompilation ?? Compilation;
            }

            Compilation = Compilation.AddMembers(targetType);
        }

        public override CompilationUnitSyntax RenderCompilationUnit()
        {
            EmitUsingStatements();

            if (OriginalTargetNamespace != null)
            {
                return Compilation.ReplaceNode(OriginalTargetNamespace, TargetNamespace);
            }

            var typeDeclaration = Compilation.ChildNodes().OfType<TypeDeclarationSyntax>().FirstOrDefault();
            if (typeDeclaration != null)
            {
                return Compilation.InsertNodesBefore(typeDeclaration, new[] { TargetNamespace });
            }
            else
            {
                return Compilation.AddMembers(TargetNamespace);
            }
        }
    }
}
#endif