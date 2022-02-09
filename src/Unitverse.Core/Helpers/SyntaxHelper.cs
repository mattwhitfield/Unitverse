namespace Unitverse.Core.Helpers
{
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class SyntaxHelper
    {
        public static LocalDeclarationStatementSyntax AsLocalVariableDeclarationStatementSyntax(
            this VariableDeclarationSyntax variableDeclarationSyntax)
        {
            return SyntaxFactory.LocalDeclarationStatement(variableDeclarationSyntax);
        }

        public static VariableDeclarationSyntax CreateVariableDeclaration(
            TypeSyntax typeSyntax,
            string variableIdentifier,
            ExpressionSyntax initialValue)
        {
            return SyntaxFactory.VariableDeclaration(typeSyntax)
                .WithVariables(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.VariableDeclarator(
                                SyntaxFactory.Identifier(variableIdentifier))
                            .WithInitializer(
                                SyntaxFactory.EqualsValueClause(initialValue))));
        }
    }
}