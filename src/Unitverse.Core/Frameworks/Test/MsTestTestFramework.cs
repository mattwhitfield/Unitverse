namespace Unitverse.Core.Frameworks.Test
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;

    public class MsTestTestFramework : BaseTestFramework, IExtendedTestFramework
    {
        public MsTestTestFramework(IUnitTestGeneratorOptions options)
            : base(options)
        {
        }

        public override bool SupportsStaticTestClasses => false;

        public bool AssertThrowsAsyncIsAwaitable => true;

        public AttributeSyntax SingleThreadedApartmentAttribute => null;

        public string TestClassAttribute => "TestClass";

        protected override string TestAttributeName => "TestMethod";

        protected override string TestCaseMethodAttributeName => "DataTestMethod";

        protected override string TestCaseAttributeName => "DataRow";

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

            var method = isReferenceType ? "AreSame" : "AreEqual";
            return SyntaxFactory.ExpressionStatement(AssertCall(method).WithArgumentList(Generate.Arguments(expected, actual)));
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

            var method = isReferenceType ? "AreNotSame" : "AreNotEqual";
            return SyntaxFactory.ExpressionStatement(AssertCall(method).WithArgumentList(Generate.Arguments(expected, actual)));
        }

        public StatementSyntax AssertFail(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            return SyntaxFactory.ExpressionStatement(AssertCall("Fail").WithArgumentList(Generate.Arguments(Generate.Literal(message))));
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

            return SyntaxFactory.ExpressionStatement(AssertCall("IsTrue").WithArgumentList(
                Generate.Arguments(SyntaxFactory.BinaryExpression(SyntaxKind.GreaterThanExpression, actual, expected))));
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

            return SyntaxFactory.ExpressionStatement(AssertCall("IsInstanceOfType").WithArgumentList(Generate.Arguments(value, SyntaxFactory.TypeOfExpression(type))));
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

            return SyntaxFactory.ExpressionStatement(AssertCall("IsTrue").WithArgumentList(
                Generate.Arguments(SyntaxFactory.BinaryExpression(SyntaxKind.LessThanExpression, actual, expected))));
        }

        public StatementSyntax AssertTrue(ExpressionSyntax actual)
        {
            if (actual == null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            return SyntaxFactory.ExpressionStatement(AssertCall("IsTrue").WithArgumentList(Generate.Arguments(actual)));
        }

        public StatementSyntax AssertFalse(ExpressionSyntax actual)
        {
            if (actual == null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            return SyntaxFactory.ExpressionStatement(AssertCall("IsFalse").WithArgumentList(Generate.Arguments(actual)));
        }

        public StatementSyntax AssertNotNull(ExpressionSyntax value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return SyntaxFactory.ExpressionStatement(AssertCall("IsNotNull").WithArgumentList(Generate.Arguments(value)));
        }

        public StatementSyntax AssertThrows(TypeSyntax exceptionType, ExpressionSyntax methodCall)
        {
            return SyntaxFactory.ExpressionStatement(AssertThrows(exceptionType, methodCall, "ThrowsException"));
        }

        public StatementSyntax AssertThrowsAsync(TypeSyntax exceptionType, ExpressionSyntax methodCall)
        {
            return SyntaxFactory.ExpressionStatement(SyntaxFactory.AwaitExpression(AssertThrows(exceptionType, methodCall, "ThrowsExceptionAsync")));
        }

        protected override BaseMethodDeclarationSyntax CreateSetupMethodSyntax(string targetTypeName)
        {
            return Generate.Method("SetUp", false, false).AddAttributeLists(Generate.Attribute("TestInitialize").AsList());
        }

        public IEnumerable<UsingDirectiveSyntax> GetUsings()
        {
            yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Microsoft.VisualStudio.TestTools.UnitTesting"));
        }

        private static InvocationExpressionSyntax AssertCall(string assertMethod)
        {
            return SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName("Assert"),
                SyntaxFactory.IdentifierName(assertMethod)));
        }

        private static InvocationExpressionSyntax AssertThrows(TypeSyntax exceptionType, ExpressionSyntax methodCall, string throws)
        {
            if (exceptionType == null)
            {
                throw new ArgumentNullException(nameof(exceptionType));
            }

            if (methodCall == null)
            {
                throw new ArgumentNullException(nameof(methodCall));
            }

            return SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("Assert"),
                        SyntaxFactory.GenericName(SyntaxFactory.Identifier(throws))
                            .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(exceptionType)))))
                .WithArgumentList(Generate.Arguments(Generate.ParenthesizedLambdaExpression(methodCall)));
        }
    }
}