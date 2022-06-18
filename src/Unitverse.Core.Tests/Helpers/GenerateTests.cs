namespace Unitverse.Core.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    [TestFixture]
    public static class GenerateTests
    {
        [Test]
        [TestCase(false, false, "public void TestValue64466904()")]
        [TestCase(true, false, "public async Task TestValue64466904()")]
        [TestCase(false, true, "public static void TestValue64466904()")]
        [TestCase(true, true, "public static async Task TestValue64466904()")]
        public static void CanCallMethod(bool isAsync, bool isStatic, string expected)
        {
            var name = "TestValue64466904";
            var result = Generate.Method(name, isAsync, isStatic);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo(expected));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CannotCallMethodWithInvalidName(string value)
        {
            Assert.Throws<ArgumentNullException>(() => Generate.Method(value, false, true));
        }

        [Test]
        [TestCase("fred", "\"fred\"")]
        [TestCase(true, "true")]
        [TestCase(false, "false")]
        [TestCase(123, "123")]
        [TestCase((short)123, "123")]
        [TestCase(123L, "123L")]
        [TestCase((byte)123, "123")]
        [TestCase(123.45, "123.45")]
        [TestCase(123.45f, "123.45F")]
        public static void CanCallLiteral(object literal, string expected)
        {
            var result = Generate.Literal(literal);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo(expected));
        }

        [Test]
        public static void CanCallLiteralWithNullLiteral()
        {
            Assert.That(Generate.Literal(default(object)), Is.InstanceOf<LiteralExpressionSyntax>());
        }

        [Test]
        public static void CannotCallLiteralWithNonLiteralObject()
        {
            Assert.Throws<InvalidOperationException>(() => Generate.Literal(new List<int>()));
        }

        [Test]
        public static void CanCallMethodCall()
        {
            var target = SyntaxFactory.IdentifierName("foo");
            var method = Generate.Method("Test", false, false);
            var name = "TestValue873775832";
            var arguments = new[] { Generate.Literal(1), Generate.Literal(1), Generate.Literal(1) };
            var result = Generate.MethodCall(target, method, name, Substitute.For<IFrameworkSet>(), arguments);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo("foo.TestValue873775832(1, 1, 1)"));
        }

        [Test]
        public static void CannotCallMethodCallWithNullTarget()
        {
            Assert.Throws<ArgumentNullException>(() => Generate.MethodCall(default(ExpressionSyntax), Generate.Method("Test", false, false), "TestValue1389042061", Substitute.For<IFrameworkSet>(), Generate.Literal(1), Generate.Literal(1), Generate.Literal(1)));
        }

        [Test]
        public static void CannotCallMethodCallWithNullMethod()
        {
            Assert.Throws<ArgumentNullException>(() => Generate.MethodCall(Generate.Literal(1), default(MethodDeclarationSyntax), "TestValue1162570393", Substitute.For<IFrameworkSet>(), Generate.Literal(1), Generate.Literal(1), Generate.Literal(1)));
        }

        [Test]
        public static void CannotCallMethodCallWithNullName()
        {
            Assert.Throws<ArgumentNullException>(() => Generate.MethodCall(Generate.Literal(1), Generate.Method("Test", false, false), default(string), Substitute.For<IFrameworkSet>(), Generate.Literal(1), Generate.Literal(1), Generate.Literal(1)));
        }

        [Test]
        public static void CannotCallMethodCallWithNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => Generate.MethodCall(Generate.Literal(1), Generate.Method("Test", false, false), "TestValue515316561", Substitute.For<IFrameworkSet>(), default(ExpressionSyntax[])));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CannotCallMethodCallWithInvalidName(string value)
        {
            Assert.Throws<ArgumentNullException>(() => Generate.MethodCall(Generate.Literal(1), Generate.Method("Test", false, false), value, Substitute.For<IFrameworkSet>(), Generate.Literal(1), Generate.Literal(1), Generate.Literal(1)));
        }

        [Test]
        public static void CanCallAttributeWithName()
        {
            var name = "TestValue905136492";
            var result = Generate.Attribute(name);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo("TestValue905136492"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CannotCallAttributeWithNameWithInvalidName(string value)
        {
            Assert.Throws<ArgumentNullException>(() => Generate.Attribute(value));
        }

        [Test]
        public static void CanCallAttributeWithStringAndArrayOfObject()
        {
            var name = "TestValue220626612";
            var arguments = new object[] { 1, 4, 2 };
            var result = Generate.Attribute(name, arguments);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo("TestValue220626612(1, 4, 2)"));
        }

        [Test]
        public static void CannotCallAttributeWithStringAndArrayOfObjectWithNullName()
        {
            Assert.Throws<ArgumentNullException>(() => Generate.Attribute(default(string), new object(), new object(), new object()));
        }

        [Test]
        public static void CannotCallAttributeWithStringAndArrayOfObjectWithNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => Generate.Attribute("TestValue565899197", default(object[])));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CannotCallAttributeWithStringAndArrayOfObjectWithInvalidName(string value)
        {
            Assert.Throws<ArgumentNullException>(() => Generate.Attribute(value, new object(), new object(), new object()));
        }

        [Test]
        public static void CanCallAttributeWithStringAndArrayOfExpressionSyntax()
        {
            var name = "TestValue1946645090";
            var arguments = new[] { Generate.Literal(1), Generate.Literal(1), Generate.Literal(1) };
            var result = Generate.Attribute(name, arguments);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo("TestValue1946645090(1, 1, 1)"));
        }

        [Test]
        public static void CannotCallAttributeWithStringAndArrayOfExpressionSyntaxWithNullName()
        {
            Assert.Throws<ArgumentNullException>(() => Generate.Attribute(default(string), Generate.Literal(1), Generate.Literal(1), Generate.Literal(1)));
        }

        [Test]
        public static void CannotCallAttributeWithStringAndArrayOfExpressionSyntaxWithNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => Generate.Attribute("TestValue638342250", default(ExpressionSyntax[])));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CannotCallAttributeWithStringAndArrayOfExpressionSyntaxWithInvalidName(string value)
        {
            Assert.Throws<ArgumentNullException>(() => Generate.Attribute(value, Generate.Literal(1), Generate.Literal(1), Generate.Literal(1)));
        }

        [Test]
        public static void CanCallIndexerAccess()
        {
            var target = SyntaxFactory.IdentifierName("_test");
            var arguments = new ExpressionSyntax[] { Generate.Literal(1), Generate.Literal(4), Generate.Literal(2) };
            var result = Generate.IndexerAccess(target, arguments);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo("_test[1, 4, 2]"));
        }

        [Test]
        public static void CannotCallIndexerAccessWithNullTarget()
        {
            Assert.Throws<ArgumentNullException>(() => Generate.IndexerAccess(default(ExpressionSyntax), Generate.Literal(1), Generate.Literal(1), Generate.Literal(1)));
        }

        [Test]
        public static void CannotCallIndexerAccessWithNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => Generate.IndexerAccess(Generate.Literal(1), default(ExpressionSyntax[])));
        }

        [Test]
        public static void CanCallObjectCreation()
        {
            var type = SyntaxFactory.IdentifierName("foo");
            var arguments = new ExpressionSyntax[] { Generate.Literal(1), Generate.Literal(1), Generate.Literal(1) };
            var result = Generate.ObjectCreation(type, arguments);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo("new foo(1, 1, 1)"));
        }

        [Test]
        public static void CannotCallObjectCreationWithNullType()
        {
            Assert.Throws<ArgumentNullException>(() => Generate.ObjectCreation(default(TypeSyntax), Generate.Literal(1), Generate.Literal(1), Generate.Literal(1)));
        }

        [Test]
        public static void CannotCallObjectCreationWithNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => Generate.ObjectCreation(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), default(ExpressionSyntax[])));
        }

        [Test]
        public static void CanCallSetupMethod()
        {
            var extractor = new TestableItemExtractor(TestSemanticModelFactory.Tree, TestSemanticModelFactory.Model);
            var model = extractor.Extract(null, Substitute.For<IUnitTestGeneratorOptions>()).First();
            var targetTypeName = "TestValue1540739065";
            var options = Substitute.For<IUnitTestGeneratorOptions>();
            options.NamingOptions.TargetFieldName.Returns("_testClass");
            options.NamingOptions.DependencyFieldName.Returns("_{parameterName:camel}");
            options.GenerationOptions.TestTypeNaming.Returns("{0}Tests");
            options.GenerationOptions.FrameworkType.Returns(TestFrameworkTypes.NUnit3);
            var frameworkSet = FrameworkSetFactory.Create(options);
            var classDeclaration = TestSemanticModelFactory.Class;

            var result = Generate.SetupMethod(model, targetTypeName, frameworkSet, ref classDeclaration);
            Assert.That(result.Method.NormalizeWhitespace().ToFullString().StartsWith("[SetUp]\r\npublic void SetUp()\r\n{\r\n    _param = \"TestValue", StringComparison.Ordinal));
        }

        [Test]
        public static void CannotCallSetupMethodWithNullModel()
        {
            var classDeclaration = TestSemanticModelFactory.Class;
            Assert.Throws<ArgumentNullException>(() => Generate.SetupMethod(default(ClassModel), "TestValue1152815127", Substitute.For<IFrameworkSet>(), ref classDeclaration));
        }

        [Test]
        public static void CannotCallSetupMethodWithNullTargetTypeName()
        {
            var classDeclaration = TestSemanticModelFactory.Class;
            Assert.Throws<ArgumentNullException>(() => Generate.SetupMethod(new ClassModel(TestSemanticModelFactory.Class, TestSemanticModelFactory.Model, false), default(string), Substitute.For<IFrameworkSet>(), ref classDeclaration));
        }

        [Test]
        public static void CannotCallSetupMethodWithNullFrameworkSet()
        {
            var classDeclaration = TestSemanticModelFactory.Class;
            Assert.Throws<ArgumentNullException>(() => Generate.SetupMethod(new ClassModel(TestSemanticModelFactory.Class, TestSemanticModelFactory.Model, true), "TestValue967983743", default(IFrameworkSet), ref classDeclaration));
        }

        [Test]
        public static void CannotCallSetupMethodWithNullClassDeclaration()
        {
            var classDeclaration = default(ClassDeclarationSyntax);
            Assert.Throws<ArgumentNullException>(() => Generate.SetupMethod(new ClassModel(TestSemanticModelFactory.Class, TestSemanticModelFactory.Model, false), "TestValue1524243242", Substitute.For<IFrameworkSet>(), ref classDeclaration));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CannotCallSetupMethodWithInvalidTargetTypeName(string value)
        {
            var classDeclaration = TestSemanticModelFactory.Class;
            Assert.Throws<ArgumentNullException>(() => Generate.SetupMethod(new ClassModel(TestSemanticModelFactory.Class, TestSemanticModelFactory.Model, false), value, Substitute.For<IFrameworkSet>(), ref classDeclaration));
        }

        [Test]
        public static void CanCallArgumentsWithArrayOfExpressionSyntax()
        {
            var expressions = new ExpressionSyntax[] { Generate.Literal(1), Generate.Literal(1), Generate.Literal(1) };
            var result = Generate.Arguments(expressions);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo("(1, 1, 1)"));
        }

        [Test]
        public static void CannotCallArgumentsWithArrayOfExpressionSyntaxWithNullExpressions()
        {
            Assert.Throws<ArgumentNullException>(() => Generate.Arguments(default(ExpressionSyntax[])));
        }

        [Test]
        public static void CanCallArgumentsWithIEnumerableOfExpressionSyntax()
        {
            var expressions = new ExpressionSyntax[] { Generate.Literal(1), Generate.Literal(1), Generate.Literal(1) };
            var result = Generate.Arguments(expressions);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo("(1, 1, 1)"));
        }

        [Test]
        public static void CannotCallArgumentsWithIEnumerableOfExpressionSyntaxWithNullExpressions()
        {
            Assert.Throws<ArgumentNullException>(() => Generate.Arguments(default(IEnumerable<ExpressionSyntax>)));
        }
    }
}