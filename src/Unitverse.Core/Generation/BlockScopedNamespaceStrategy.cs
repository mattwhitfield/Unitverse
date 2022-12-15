namespace Unitverse.Core.Generation
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Options;

    public class BlockScopedNamespaceStrategy : CompilationUnitStrategy
    {
        public BlockScopedNamespaceStrategy(SemanticModel sourceModel, SemanticModel? targetModel, IGenerationItem generationItem, Solution? solution, DocumentOptionSet? documentOptions, string sourceNamespaceName, string targetNamespaceName)
            : base(sourceModel, targetModel, generationItem, solution, documentOptions, sourceNamespaceName, targetNamespaceName)
        {
        }

        private NamespaceDeclarationSyntax? _targetNamespace;

        private CompilationUnitSyntax? _compilation;

        private NamespaceDeclarationSyntax? _originalTargetNamespace;

        public override SyntaxNode TargetRoot => _targetNamespace ?? throw new InvalidOperationException("Call initialize");

        protected override void InitializeInternal(SyntaxNode? targetTree)
        {
            CompilationUnitSyntax? compilation = null;
            NamespaceDeclarationSyntax? targetNamespace = null;

            if (targetTree != null)
            {
                compilation = targetTree.AncestorsAndSelf().OfType<CompilationUnitSyntax>().FirstOrDefault() ?? SyntaxFactory.CompilationUnit();
                _originalTargetNamespace = targetNamespace = targetTree.DescendantNodesAndSelf().OfType<NamespaceDeclarationSyntax>().FirstOrDefault(x => string.Equals(x.Name.ToString(), TargetNamespaceName, StringComparison.OrdinalIgnoreCase)) ?? targetTree.DescendantNodesAndSelf().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
            }

            _compilation = compilation ?? SyntaxFactory.CompilationUnit();
            _targetNamespace = targetNamespace ?? SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(TargetNamespaceName));
        }

        public override void AddTypeToTarget(TypeDeclarationSyntax targetType, TypeDeclarationSyntax? originalTargetType)
        {
            if (_targetNamespace == null)
            {
                throw new InvalidOperationException("Call initialize");
            }

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
            if (_targetNamespace == null || _compilation == null)
            {
                throw new InvalidOperationException("Call initialize");
            }

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
            if (_targetNamespace == null || _compilation == null)
            {
                throw new InvalidOperationException("Call initialize");
            }

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
