namespace Unitverse.Core.Strategies.ClassGeneration
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Models;

    public interface IClassGenerationStrategy
    {
        int Priority { get; }

        bool CanHandle(ClassModel model);

        ClassDeclarationSyntax Create(ClassModel model);
    }
}