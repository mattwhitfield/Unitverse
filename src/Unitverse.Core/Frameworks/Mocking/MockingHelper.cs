namespace Unitverse.Core.Frameworks.Mocking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;

    public static class MockingHelper
    {
        public static ExpressionSyntax GetMethodCall(IMethodSymbol dependencyMethodCall, string mockFieldName, Func<ITypeSymbol, IGenerationContext, ExpressionSyntax> getArgument, IGenerationContext context)
        {
            var target = SyntaxFactory.IdentifierName(mockFieldName);

            return GetMethodCall(dependencyMethodCall, target, getArgument, context);
        }

        public static ExpressionSyntax GetMethodCall(IMethodSymbol dependencyMethodCall, ExpressionSyntax target, Func<ITypeSymbol, IGenerationContext, ExpressionSyntax> getArgument, IGenerationContext context)
        {
            SimpleNameSyntax methodReference;
            if (dependencyMethodCall.TypeArguments.Any())
            {
                methodReference = SyntaxFactory.GenericName(SyntaxFactory.Identifier(dependencyMethodCall.Name)).WithTypeArgumentList(TypeArgumentList(dependencyMethodCall.TypeArguments, context));
            }
            else
            {
                methodReference = SyntaxFactory.IdentifierName(dependencyMethodCall.Name);
            }

            return SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        target,
                        methodReference))
                .WithArgumentList(Generate.Arguments(dependencyMethodCall.Parameters.Select(x => getArgument(x.Type, context))));
        }

        public static TypeArgumentListSyntax TypeArgumentList(IEnumerable<ITypeSymbol> typeSymbols, IGenerationContext context)
        {
            return SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList<TypeSyntax>(typeSymbols.Select(x => x.ToTypeSyntax(context))));
        }

        public static ITypeSymbol ReduceAsyncReturnType(ITypeSymbol returnType)
        {
            if (returnType is INamedTypeSymbol namedType && namedType.Name == "Task" && namedType.ContainingNamespace.ToDisplayString() == "System.Threading.Tasks")
            {
                if (namedType.TypeArguments.Length > 0)
                {
                    return namedType.TypeArguments[0];
                }
            }

            return returnType;
        }
    }
}
