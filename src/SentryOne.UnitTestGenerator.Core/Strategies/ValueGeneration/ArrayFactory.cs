namespace SentryOne.UnitTestGenerator.Core.Strategies.ValueGeneration
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Helpers;

    public static class ArrayFactory
    {
        public static ExpressionSyntax ImplicitlyTypedArray(ITypeSymbol typeSymbol, SemanticModel model, IFrameworkSet frameworkSet)
        {
            if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
            {
                return SyntaxFactory.ImplicitArrayCreationExpression(
                    SyntaxFactory.InitializerExpression(
                        SyntaxKind.ArrayInitializerExpression,
                        SyntaxFactory.SeparatedList<ExpressionSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                AssignmentValueHelper.GetDefaultAssignmentValue(arrayTypeSymbol.ElementType, model, frameworkSet),
                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                AssignmentValueHelper.GetDefaultAssignmentValue(arrayTypeSymbol.ElementType, model, frameworkSet),
                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                AssignmentValueHelper.GetDefaultAssignmentValue(arrayTypeSymbol.ElementType, model, frameworkSet),
                            })));
            }

            return AssignmentValueHelper.GetDefaultAssignmentValue(typeSymbol, model, frameworkSet);
        }

        public static ExpressionSyntax ImplicitlyTyped(ITypeSymbol typeSymbol, SemanticModel model, IFrameworkSet frameworkSet)
        {
            if (typeSymbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.TypeArguments.Length > 0)
            {
                var targetType = namedTypeSymbol.TypeArguments[0];

                return SyntaxFactory.ImplicitArrayCreationExpression(
                    SyntaxFactory.InitializerExpression(
                        SyntaxKind.ArrayInitializerExpression,
                        SyntaxFactory.SeparatedList<ExpressionSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                AssignmentValueHelper.GetDefaultAssignmentValue(targetType, model, frameworkSet),
                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                AssignmentValueHelper.GetDefaultAssignmentValue(targetType, model, frameworkSet),
                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                AssignmentValueHelper.GetDefaultAssignmentValue(targetType, model, frameworkSet),
                            })));
            }

            return AssignmentValueHelper.GetDefaultAssignmentValue(typeSymbol, model, frameworkSet);
        }

        public static ExpressionSyntax Byte()
        {
            return SyntaxFactory.ArrayCreationExpression(
                    SyntaxFactory.ArrayType(
                            SyntaxFactory.PredefinedType(
                                SyntaxFactory.Token(SyntaxKind.ByteKeyword)))
                        .WithRankSpecifiers(
                            SyntaxFactory.SingletonList(
                                SyntaxFactory.ArrayRankSpecifier(
                                    SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                                        SyntaxFactory.OmittedArraySizeExpression())))))
                .WithInitializer(
                    SyntaxFactory.InitializerExpression(
                        SyntaxKind.ArrayInitializerExpression,
                        SyntaxFactory.SeparatedList<ExpressionSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                Generate.Literal((byte)ValueGenerationStrategyFactory.Random.Next(255)),
                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                Generate.Literal((byte)ValueGenerationStrategyFactory.Random.Next(255)),
                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                Generate.Literal((byte)ValueGenerationStrategyFactory.Random.Next(255)),
                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                Generate.Literal((byte)ValueGenerationStrategyFactory.Random.Next(255)),
                            })));
        }
    }
}