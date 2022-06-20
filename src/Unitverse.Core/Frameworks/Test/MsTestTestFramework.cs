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

        public AttributeSyntax? SingleThreadedApartmentAttribute => null;

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

            var method = isReferenceType ? "AreNotSame" : "AreNotEqual";
            return Generate.Statement(AssertCall(method).WithArgs(expected, actual));
        }

        public StatementSyntax AssertFail(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            return Generate.Statement(AssertCall("Fail").WithArgs(Generate.Literal(message)));
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

            return Generate.Statement(AssertCall("IsTrue").WithArgs(SyntaxFactory.BinaryExpression(SyntaxKind.GreaterThanExpression, actual, expected)));
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

            return Generate.Statement(AssertCall("IsInstanceOfType").WithArgs(value, SyntaxFactory.TypeOfExpression(type)));
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

            return Generate.Statement(AssertCall("IsTrue").WithArgs(SyntaxFactory.BinaryExpression(SyntaxKind.LessThanExpression, actual, expected)));
        }

        public StatementSyntax AssertTrue(ExpressionSyntax actual)
        {
            if (actual == null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            return Generate.Statement(AssertCall("IsTrue").WithArgs(actual));
        }

        public StatementSyntax AssertFalse(ExpressionSyntax actual)
        {
            if (actual == null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            return Generate.Statement(AssertCall("IsFalse").WithArgs(actual));
        }

        public StatementSyntax AssertNotNull(ExpressionSyntax value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return Generate.Statement(AssertCall("IsNotNull").WithArgs(value));
        }

        public StatementSyntax AssertThrows(TypeSyntax exceptionType, ExpressionSyntax methodCall)
        {
            return Generate.Statement(AssertThrows(exceptionType, methodCall, "ThrowsException"));
        }

        public StatementSyntax AssertThrowsAsync(TypeSyntax exceptionType, ExpressionSyntax methodCall)
        {
            return Generate.Statement(SyntaxFactory.AwaitExpression(AssertThrows(exceptionType, methodCall, "ThrowsExceptionAsync")));
        }

        protected override BaseMethodDeclarationSyntax CreateSetupMethodSyntax(string targetTypeName)
        {
            return Generate.Method("SetUp", false, false).AddAttributeLists(Generate.Attribute("TestInitialize").AsList());
        }

        public IEnumerable<UsingDirectiveSyntax> GetUsings()
        {
            yield return Generate.UsingDirective("Microsoft.VisualStudio.TestTools.UnitTesting");
        }

        private static InvocationExpressionSyntax AssertCall(string assertMethod)
        {
            return Generate.MemberInvocation("Assert", assertMethod);
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

            return Generate.MemberInvocation("Assert", Generate.GenericName(throws, exceptionType), Generate.ParenthesizedLambdaExpression(methodCall));
        }
    }
}