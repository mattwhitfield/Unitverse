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

    public abstract class NUnitTestFramework : ITestFramework
    {
        public bool SupportsStaticTestClasses => true;

        public bool AssertThrowsAsyncIsAwaitable => false;

        public abstract AttributeSyntax SingleThreadedApartmentAttribute { get; }

        public string TestClassAttribute => "TestFixture";

        private static MemberAccessExpressionSyntax AssertThat =>
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName("Assert"),
                SyntaxFactory.IdentifierName("That"));

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

            return SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(AssertThat)
                .WithArgumentList(Generate.Arguments(actual, SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("Is"),
                            SyntaxFactory.IdentifierName("EqualTo")))
                    .WithArgumentList(Generate.Arguments(expected)))));
        }

        public StatementSyntax AssertFail(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            return SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("Assert"),
                        SyntaxFactory.IdentifierName("Fail")))
                .WithArgumentList(Generate.Arguments(Generate.Literal(message))));
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

            return SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(AssertThat)
                .WithArgumentList(Generate.Arguments(actual, SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("Is"),
                            SyntaxFactory.IdentifierName("GreaterThan")))
                    .WithArgumentList(Generate.Arguments(expected)))));
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

            return SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(AssertThat)
                .WithArgumentList(Generate.Arguments(value, SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("Is"),
                        SyntaxFactory.GenericName(
                                SyntaxFactory.Identifier(Strings.NUnitTestFramework_AssertIsInstanceOf_InstanceOf))
                            .WithTypeArgumentList(
                                SyntaxFactory.TypeArgumentList(
                                    SyntaxFactory.SingletonSeparatedList(type))))))));
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

            return SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(AssertThat)
                .WithArgumentList(Generate.Arguments(actual, SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("Is"),
                            SyntaxFactory.IdentifierName("LessThan")))
                    .WithArgumentList(Generate.Arguments(expected)))));
        }

        public StatementSyntax AssertNotNull(ExpressionSyntax value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(Strings.MsTestTestFramework_CreateTestCaseMethod_value);
            }

            return SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(AssertThat)
                .WithArgumentList(Generate.Arguments(
                    value,
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("Is"),
                            SyntaxFactory.IdentifierName("Not")),
                        SyntaxFactory.IdentifierName("Null")))));
        }

        public StatementSyntax AssertThrows(TypeSyntax exceptionType, ExpressionSyntax methodCall)
        {
            return AssertThrows(exceptionType, methodCall, "Throws");
        }

        public StatementSyntax AssertThrowsAsync(TypeSyntax exceptionType, ExpressionSyntax methodCall)
        {
            return AssertThrows(exceptionType, methodCall, "ThrowsAsync");
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
                    SyntaxFactory.SingletonSeparatedList(Generate.Attribute("SetUp"))));
        }

        public MethodDeclarationSyntax CreateTestCaseMethod(string name, bool isAsync, bool isStatic, TypeSyntax valueType, IEnumerable<object> testValues)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException(nameof(valueType));
            }

            if (testValues == null)
            {
                throw new ArgumentNullException(nameof(testValues));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var method = Generate.Method(name, isAsync, isStatic);

            method = method.AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier(Strings.MsTestTestFramework_CreateTestCaseMethod_value)).WithType(valueType));

            foreach (var testValue in testValues)
            {
                method = method.AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(Generate.Attribute("TestCase", testValue))));
            }

            return method;
        }

        public MethodDeclarationSyntax CreateTestMethod(string name, bool isAsync, bool isStatic)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var method = Generate.Method(name, isAsync, isStatic);

            return method.AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(Generate.Attribute("Test"))));
        }

        public virtual IEnumerable<UsingDirectiveSyntax> GetUsings()
        {
            yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(Strings.NUnitTestFramework_GetUsings_NUnit_Framework));
        }

        public abstract IEnumerable<INugetPackageReference> ReferencedNugetPackages(IVersioningOptions options);

        private static StatementSyntax AssertThrows(TypeSyntax exceptionType, ExpressionSyntax methodCall, string throws)
        {
            if (exceptionType == null)
            {
                throw new ArgumentNullException(nameof(exceptionType));
            }

            if (methodCall == null)
            {
                throw new ArgumentNullException(nameof(methodCall));
            }

            return SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("Assert"),
                        SyntaxFactory.GenericName(SyntaxFactory.Identifier(throws))
                            .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(exceptionType)))))
                .WithArgumentList(Generate.Arguments(Generate.ParenthesizedLambdaExpression(methodCall))));
        }
    }
}