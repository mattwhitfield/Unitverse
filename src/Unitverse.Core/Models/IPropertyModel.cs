namespace Unitverse.Core.Models
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public interface IPropertyModel : ITestableModel<PropertyDeclarationSyntax>
    {
        bool HasGet { get; }

        bool HasSet { get; }

        bool HasInit { get; }

        IPropertySymbol? Symbol { get; }

        TypeInfo TypeInfo { get; }

        bool IsStatic { get; }

        ExpressionSyntax Access(ExpressionSyntax target);
    }
}