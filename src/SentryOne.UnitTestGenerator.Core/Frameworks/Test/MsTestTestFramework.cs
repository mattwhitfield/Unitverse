namespace SentryOne.UnitTestGenerator.Core.Frameworks.Test
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Options;
    using SentryOne.UnitTestGenerator.Core.Resources;

    public class MsTestTestFramework : ITestFramework
    {
        public bool SupportsStaticTestClasses => false;

        public bool AssertThrowsAsyncIsAwaitable => true;

        public AttributeSyntax SingleThreadedApartmentAttribute => null;

        public string TestClassAttribute => "TestClass";

        public StatementSyntax AssertEqual(ExpressionSyntax actual, ExpressionSyntax expected)
        {
            if (actual == null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            if (expected == null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            return SyntaxFactory.ExpressionStatement(AssertCall("AreEqual").WithArgumentList(Generate.Arguments(expected, actual)));
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
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                        SyntaxFactory.Argument(
                            SyntaxFactory.BinaryExpression(
                                SyntaxKind.GreaterThanExpression, actual, expected))))));
        }

        public StatementSyntax AssertIsInstanceOf(ExpressionSyntax value, TypeSyntax type)
        {
            if (value == null)
            {
                throw new ArgumentNullException(Strings.MsTestTestFramework_CreateTestCaseMethod_value);
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
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                        SyntaxFactory.Argument(
                            SyntaxFactory.BinaryExpression(
                                SyntaxKind.LessThanExpression, actual, expected))))));
        }

        public StatementSyntax AssertNotNull(ExpressionSyntax value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(Strings.MsTestTestFramework_CreateTestCaseMethod_value);
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

        public BaseMethodDeclarationSyntax CreateSetupMethod(string targetTypeName)
        {
            if (string.IsNullOrWhiteSpace(targetTypeName))
            {
                throw new ArgumentNullException(nameof(targetTypeName));
            }

            var method = Generate.Method("SetUp", false, false);

            return method.AddAttributeLists(
                SyntaxFactory.AttributeList(
                    SyntaxFactory.SingletonSeparatedList(Generate.Attribute("TestInitialize"))));
        }

        public MethodDeclarationSyntax CreateTestCaseMethod(string name, bool isAsync, bool isStatic, TypeSyntax valueType, IEnumerable<object> testValues)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (valueType == null)
            {
                throw new ArgumentNullException(nameof(valueType));
            }

            if (testValues == null)
            {
                throw new ArgumentNullException(nameof(testValues));
            }

            var method = Generate.Method(name, isAsync, false);

            method = method.AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier(Strings.MsTestTestFramework_CreateTestCaseMethod_value)).WithType(valueType));
            method = method.AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(Generate.Attribute("DataTestMethod"))));

            foreach (var testValue in testValues)
            {
                method = method.AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(Generate.Attribute("DataRow", testValue))));
            }

            return method;
        }

        public MethodDeclarationSyntax CreateTestMethod(string name, bool isAsync, bool isStatic)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var method = Generate.Method(name, isAsync, false);

            return method.AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(Generate.Attribute("TestMethod"))));
        }

        public IEnumerable<UsingDirectiveSyntax> GetUsings()
        {
            yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(Strings.MsTestTestFramework_GetUsings_Microsoft_VisualStudio_TestTools_UnitTesting));
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