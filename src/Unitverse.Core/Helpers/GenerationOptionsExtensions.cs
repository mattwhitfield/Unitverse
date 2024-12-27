namespace Unitverse.Core.Helpers
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Options;

    internal static class GenerationOptionsExtensions
    {
        public static bool CanUseAutoFixtureForMocking(this IGenerationOptions options)
        {
            return options.UseAutoFixture && options.UseAutoFixtureForMocking && options.MockingFrameworkType != MockingFrameworkType.JustMock;
        }

        public static bool ShouldUseSeparateChecksForNullAndEmpty(this IGenerationOptions options, SyntaxNode method)
        {
            if (options.UseSeparateChecksForNullAndEmpty)
            {
                return true;
            }

            var invocationExpressions = method.DescendantNodes().OfType<InvocationExpressionSyntax>();
            foreach (var expression in invocationExpressions)
            {
                if (expression.Expression is MemberAccessExpressionSyntax memberAccessExpression)
                {
                    if (memberAccessExpression.Name.Identifier.ValueText == "ThrowIfNullOrWhiteSpace")
                    {
                        if (memberAccessExpression.Expression.DescendantNodesAndSelf().OfType<SimpleNameSyntax>().Any(name => name.Identifier.ValueText == "ArgumentException"))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
