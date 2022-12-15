namespace Unitverse.Core.Generation
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Options;

    public class BlockScopedNamespaceStrategy : CompilationUnitStrategy
    {
        public BlockScopedNamespaceStrategy(SemanticModel sourceModel, SyntaxNode? targetNode, IGenerationItem generationItem, DocumentOptionSet? documentOptions, CompilationUnitSyntax targetCompilationUnit, NamespaceDeclarationSyntax targetNamespace, NamespaceDeclarationSyntax? originalTargetNamespace)
            : base(sourceModel, targetNode, generationItem, documentOptions)
        {
            _targetNamespace = targetNamespace;
            _compilation = targetCompilationUnit;
            _originalTargetNamespace = originalTargetNamespace;
        }

        private NamespaceDeclarationSyntax _targetNamespace;

        private CompilationUnitSyntax _compilation;

        private NamespaceDeclarationSyntax? _originalTargetNamespace;

        public override SyntaxNode TargetRoot => _targetNamespace;

        public override void AddTypeToTarget(TypeDeclarationSyntax targetType, TypeDeclarationSyntax? originalTargetType)
        {
            if (originalTargetType == null)
            {
                originalTargetType = _targetNamespace.DescendantNodes().OfType<TypeDeclarationSyntax>().FirstOrDefault(x => x.Identifier.ValueText == targetType.Identifier.ValueText);
            }

            if (originalTargetType != null)
            {
                var newTargetNamespace = _targetNamespace.RemoveNode(originalTargetType, SyntaxRemoveOptions.KeepNoTrivia);
                _targetNamespace = newTargetNamespace ?? _targetNamespace;
            }

            _targetNamespace = _targetNamespace.AddMembers(targetType);
        }

        protected override void AddUsingToTarget(UsingDirectiveSyntax usingDirective)
        {
            if (GenerationItem.Options.GenerationOptions.EmitUsingsOutsideNamespace)
            {
                _compilation = _compilation.AddUsings(usingDirective);
            }
            else
            {
                _targetNamespace = _targetNamespace.AddUsings(usingDirective);
            }
        }

        public override CompilationUnitSyntax RenderCompilationUnit()
        {
            EmitUsingStatements();

            if (_originalTargetNamespace != null)
            {
                var newCompilation = _compilation.RemoveNode(_originalTargetNamespace, SyntaxRemoveOptions.KeepNoTrivia);
                _compilation = newCompilation ?? _compilation;
            }

            return _compilation.AddMembers(_targetNamespace);
        }
    }
}
