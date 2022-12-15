namespace Unitverse.Core.Generation
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Options;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;

    public interface ICompilationUnitStrategy
    {
        void AddUsing(UsingDirectiveSyntax usingDirective);

        void AddTypeParameterAliases(ClassModel classModel, IGenerationContext context);

        void AddTypeToTarget(TypeDeclarationSyntax targetType, TypeDeclarationSyntax? originalTargetType);

        CompilationUnitSyntax RenderCompilationUnit();

        SemanticModel SourceModel { get; }

        IGenerationItem GenerationItem { get; }

        DocumentOptionSet? DocumentOptions { get; }

        SyntaxNode TargetRoot { get; }
    }
}
