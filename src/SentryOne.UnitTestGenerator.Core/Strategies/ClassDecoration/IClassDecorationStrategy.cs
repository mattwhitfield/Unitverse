namespace Unitverse.Core.Strategies.ClassDecoration
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Models;

    internal interface IClassDecorationStrategy
    {
        bool IsExclusive { get; }

        int Priority { get; }

        TypeDeclarationSyntax Apply(TypeDeclarationSyntax declaration, ClassModel model);

        bool CanHandle(TypeDeclarationSyntax existingSyntax, ClassModel model);
    }
}