namespace Unitverse.Core.Helpers
{
    using System;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Options;

    public static class GenerationOptionsHelper
    {
        public static ExpressionSyntax QualifyFieldReference(this IGenerationOptions options, SimpleNameSyntax nameSyntax)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (nameSyntax is null)
            {
                throw new ArgumentNullException(nameof(nameSyntax));
            }

            if (options.PrefixFieldReferencesWithThis)
            {
                return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.ThisExpression(), nameSyntax);
            }

            return nameSyntax;
        }
    }
}
