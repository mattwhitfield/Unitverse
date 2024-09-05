namespace Unitverse.Core.Frameworks.Test
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;

    public class XUnitTestFramework : BaseTestFramework, IExtendedTestFramework
    {
        public XUnitTestFramework(IUnitTestGeneratorOptions options)
            : base(options)
        {
        }

        public override bool SupportsStaticTestClasses => true;

        public bool AssertThrowsAsyncIsAwaitable => true;

        public AttributeSyntax? SingleThreadedApartmentAttribute => null;

        public string TestClassAttribute => string.Empty;

        protected override string TestAttributeName => "Fact";

        protected override string TestCaseMethodAttributeName => "Theory";

        protected override string TestCaseAttributeName => "InlineData";

        public bool SkipValueTypeNotNull => false;

        public override bool SupportsReadonlyFields => true;

        public StatementSyntax AssertEqual(ExpressionSyntax actual, ExpressionSyntax expected, bool isReferenceType)
        {
            if (actual == null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            if (expected == null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            var method = isReferenceType ? "Same" : "Equal";
            return Generate.Statement(AssertCall(method).WithArgs(expected, actual));
        }

        public StatementSyntax AssertNotEqual(ExpressionSyntax actual, ExpressionSyntax expected, bool isReferenceType)
        {
            if (actual == null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            if (expected == null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            var method = isReferenceType ? "NotSame" : "NotEqual";
            return Generate.Statement(AssertCall(method).WithArgs(expected, actual));
        }

        public StatementSyntax AssertFail(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            return SyntaxFactory.ThrowStatement(Generate.ObjectCreation(SyntaxFactory.IdentifierName("NotImplementedException"), Generate.Literal(message)));
        }

        public StatementSyntax AssertTrue(ExpressionSyntax actual)
        {
            if (actual == null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            return Generate.Statement(AssertCall("True").WithArgs(actual));
        }

        public StatementSyntax AssertFalse(ExpressionSyntax actual)
        {
            if (actual == null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            return Generate.Statement(AssertCall("False").WithArgs(actual));
        }

        public StatementSyntax AssertGreaterThan(ExpressionSyntax actual, ExpressionSyntax expected)
        {
            if (actual == null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            if (expected == null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            return Generate.Statement(AssertCall("True").WithArgs(SyntaxFactory.BinaryExpression(SyntaxKind.GreaterThanExpression, actual, expected)));
        }

        public StatementSyntax AssertIsInstanceOf(ExpressionSyntax value, TypeSyntax type, bool isReferenceType)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return Generate.Statement(Generate.MemberInvocation("Assert", Generate.GenericName("IsType", type), value));
        }

        public StatementSyntax AssertLessThan(ExpressionSyntax actual, ExpressionSyntax expected)
        {
            if (actual == null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            if (expected == null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            return Generate.Statement(AssertCall("True").WithArgs(SyntaxFactory.BinaryExpression(SyntaxKind.LessThanExpression, actual, expected)));
        }

        public StatementSyntax AssertNotNull(ExpressionSyntax value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return Generate.Statement(AssertCall("NotNull").WithArgs(value));
        }

        public StatementSyntax AssertThrows(TypeSyntax exceptionType, ExpressionSyntax methodCall, string? associatedParameterName)
        {
            return Generate.Statement(AssertThrowsCore(exceptionType, methodCall, "Throws"));
        }

        public StatementSyntax AssertThrowsAsync(TypeSyntax exceptionType, ExpressionSyntax methodCall, string? associatedParameterName)
        {
            return Generate.Statement(SyntaxFactory.AwaitExpression(AssertThrowsCore(exceptionType, methodCall, "ThrowsAsync")));
        }

        protected override BaseMethodDeclarationSyntax CreateSetupMethodSyntax(string targetTypeName)
        {
            return SyntaxFactory.ConstructorDeclaration(SyntaxFactory.Identifier(targetTypeName)).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
        }

        public IEnumerable<UsingDirectiveSyntax> GetUsings()
        {
            yield return Generate.UsingDirective("Xunit");
        }

        private static InvocationExpressionSyntax AssertCall(string assertMethod)
        {
            return Generate.MemberInvocation("Assert", assertMethod);
        }

        private static InvocationExpressionSyntax AssertThrowsCore(TypeSyntax exceptionType, ExpressionSyntax methodCall, string throws)
        {
            if (exceptionType == null)
            {
                throw new ArgumentNullException(nameof(exceptionType));
            }

            if (methodCall == null)
            {
                throw new ArgumentNullException(nameof(methodCall));
            }

            return Generate.MemberInvocation("Assert", Generate.GenericName(throws, exceptionType), Generate.ParenthesizedLambdaExpression(methodCall));
        }
    }
}