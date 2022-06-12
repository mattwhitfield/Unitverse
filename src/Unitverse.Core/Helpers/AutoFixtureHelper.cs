namespace Unitverse.Core.Helpers
{
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class AutoFixtureHelper
    {
        internal static StatementSyntax VariableDeclaration => SyntaxFactory.LocalDeclarationStatement(SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("var")).AddVariables(SyntaxFactory.VariableDeclarator("fixture").WithInitializer(SyntaxFactory.EqualsValueClause(CreationExpression))));

        internal static IdentifierNameSyntax VariableReference => SyntaxFactory.IdentifierName("fixture");

        internal static TypeSyntax TypeSyntax => SyntaxFactory.IdentifierName("Fixture");

        internal static ExpressionSyntax CreationExpression => Generate.ObjectCreation(TypeSyntax);
    }
}
