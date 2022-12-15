#if VS2022
namespace Unitverse.Core.Generation
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Options;

    public class FileScopedNamespaceStrategy : CompilationUnitStrategy
    {
        public FileScopedNamespaceStrategy(SemanticModel sourceModel, SyntaxNode? targetTree, IGenerationItem generationItem, DocumentOptionSet? documentOptions, CompilationUnitSyntax targetCompilationUnit)
            : base(sourceModel, targetTree, generationItem, documentOptions)
        {
            _compilation = targetCompilationUnit;
        }

        private CompilationUnitSyntax _compilation;

        public override SyntaxNode TargetRoot => _compilation;

        public override void AddTypeToTarget(TypeDeclarationSyntax targetType, TypeDeclarationSyntax? originalTargetType)
        {
            if (originalTargetType == null)
            {
                originalTargetType = _compilation.DescendantNodes().OfType<TypeDeclarationSyntax>().FirstOrDefault(x => x.Identifier.ValueText == targetType.Identifier.ValueText);
            }

            if (originalTargetType != null)
            {
                var newCompilation = _compilation.RemoveNode(originalTargetType, SyntaxRemoveOptions.KeepNoTrivia);
                _compilation = newCompilation ?? _compilation;
            }

            _compilation = _compilation.AddMembers(targetType);
        }

        protected override void AddUsingToTarget(UsingDirectiveSyntax usingDirective)
        {
            _compilation = _compilation.AddUsings(usingDirective);
        }

        public override CompilationUnitSyntax RenderCompilationUnit()
        {
            EmitUsingStatements();

            return _compilation;
        }
    }
}
#endif