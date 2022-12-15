#if VS2022
namespace Unitverse.Core.Generation
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Options;

    public class FileScopedNamespaceStrategy : CompilationUnitStrategy
    {
        public FileScopedNamespaceStrategy(SemanticModel sourceModel, SemanticModel? targetModel, IGenerationItem generationItem, Solution? solution, DocumentOptionSet? documentOptions, string sourceNamespaceName, string targetNamespaceName)
            : base(sourceModel, targetModel, generationItem, solution, documentOptions, sourceNamespaceName, targetNamespaceName)
        {
        }

        private CompilationUnitSyntax? _compilation;

        public override SyntaxNode TargetRoot => _compilation ?? throw new InvalidOperationException("Call initialize");

        protected override void InitializeInternal(SyntaxNode? targetTree)
        {
            CompilationUnitSyntax? compilation = null;

            if (targetTree != null)
            {
                compilation = targetTree.AncestorsAndSelf().OfType<CompilationUnitSyntax>().FirstOrDefault() ?? SyntaxFactory.CompilationUnit();
            }

            _compilation = compilation ?? SyntaxFactory.CompilationUnit();

            if (!_compilation.DescendantNodesAndSelf().OfType<FileScopedNamespaceDeclarationSyntax>().Any())
            {
                _compilation = _compilation.AddMembers(SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.IdentifierName(TargetNamespaceName)));
            }
        }

        public override void AddTypeToTarget(TypeDeclarationSyntax targetType, TypeDeclarationSyntax? originalTargetType)
        {
            if (_compilation == null)
            {
                throw new InvalidOperationException("Call initialize");
            }

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
            if (_compilation == null)
            {
                throw new InvalidOperationException("Call initialize");
            }

            _compilation = _compilation.AddUsings(usingDirective);
        }

        public override CompilationUnitSyntax RenderCompilationUnit()
        {
            if (_compilation == null)
            {
                throw new InvalidOperationException("Call initialize");
            }

            EmitUsingStatements();

            return _compilation;
        }
    }
}
#endif