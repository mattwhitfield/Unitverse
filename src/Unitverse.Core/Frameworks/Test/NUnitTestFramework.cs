namespace Unitverse.Core.Frameworks.Test
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;
    using Unitverse.Core.Resources;

    public abstract class NUnitTestFramework : BaseTestFramework, ITestFramework, IAssertionFramework
    {
        protected NUnitTestFramework(IUnitTestGeneratorOptions options)
            : base(options)
        {
        }

        public bool SupportsStaticTestClasses => true;

        public bool AssertThrowsAsyncIsAwaitable => false;

        public abstract AttributeSyntax SingleThreadedApartmentAttribute { get; }

        public string TestClassAttribute => "TestFixture";

        private static MemberAccessExpressionSyntax AssertThat =>
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName("Assert"),
                SyntaxFactory.IdentifierName("That"));

        public StatementSyntax AssertEqual(ExpressionSyntax actual, ExpressionSyntax expected, bool isReferenceType)
        {
            return AssertEquality(actual, expected, isReferenceType, false);
        }

        public StatementSyntax AssertNotEqual(ExpressionSyntax actual, ExpressionSyntax expected, bool isReferenceType)
        {
            return AssertEquality(actual, expected, isReferenceType, true);
        }

        private StatementSyntax AssertEquality(ExpressionSyntax actual, ExpressionSyntax expected, bool isReferenceType, bool not)
        {
            if (actual == null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            if (expected == null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            var method = isReferenceType ? "SameAs" : "EqualTo";
            ExpressionSyntax target = SyntaxFactory.IdentifierName("Is");
            if (not)
            {
                target = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, target, SyntaxFactory.IdentifierName("Not"));
            }

            return SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(AssertThat)
                .WithArgumentList(Generate.Arguments(actual, SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            target,
                            SyntaxFactory.IdentifierName(method)))
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

        public StatementSyntax AssertTrue(ExpressionSyntax actual)
        {
            if (actual == null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            return SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(AssertThat)
                .WithArgumentList(Generate.Arguments(actual, SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("Is"),
                            SyntaxFactory.IdentifierName("True")))));
        }

        public StatementSyntax AssertFalse(ExpressionSyntax actual)
        {
            if (actual == null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            return SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(AssertThat)
                .WithArgumentList(Generate.Arguments(actual, SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("Is"),
                            SyntaxFactory.IdentifierName("False")))));
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

            return SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(AssertThat)
                .WithArgumentList(Generate.Arguments(value, SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("Is"),
                        SyntaxFactory.GenericName(
                                SyntaxFactory.Identifier("InstanceOf"))
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
                throw new ArgumentNullException(nameof(value));
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

        public SectionedMethodHandler CreateTestCaseMethod(NameResolver nameResolver, NamingContext namingContext, bool isAsync, bool isStatic, TypeSyntax valueType, IEnumerable<object> testValues)
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

            method = method.AddParameterListParameters(Generate.Parameter("value").WithType(valueType));

            foreach (var testValue in testValues)
            {
                method = method.AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(Generate.Attribute("TestCase", testValue))));
            }

            return new SectionedMethodHandler(method, Options.GenerationOptions.ArrangeComment, Options.GenerationOptions.ActComment, Options.GenerationOptions.AssertComment);
        }

        public SectionedMethodHandler CreateTestMethod(NameResolver nameResolver, NamingContext namingContext, bool isAsync, bool isStatic)
        {
            if (nameResolver is null)
            {
                throw new ArgumentNullException(nameof(nameResolver));
            }

            if (namingContext is null)
            {
                throw new ArgumentNullException(nameof(namingContext));
            }

            var method = Generate.Method(nameResolver.Resolve(namingContext), isAsync, isStatic)
                                 .AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(Generate.Attribute("Test"))));

            return new SectionedMethodHandler(method, Options.GenerationOptions.ArrangeComment, Options.GenerationOptions.ActComment, Options.GenerationOptions.AssertComment);
        }

        public virtual IEnumerable<UsingDirectiveSyntax> GetUsings()
        {
            yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("NUnit.Framework"));
        }

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