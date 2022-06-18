namespace Unitverse.Core.Helpers
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Options;

    internal static class AutoFixtureHelper
    {
        internal static StatementSyntax VariableDeclaration(IGenerationOptions options)
        {
            var creationExpression = CreationExpression;
            if (options.UseAutoFixtureForMocking)
            {
                ExpressionSyntax customization = null;
                switch (options.MockingFrameworkType)
                {
                    case MockingFrameworkType.NSubstitute:
                        customization = Generate.ObjectCreation(SyntaxFactory.IdentifierName("AutoNSubstituteCustomization"));
                        break;
                    case MockingFrameworkType.Moq:
                    case MockingFrameworkType.MoqAutoMock:
                        customization = Generate.ObjectCreation(SyntaxFactory.IdentifierName("AutoMoqCustomization"));
                        break;
                    case MockingFrameworkType.FakeItEasy:
                        customization = Generate.ObjectCreation(SyntaxFactory.IdentifierName("AutoFakeItEasyCustomization"));
                        break;
                }

                if (customization != null)
                {
                    creationExpression = Generate.MethodCall(creationExpression, SyntaxFactory.IdentifierName("Customize"), customization);
                }
            }

            return SyntaxFactory.LocalDeclarationStatement(SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("var")).AddVariables(SyntaxFactory.VariableDeclarator("fixture").WithInitializer(SyntaxFactory.EqualsValueClause(creationExpression))));
        }

        internal static IdentifierNameSyntax VariableReference => SyntaxFactory.IdentifierName("fixture");

        internal static TypeSyntax TypeSyntax => SyntaxFactory.IdentifierName("Fixture");

        internal static ExpressionSyntax CreationExpression => Generate.ObjectCreation(TypeSyntax);

        internal static ExpressionSyntax Create(ITypeSymbol typeSymbol, IGenerationContext context)
        {
            if (typeSymbol is null)
            {
                throw new ArgumentNullException(nameof(typeSymbol));
            }

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return GetValue(typeSymbol.ToTypeSyntax(context), context, "Create");
        }

        internal static ExpressionSyntax Create(TypeSyntax typeSyntax, IGenerationContext context)
        {
            if (typeSyntax is null)
            {
                throw new ArgumentNullException(nameof(typeSyntax));
            }

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return GetValue(typeSyntax, context, "Create");
        }

        internal static ExpressionSyntax Freeze(TypeSyntax typeSyntax, IGenerationContext context)
        {
            if (typeSyntax is null)
            {
                throw new ArgumentNullException(nameof(typeSyntax));
            }

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return GetValue(typeSyntax, context, "Freeze");
        }

        private static ExpressionSyntax GetValue(TypeSyntax typeSyntax, IGenerationContext context, string methodName)
        {
            context.CurrentMethod?.AddRequirement(Requirements.AutoFixture);
            var method = SyntaxFactory.GenericName(SyntaxFactory.Identifier(methodName)).WithTypeArgumentList(typeSyntax.AsList());
            return SyntaxFactory.InvocationExpression(Generate.MemberAccess(VariableReference, method));
        }
    }
}
