namespace Unitverse.Core.Frameworks.Assertion
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;

    public class ShouldlyAssertionFramework : IAssertionFramework
    {
        private readonly IAssertionFramework _baseFramework;

        public bool AssertThrowsAsyncIsAwaitable => true;

        public bool SkipValueTypeNotNull => true;

        public ShouldlyAssertionFramework(IAssertionFramework baseFramework)
        {
            _baseFramework = baseFramework ?? throw new ArgumentNullException(nameof(baseFramework));
        }

        private static ExpressionSyntax Should(ExpressionSyntax actual)
        {
            if (actual is BinaryExpressionSyntax)
            {
                return SyntaxFactory.ParenthesizedExpression(actual);
            }

            return actual;
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
            return Generate.Statement(Should(actual, isReferenceType ? "ShouldBeSameAs" : "ShouldBe", expected));
        }

        public StatementSyntax AssertNotEqual(ExpressionSyntax actual, ExpressionSyntax expected, bool isReferenceType)
        {
            return Generate.Statement(Should(actual, isReferenceType ? "ShouldNotBeSameAs" : "ShouldNotBe", expected));
        }

        public StatementSyntax AssertFail(string message)
        {
            return _baseFramework.AssertFail(message);
        }

        public StatementSyntax AssertGreaterThan(ExpressionSyntax actual, ExpressionSyntax expected)
        {
            return Generate.Statement(Should(actual, "ShouldBeGreaterThan", expected));
        }

        public StatementSyntax AssertTrue(ExpressionSyntax actual)
        {
            return Generate.Statement(Should(actual, "ShouldBeTrue"));
        }

        public StatementSyntax AssertFalse(ExpressionSyntax actual)
        {
            return Generate.Statement(Should(actual, "ShouldBeFalse"));
        }

        public StatementSyntax AssertIsInstanceOf(ExpressionSyntax value, TypeSyntax type, bool isReferenceType)
        {
            return Generate.Statement(Generate.MemberInvocation(Should(value), Generate.GenericName("ShouldBeOfType", type)));
        }

        public StatementSyntax AssertLessThan(ExpressionSyntax actual, ExpressionSyntax expected)
        {
            return Generate.Statement(Should(actual, "ShouldBeLessThan", expected));
        }

        public StatementSyntax AssertNotNull(ExpressionSyntax value)
        {
            return Generate.Statement(Should(value, "ShouldNotBeNull"));
        }

        public StatementSyntax AssertThrows(TypeSyntax exceptionType, ExpressionSyntax methodCall, string? associatedParameterName)
        {
            return Generate.Statement(AssertThrowsCore(exceptionType, methodCall, "Throw"));
        }

        public StatementSyntax AssertThrowsAsync(TypeSyntax exceptionType, ExpressionSyntax methodCall, string? associatedParameterName)
        {
            return Generate.Statement(SyntaxFactory.AwaitExpression(AssertThrowsCore(exceptionType, methodCall, "ThrowAsync")));
        }

        private static ExpressionSyntax AssertThrowsCore(TypeSyntax exceptionType, ExpressionSyntax methodCall, string methodName)
        {
            return Generate.MemberInvocation("Should", Generate.GenericName(methodName, exceptionType), Generate.ParenthesizedLambdaExpression(methodCall));
        }

        public IEnumerable<UsingDirectiveSyntax> GetUsings()
        {
            yield return Generate.UsingDirective("Shouldly");
        }
    }
}
