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
            string variableIdentifier,
            ExpressionSyntax initialValue)
        {
            return CreateVariableDeclaration(SyntaxFactory.IdentifierName("var"), variableIdentifier, initialValue);
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

        public static InvocationExpressionSyntax CreateInvocationStatement(IdentifierNameSyntax identifierNameSyntax, IdentifierNameSyntax identifierName, IdentifierNameSyntax nameSyntax)
        {
            return SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        identifierNameSyntax,
                        identifierName))
                .WithArgumentList(
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                            SyntaxFactory.Argument(
                                nameSyntax))));
        }
    }
}