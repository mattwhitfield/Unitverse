namespace Unitverse.Core.Frameworks.Test
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;

    public abstract class NUnitTestFramework : BaseTestFramework, IExtendedTestFramework
    {
        protected NUnitTestFramework(IUnitTestGeneratorOptions options)
            : base(options)
        {
        }

        public override bool SupportsStaticTestClasses => true;

        public bool AssertThrowsAsyncIsAwaitable => false;

        public abstract AttributeSyntax SingleThreadedApartmentAttribute { get; }

        public string TestClassAttribute => "TestFixture";

        protected override string TestAttributeName => "Test";

        protected override string TestCaseMethodAttributeName => string.Empty;

        protected override string TestCaseAttributeName => "TestCase";

        public bool SkipValueTypeNotNull => false;

        private static InvocationExpressionSyntax AssertThat => Generate.MemberInvocation("Assert", "That");

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
                target = Generate.MemberAccess(target, "Not");
            }

            return Generate.Statement(AssertThat.WithArgs(actual, Generate.MemberInvocation(target, method, expected)));
        }

        public StatementSyntax AssertFail(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            return Generate.Statement(Generate.MemberInvocation("Assert", "Fail", Generate.Literal(message)));
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

            return Generate.Statement(AssertThat.WithArgs(actual, Generate.MemberInvocation("Is", "GreaterThan", expected)));
        }

        public StatementSyntax AssertTrue(ExpressionSyntax actual)
        {
            if (actual == null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            return Generate.Statement(AssertThat.WithArgs(actual, Generate.MemberAccess("Is", "True")));
        }

        public StatementSyntax AssertFalse(ExpressionSyntax actual)
        {
            if (actual == null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            return Generate.Statement(AssertThat.WithArgs(actual, Generate.MemberAccess("Is", "False")));
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

            return Generate.Statement(
                        AssertThat.WithArgs(value, Generate.MemberInvocation("Is", Generate.GenericName("InstanceOf", type))));
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

            return Generate.Statement(
                        AssertThat.WithArgs(actual, Generate.MemberInvocation("Is", "LessThan", expected)));
        }

        public StatementSyntax AssertNotNull(ExpressionSyntax value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return Generate.Statement(
                        AssertThat.WithArgs(value, Generate.MemberAccess(Generate.MemberAccess("Is", "Not"), "Null")));
        }

        public StatementSyntax AssertThrows(TypeSyntax exceptionType, ExpressionSyntax methodCall, string? associatedParameterName)
        {
            return AssertThrowsCore(exceptionType, methodCall, "Throws");
        }

        public StatementSyntax AssertThrowsAsync(TypeSyntax exceptionType, ExpressionSyntax methodCall, string? associatedParameterName)
        {
            return AssertThrowsCore(exceptionType, methodCall, "ThrowsAsync");
        }

        protected override BaseMethodDeclarationSyntax CreateSetupMethodSyntax(string targetTypeName)
        {
            if (Options.GenerationOptions.UseConstructorForTestClassSetUp)
            {
                return SyntaxFactory.ConstructorDeclaration(SyntaxFactory.Identifier(targetTypeName)).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            }

            return Generate.Method("SetUp", false, false).AddAttributeLists(Generate.Attribute("SetUp").AsList());
        }

        public virtual IEnumerable<UsingDirectiveSyntax> GetUsings()
        {
            yield return Generate.UsingDirective("NUnit.Framework");
        }

        private static StatementSyntax AssertThrowsCore(TypeSyntax exceptionType, ExpressionSyntax methodCall, string throws)
        {
            if (exceptionType == null)
            {
                throw new ArgumentNullException(nameof(exceptionType));
            }

            if (methodCall == null)
            {
                throw new ArgumentNullException(nameof(methodCall));
            }

            return Generate.Statement(Generate.MemberInvocation("Assert", Generate.GenericName(throws, exceptionType), Generate.ParenthesizedLambdaExpression(methodCall)));
        }
    }
}