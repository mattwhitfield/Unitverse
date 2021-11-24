namespace Unitverse.Core.Models
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public interface IPropertyModel : ITestableModel<PropertyDeclarationSyntax>
    {
        bool HasGet { get; }

        bool HasSet { get; }

        TypeInfo TypeInfo { get; }

        bool IsStatic { get; }

        ExpressionSyntax Access(ExpressionSyntax target);
    }
}