#if VS2022
namespace Unitverse.Core.Generation
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Options;

    public class FileScopedNamespaceStrategy : CompilationUnitStrategy
    {
        public FileScopedNamespaceStrategy(SemanticModel sourceModel, IGenerationItem generationItem, DocumentOptionSet? documentOptions, CompilationUnitSyntax targetCompilationUnit, FileScopedNamespaceDeclarationSyntax targetNamespace, FileScopedNamespaceDeclarationSyntax? originalTargetNamespace)
            : base(sourceModel, generationItem, documentOptions, targetCompilationUnit, targetNamespace, originalTargetNamespace)
        {
        }

        public override SyntaxNode TargetRoot => Compilation;

        public override CompilationUnitSyntax RenderCompilationUnit()
        {
            EmitUsingStatements();

            UpdateOriginalTargetNamespace();
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