namespace Unitverse.Core.Generation
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Options;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;

    internal interface ICompilationUnitStrategy
    {
        Task Initialize();

        void AddUsing(UsingDirectiveSyntax usingDirective);

        void AddTypeParameterAliases(ClassModel classModel, IGenerationContext context);

        void AddTypeToTarget(TypeDeclarationSyntax targetType, TypeDeclarationSyntax? originalTargetType);

        CompilationUnitSyntax RenderCompilationUnit();

        SemanticModel SourceModel { get; }

        SemanticModel? TargetModel { get; }

        IGenerationItem GenerationItem { get; }

        Solution? Solution { get; }

        DocumentOptionSet? DocumentOptions { get; }

        string SourceNamespaceName { get; }

        string TargetNamespaceName { get; }

        SyntaxNode TargetRoot { get; }
    }
}
