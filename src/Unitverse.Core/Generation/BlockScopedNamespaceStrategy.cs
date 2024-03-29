﻿namespace Unitverse.Core.Generation
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Options;

    public class BlockScopedNamespaceStrategy : CompilationUnitStrategy
    {
        public BlockScopedNamespaceStrategy(SemanticModel sourceModel, IGenerationItem generationItem, DocumentOptionSet? documentOptions, CompilationUnitSyntax targetCompilationUnit, NamespaceDeclarationSyntax targetNamespace, NamespaceDeclarationSyntax? originalTargetNamespace)
            : base(sourceModel, generationItem, documentOptions, targetCompilationUnit, targetNamespace, originalTargetNamespace)
        {
        }

        public override SyntaxNode TargetRoot => TargetNamespace;

        public override CompilationUnitSyntax RenderCompilationUnit()
        {
            EmitUsingStatements();

            UpdateOriginalTargetNamespace();
            if (OriginalTargetNamespace != null)
            {
                return Compilation.ReplaceNode(OriginalTargetNamespace, TargetNamespace);
            }

            return Compilation.AddMembers(TargetNamespace);
        }
    }
}
