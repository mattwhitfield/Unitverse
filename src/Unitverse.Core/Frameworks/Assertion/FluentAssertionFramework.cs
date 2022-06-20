namespace Unitverse.Core.Frameworks.Assertion
{
    using System;
    using System.Collections.Generic;
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
            if (actual is BinaryExpressionSyntax)
            {
                actual = SyntaxFactory.ParenthesizedExpression(actual);
            }

            return Generate.MemberInvocation(actual, "Should");
        }

        private static ExpressionSyntax Should(ExpressionSyntax actual, string methodName, ExpressionSyntax? expected = null)
        {
            var invocation = Generate.MemberInvocation(Should(actual), methodName);

            if (expected != null)
            {
                return invocation.WithArgs(expected);
            }

            return invocation;
        }

        public StatementSyntax AssertEqual(ExpressionSyntax actual, ExpressionSyntax expected, bool isReferenceType)
        {
            return Generate.Statement(Should(actual, isReferenceType ? "BeSameAs" : "Be", expected));
        }

        public StatementSyntax AssertNotEqual(ExpressionSyntax actual, ExpressionSyntax expected, bool isReferenceType)
        {
            return Generate.Statement(Should(actual, isReferenceType ? "NotBeSameAs" : "NotBe", expected));
        }

        public StatementSyntax AssertFail(string message)
        {
            return _baseFramework.AssertFail(message);
        }

        public StatementSyntax AssertGreaterThan(ExpressionSyntax actual, ExpressionSyntax expected)
        {
            return Generate.Statement(Should(actual, "BeGreaterThan", expected));
        }

        public StatementSyntax AssertTrue(ExpressionSyntax actual)
        {
            return Generate.Statement(Should(actual, "BeTrue"));
        }

        public StatementSyntax AssertFalse(ExpressionSyntax actual)
        {
            return Generate.Statement(Should(actual, "BeFalse"));
        }

        public StatementSyntax AssertIsInstanceOf(ExpressionSyntax value, TypeSyntax type, bool isReferenceType)
        {
            ExpressionSyntax resolvedValue = value;
            if (!isReferenceType)
            {
                resolvedValue = Generate.MemberInvocation(value, Generate.GenericName("As", SyntaxKind.ObjectKeyword));
            }

            return Generate.Statement(Generate.MemberInvocation(Should(resolvedValue), Generate.GenericName("BeAssignableTo", type)));
        }

        public StatementSyntax AssertLessThan(ExpressionSyntax actual, ExpressionSyntax expected)
        {
            return Generate.Statement(Should(actual, "BeLessThan", expected));
        }

        public StatementSyntax AssertNotNull(ExpressionSyntax value)
        {
            return Generate.Statement(Should(value, "NotBeNull"));
        }

        public StatementSyntax AssertThrows(TypeSyntax exceptionType, ExpressionSyntax methodCall)
        {
            return Generate.Statement(AssertThrows(exceptionType, methodCall, "Throw"));
        }

        public StatementSyntax AssertThrowsAsync(TypeSyntax exceptionType, ExpressionSyntax methodCall)
        {
            return Generate.Statement(SyntaxFactory.AwaitExpression(AssertThrows(exceptionType, methodCall, "ThrowAsync")));
        }

        private ExpressionSyntax AssertThrows(TypeSyntax exceptionType, ExpressionSyntax methodCall, string methodName)
        {
            return Generate.MemberInvocation(
                        Generate.MemberInvocation(
                            Generate.MemberInvocation("FluentActions", "Invoking", Generate.ParenthesizedLambdaExpression(methodCall)),
                            "Should"),
                        Generate.GenericName(methodName, exceptionType));
        }

        public IEnumerable<UsingDirectiveSyntax> GetUsings()
        {
            yield return Generate.UsingDirective("FluentAssertions");
        }
    }
}
