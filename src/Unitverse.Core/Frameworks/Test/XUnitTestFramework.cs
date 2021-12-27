namespace Unitverse.Core.Frameworks.Test
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;
    using Unitverse.Core.Resources;

    public class XUnitTestFramework : ITestFramework, IAssertionFramework
    {
        public bool SupportsStaticTestClasses => true;

        public bool AssertThrowsAsyncIsAwaitable => true;

        public AttributeSyntax SingleThreadedApartmentAttribute => null;

        public string TestClassAttribute => string.Empty;

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

            return SyntaxFactory.ExpressionStatement(AssertCall("Equal").WithArgumentList(Generate.Arguments(expected, actual)));
        }

        public StatementSyntax AssertFail(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            return SyntaxFactory.ThrowStatement(Generate.ObjectCreation(SyntaxFactory.IdentifierName("NotImplementedException"), Generate.Literal(message)));
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

            return SyntaxFactory.ExpressionStatement(AssertCall("True").WithArgumentList(
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                        SyntaxFactory.Argument(
                            SyntaxFactory.BinaryExpression(
                                SyntaxKind.GreaterThanExpression, actual, expected))))));
        }

        public StatementSyntax AssertIsInstanceOf(ExpressionSyntax value, TypeSyntax type, bool isReferenceType)
        {
            if (value == null)
            {
                throw new ArgumentNullException(Strings.MsTestTestFramework_CreateTestCaseMethod_value);
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("Assert"),
                        SyntaxFactory.GenericName(SyntaxFactory.Identifier(Strings.XUnitTestFramework_AssertIsInstanceOf_IsType))
                            .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(type)))))
                .WithArgumentList(Generate.Arguments(value)));
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

            return SyntaxFactory.ExpressionStatement(AssertCall("True").WithArgumentList(
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

            return SyntaxFactory.ExpressionStatement(AssertCall("NotNull").WithArgumentList(Generate.Arguments(value)));
        }

        public StatementSyntax AssertThrows(TypeSyntax exceptionType, ExpressionSyntax methodCall)
        {
            return SyntaxFactory.ExpressionStatement(AssertThrows(exceptionType, methodCall, "Throws"));
        }

        public StatementSyntax AssertThrowsAsync(TypeSyntax exceptionType, ExpressionSyntax methodCall)
        {
            return SyntaxFactory.ExpressionStatement(SyntaxFactory.AwaitExpression(AssertThrows(exceptionType, methodCall, "ThrowsAsync")));
        }

        public BaseMethodDeclarationSyntax CreateSetupMethod(string targetTypeName)
        {
            if (string.IsNullOrWhiteSpace(targetTypeName))
            {
                throw new ArgumentNullException(nameof(targetTypeName));
            }

            return SyntaxFactory.ConstructorDeclaration(SyntaxFactory.Identifier(targetTypeName)).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
        }

        public MethodDeclarationSyntax CreateTestCaseMethod(NameResolver nameResolver, NamingContext namingContext, bool isAsync, bool isStatic, TypeSyntax valueType, IEnumerable<object> testValues)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException(nameof(valueType));
            }

            if (testValues == null)
            {
                throw new ArgumentNullException(nameof(testValues));
            }

            if (nameResolver is null)
            {
                throw new ArgumentNullException(nameof(nameResolver));
            }

            if (namingContext is null)
            {
                throw new ArgumentNullException(nameof(namingContext));
            }

            var method = Generate.Method(nameResolver.Resolve(namingContext), isAsync, isStatic);

            method = method.AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier(Strings.MsTestTestFramework_CreateTestCaseMethod_value)).WithType(valueType));
            method = method.AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(Generate.Attribute("Theory"))));

            foreach (var testValue in testValues)
            {
                method = method.AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(Generate.Attribute("InlineData", testValue))));
            }

            return method;
        }

        public MethodDeclarationSyntax CreateTestMethod(NameResolver nameResolver, NamingContext namingContext, bool isAsync, bool isStatic)
        {
            if (nameResolver is null)
            {
                throw new ArgumentNullException(nameof(nameResolver));
            }

            if (namingContext is null)
            {
                throw new ArgumentNullException(nameof(namingContext));
            }

            var method = Generate.Method(nameResolver.Resolve(namingContext), isAsync, isStatic);

            return method.AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(Generate.Attribute("Fact"))));
        }

        public IEnumerable<UsingDirectiveSyntax> GetUsings()
        {
            yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(Strings.XUnitTestFramework_GetUsings_Xunit));
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