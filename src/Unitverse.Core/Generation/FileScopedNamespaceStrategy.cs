﻿#if VS2022
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
            var replaceableNode = FindTypeNode(Compilation, originalTargetType) ??
                                  FindTypeNode(Compilation, targetType);

            if (replaceableNode != null)
            {
                Compilation = Compilation.ReplaceNode(replaceableNode, targetType);
            }
            else
            {
                Compilation = Compilation.AddMembers(targetType);
            }
        }
    }
}
#endif