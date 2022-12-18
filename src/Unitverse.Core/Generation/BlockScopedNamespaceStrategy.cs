﻿namespace Unitverse.Core.Generation
{
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
            var replaceableNode = FindTypeNode(TargetNamespace, originalTargetType) ??
                                  FindTypeNode(TargetNamespace, targetType);

            if (replaceableNode != null)
            {
                TargetNamespace = TargetNamespace.ReplaceNode(replaceableNode, targetType);
            }
            else
            {
                TargetNamespace = TargetNamespace.AddMembers(targetType);
            }
        }
    }
}