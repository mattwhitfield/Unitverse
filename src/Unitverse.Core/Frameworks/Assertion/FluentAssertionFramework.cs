namespace Unitverse.Core.Frameworks.Assertion
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;

    public class FluentAssertionFramework : IAssertionFramework
    {
        private readonly IAssertionFramework _baseFramework;

        public bool AssertThrowsAsyncIsAwaitable => true;

        public FluentAssertionFramework(IAssertionFramework baseFramework)
        {
            _baseFramework = baseFramework ?? throw new ArgumentNullException(nameof(baseFramework));
        }

        private static ExpressionSyntax Should(ExpressionSyntax actual)
        {
            return SyntaxFactory.InvocationExpression(
                                                    SyntaxFactory.MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        actual,
                                                        SyntaxFactory.IdentifierName("Should")));
        }

        private static ExpressionSyntax Should(ExpressionSyntax actual, string methodName, ExpressionSyntax expected = null)
        {
            var invocation = SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    Should(actual),
                                    SyntaxFactory.IdentifierName(methodName)));

            if (expected != null)
            {
                return invocation.WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(expected))));
            }

            return invocation;
        }

        public StatementSyntax AssertEqual(ExpressionSyntax actual, ExpressionSyntax expected, bool isReferenceType)
        {
            return SyntaxFactory.ExpressionStatement(Should(actual, isReferenceType ? "BeSameAs" : "Be", expected));
        }

        public StatementSyntax AssertFail(string message)
        {
            return _baseFramework.AssertFail(message);
        }

        public StatementSyntax AssertGreaterThan(ExpressionSyntax actual, ExpressionSyntax expected)
        {
            return SyntaxFactory.ExpressionStatement(Should(actual, "BeGreaterThan", expected));
        }

        public StatementSyntax AssertIsInstanceOf(ExpressionSyntax value, TypeSyntax type, bool isReferenceType)
        {
            ExpressionSyntax resolvedValue = value;
            if (!isReferenceType)
            {
                resolvedValue = SyntaxFactory.InvocationExpression(
                                        SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            value,
                                            SyntaxFactory.GenericName(
                                                SyntaxFactory.Identifier("As"))
                                            .WithTypeArgumentList(
                                                SyntaxFactory.TypeArgumentList(
                                                    SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                        SyntaxFactory.PredefinedType(
                                                            SyntaxFactory.Token(SyntaxKind.ObjectKeyword)))))));
            }

            return SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        Should(resolvedValue),
                        SyntaxFactory.GenericName(
                            SyntaxFactory.Identifier("BeAssignableTo"))
                        .WithTypeArgumentList(
                            SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(type))))));
        }

        public StatementSyntax AssertLessThan(ExpressionSyntax actual, ExpressionSyntax expected)
        {
            return SyntaxFactory.ExpressionStatement(Should(actual, "BeLessThan", expected));
        }

        public StatementSyntax AssertNotNull(ExpressionSyntax value)
        {
            return SyntaxFactory.ExpressionStatement(Should(value, "NotBeNull"));
        }

        public StatementSyntax AssertThrows(TypeSyntax exceptionType, ExpressionSyntax methodCall)
        {
            return SyntaxFactory.ExpressionStatement(AssertThrows(exceptionType, methodCall, "Throw"));
        }

        public StatementSyntax AssertThrowsAsync(TypeSyntax exceptionType, ExpressionSyntax methodCall)
        {
            return SyntaxFactory.ExpressionStatement(SyntaxFactory.AwaitExpression(AssertThrows(exceptionType, methodCall, "ThrowAsync")));
        }

        private ExpressionSyntax AssertThrows(TypeSyntax exceptionType, ExpressionSyntax methodCall, string methodName)
        {
            return SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.InvocationExpression(
                                        SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            SyntaxFactory.IdentifierName("FluentActions"),
                                            SyntaxFactory.IdentifierName("Invoking")))
                                    .WithArgumentList(
                                        Generate.Arguments(Generate.ParenthesizedLambdaExpression(methodCall))),
                                    SyntaxFactory.IdentifierName("Should"))),
                            SyntaxFactory.GenericName(
                                SyntaxFactory.Identifier(methodName))
                            .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(exceptionType)))));
        }

        public IEnumerable<UsingDirectiveSyntax> GetUsings()
        {
            yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("FluentAssertions"));
        }
    }
}
