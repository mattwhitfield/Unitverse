namespace Unitverse.Core.Tests.Frameworks.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Frameworks.Test;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;

    [TestFixture]
    public class TestFrameworkTests
    {
        public static IEnumerable<ITestFramework> Targets
        {
            get
            {
                yield return new MsTestTestFramework();
                yield return new NUnit2TestFramework();
                yield return new NUnit3TestFramework();
                yield return new XUnitTestFramework();
            }
        }

        [TestCase(TestFrameworkTypes.NUnit2, "Assert.That(1, Is.EqualTo(1));")]
        [TestCase(TestFrameworkTypes.NUnit3, "Assert.That(1, Is.EqualTo(1));")]
        [TestCase(TestFrameworkTypes.MsTest, "Assert.AreEqual(1, 1);")]
        [TestCase(TestFrameworkTypes.XUnit, "Assert.Equal(1, 1);")]
        public void CanCallAssertEqual(TestFrameworkTypes frameworkTypes, string expectedOutput)
        {
            var testClass = CreateFramework(frameworkTypes);
            var actual = Generate.Literal(1);
            var expected = Generate.Literal(1);
            var result = testClass.AssertEqual(actual, expected, false);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo(expectedOutput));
        }

        [TestCase(TestFrameworkTypes.NUnit2, "Assert.That(1, Is.SameAs(1));")]
        [TestCase(TestFrameworkTypes.NUnit3, "Assert.That(1, Is.SameAs(1));")]
        [TestCase(TestFrameworkTypes.MsTest, "Assert.AreSame(1, 1);")]
        [TestCase(TestFrameworkTypes.XUnit, "Assert.Same(1, 1);")]
        public void CanCallAssertEqualRef(TestFrameworkTypes frameworkTypes, string expectedOutput)
        {
            var testClass = CreateFramework(frameworkTypes);
            var actual = Generate.Literal(1);
            var expected = Generate.Literal(1);
            var result = testClass.AssertEqual(actual, expected, true);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo(expectedOutput));
        }

        [TestCase(TestFrameworkTypes.NUnit2, "Assert.That(derf, Is.True);")]
        [TestCase(TestFrameworkTypes.NUnit3, "Assert.That(derf, Is.True);")]
        [TestCase(TestFrameworkTypes.MsTest, "Assert.IsTrue(derf);")]
        [TestCase(TestFrameworkTypes.XUnit, "Assert.True(derf);")]
        public void CanCallAssertTrue(TestFrameworkTypes frameworkTypes, string expectedOutput)
        {
            var testClass = CreateFramework(frameworkTypes);
            var actual = SyntaxFactory.IdentifierName("derf");
            var result = testClass.AssertTrue(actual);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo(expectedOutput));
        }

        [TestCase(TestFrameworkTypes.NUnit2, "Assert.That(derf, Is.False);")]
        [TestCase(TestFrameworkTypes.NUnit3, "Assert.That(derf, Is.False);")]
        [TestCase(TestFrameworkTypes.MsTest, "Assert.IsFalse(derf);")]
        [TestCase(TestFrameworkTypes.XUnit, "Assert.False(derf);")]
        public void CanCallAssertFalse(TestFrameworkTypes frameworkTypes, string expectedOutput)
        {
            var testClass = CreateFramework(frameworkTypes);
            var actual = SyntaxFactory.IdentifierName("derf");
            var result = testClass.AssertFalse(actual);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo(expectedOutput));
        }

        [TestCase(TestFrameworkTypes.NUnit2, "Assert.Fail(\"TestValue538721341\");")]
        [TestCase(TestFrameworkTypes.NUnit3, "Assert.Fail(\"TestValue538721341\");")]
        [TestCase(TestFrameworkTypes.MsTest, "Assert.Fail(\"TestValue538721341\");")]
        [TestCase(TestFrameworkTypes.XUnit, "throw new NotImplementedException(\"TestValue538721341\");")]
        public void CanCallAssertFail(TestFrameworkTypes frameworkTypes, string expectedOutput)
        {
            var testClass = CreateFramework(frameworkTypes);
            var message = "TestValue538721341";
            var result = testClass.AssertFail(message);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo(expectedOutput));
        }

        [TestCase(TestFrameworkTypes.NUnit2, "Assert.That(1, Is.InstanceOf<int>());")]
        [TestCase(TestFrameworkTypes.NUnit3, "Assert.That(1, Is.InstanceOf<int>());")]
        [TestCase(TestFrameworkTypes.MsTest, "Assert.IsInstanceOfType(1, typeof(int));")]
        [TestCase(TestFrameworkTypes.XUnit, "Assert.IsType<int>(1);")]
        public void CanCallAssertIsInstanceOf(TestFrameworkTypes frameworkTypes, string expectedOutput)
        {
            var testClass = CreateFramework(frameworkTypes);
            var value = Generate.Literal(1);
            var type = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword));
            var result = testClass.AssertIsInstanceOf(value, type, true);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo(expectedOutput));
        }

        [TestCase(TestFrameworkTypes.NUnit2, "Assert.That(1, Is.Not.Null);")]
        [TestCase(TestFrameworkTypes.NUnit3, "Assert.That(1, Is.Not.Null);")]
        [TestCase(TestFrameworkTypes.MsTest, "Assert.IsNotNull(1);")]
        [TestCase(TestFrameworkTypes.XUnit, "Assert.NotNull(1);")]
        public void CanCallAssertNotNull(TestFrameworkTypes frameworkTypes, string expectedOutput)
        {
            var testClass = CreateFramework(frameworkTypes);
            var value = Generate.Literal(1);
            var result = testClass.AssertNotNull(value);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo(expectedOutput));
        }

        [TestCase(TestFrameworkTypes.NUnit2, "Assert.Throws<int>(() => 1);")]
        [TestCase(TestFrameworkTypes.NUnit3, "Assert.Throws<int>(() => 1);")]
        [TestCase(TestFrameworkTypes.MsTest, "Assert.ThrowsException<int>(() => 1);")]
        [TestCase(TestFrameworkTypes.XUnit, "Assert.Throws<int>(() => 1);")]
        public void CanCallAssertThrows(TestFrameworkTypes frameworkTypes, string expectedOutput)
        {
            var testClass = CreateFramework(frameworkTypes);
            var exceptionType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword));
            var methodCall = Generate.Literal(1);
            var result = testClass.AssertThrows(exceptionType, methodCall);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo(expectedOutput));
        }

        [TestCase(TestFrameworkTypes.NUnit2, "Assert.ThrowsAsync<int>(() => 1);")]
        [TestCase(TestFrameworkTypes.NUnit3, "Assert.ThrowsAsync<int>(() => 1);")]
        [TestCase(TestFrameworkTypes.MsTest, "await Assert.ThrowsExceptionAsync<int>(() => 1);")]
        [TestCase(TestFrameworkTypes.XUnit, "await Assert.ThrowsAsync<int>(() => 1);")]
        public void CanCallAssertThrowsAsync(TestFrameworkTypes frameworkTypes, string expectedOutput)
        {
            var testClass = CreateFramework(frameworkTypes);
            var exceptionType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword));
            var methodCall = Generate.Literal(1);
            var result = testClass.AssertThrowsAsync(exceptionType, methodCall);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo(expectedOutput));
        }

        [TestCase(TestFrameworkTypes.NUnit2, "[SetUp]\r\npublic void SetUp()")]
        [TestCase(TestFrameworkTypes.NUnit3, "[SetUp]\r\npublic void SetUp()")]
        [TestCase(TestFrameworkTypes.MsTest, "[TestInitialize]\r\npublic void SetUp()")]
        [TestCase(TestFrameworkTypes.XUnit, "public TestValue1801609112()")]
        public void CanCallCreateSetupMethod(TestFrameworkTypes frameworkTypes, string expectedOutput)
        {
            var testClass = CreateFramework(frameworkTypes);
            var targetTypeName = "TestValue1801609112";
            var result = testClass.CreateSetupMethod(targetTypeName);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo(expectedOutput));
        }

        [TestCase(TestFrameworkTypes.NUnit2, "[TestCase(1)]\r\n[TestCase(2)]\r\npublic void TestValue947022583(int value)")]
        [TestCase(TestFrameworkTypes.NUnit3, "[TestCase(1)]\r\n[TestCase(2)]\r\npublic void TestValue947022583(int value)")]
        [TestCase(TestFrameworkTypes.MsTest, "[DataTestMethod]\r\n[DataRow(1)]\r\n[DataRow(2)]\r\npublic void TestValue947022583(int value)")]
        [TestCase(TestFrameworkTypes.XUnit, "[Theory]\r\n[InlineData(1)]\r\n[InlineData(2)]\r\npublic void TestValue947022583(int value)")]
        public void CanCallCreateTestCaseMethod(TestFrameworkTypes frameworkTypes, string expectedOutput)
        {
            var testClass = CreateFramework(frameworkTypes);
            var name = "TestValue947022583";
            var isAsync = false;
            var isStatic = false;
            var valueType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword));
            var testValues = new object[] { 1, 2 };
            var result = testClass.CreateTestCaseMethod(new NameResolver(name), new NamingContext("class"), isAsync, isStatic, valueType, testValues);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo(expectedOutput));
        }

        [TestCase(TestFrameworkTypes.NUnit2, "[Test]\r\npublic static void TestValue1606901338()")]
        [TestCase(TestFrameworkTypes.NUnit3, "[Test]\r\npublic static void TestValue1606901338()")]
        [TestCase(TestFrameworkTypes.MsTest, "[TestMethod]\r\npublic void TestValue1606901338()")]
        [TestCase(TestFrameworkTypes.XUnit, "[Fact]\r\npublic static void TestValue1606901338()")]
        public void CanCallCreateTestMethod(TestFrameworkTypes frameworkTypes, string expectedOutput)
        {
            var testClass = CreateFramework(frameworkTypes);
            var name = "TestValue1606901338";
            var isAsync = false;
            var isStatic = true;
            var result = testClass.CreateTestMethod(new NameResolver(name), new NamingContext("class"), isAsync, isStatic);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo(expectedOutput));
        }

        [TestCase(TestFrameworkTypes.NUnit2, "NUnit.Framework")]
        [TestCase(TestFrameworkTypes.NUnit3, "NUnit.Framework")]
        [TestCase(TestFrameworkTypes.MsTest, "Microsoft.VisualStudio.TestTools.UnitTesting")]
        [TestCase(TestFrameworkTypes.XUnit, "Xunit")]
        public void CanCallGetUsings(TestFrameworkTypes frameworkTypes, string expectedOutput)
        {
            var testClass = CreateFramework(frameworkTypes);
            var result = testClass.GetUsings().ToList();
            Assert.That(result.Any(x => x.NormalizeWhitespace().ToFullString().Contains(expectedOutput)));
        }

        [TestCase(TestFrameworkTypes.NUnit2, "RequiresSTA")]
        [TestCase(TestFrameworkTypes.NUnit3, "Apartment(ApartmentState.STA)")]
        [TestCase(TestFrameworkTypes.MsTest, null)]
        [TestCase(TestFrameworkTypes.XUnit, null)]
        public void CanGetSingleThreadedApartmentAttribute(TestFrameworkTypes frameworkTypes, string expected)
        {
            var testClass = CreateFramework(frameworkTypes);
            Assert.That(testClass.SingleThreadedApartmentAttribute?.ToFullString(), Is.EqualTo(expected));
        }

        [TestCase(TestFrameworkTypes.NUnit2, "TestFixture")]
        [TestCase(TestFrameworkTypes.NUnit3, "TestFixture")]
        [TestCase(TestFrameworkTypes.MsTest, "TestClass")]
        [TestCase(TestFrameworkTypes.XUnit, "")]
        public void CanGetTestClassAttribute(TestFrameworkTypes frameworkTypes, string expected)
        {
            var testClass = CreateFramework(frameworkTypes);
            Assert.That(testClass.TestClassAttribute, Is.EqualTo(expected));
        }

        [TestCaseSource(nameof(Targets))]
        public void CannotCallAssertEqualWithNullActual(ITestFramework testClass)
        {
            Assert.Throws<ArgumentNullException>(() => testClass.AssertEqual(default(ExpressionSyntax), Generate.Literal(1), true));
        }

        [TestCaseSource(nameof(Targets))]
        public void CannotCallAssertEqualWithNullExpected(ITestFramework testClass)
        {
            Assert.Throws<ArgumentNullException>(() => testClass.AssertEqual(Generate.Literal(1), default(ExpressionSyntax), false));
        }

        [TestCaseSource(nameof(Targets))]
        public void CannotCallAssertFailWithInvalidMessage(ITestFramework testClass)
        {
            Assert.Throws<ArgumentNullException>(() => testClass.AssertFail(null));
        }

        [TestCaseSource(nameof(Targets))]
        public void CannotCallAssertIsInstanceOfWithNullType(ITestFramework testClass)
        {
            Assert.Throws<ArgumentNullException>(() => testClass.AssertIsInstanceOf(Generate.Literal(1), default(TypeSyntax), true));
        }

        [TestCaseSource(nameof(Targets))]
        public void CannotCallAssertIsInstanceOfWithNullValue(ITestFramework testClass)
        {
            Assert.Throws<ArgumentNullException>(() => testClass.AssertIsInstanceOf(default(ExpressionSyntax), SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), true));
        }

        [TestCaseSource(nameof(Targets))]
        public void CannotCallAssertNotNullWithNullValue(ITestFramework testClass)
        {
            Assert.Throws<ArgumentNullException>(() => testClass.AssertNotNull(default(ExpressionSyntax)));
        }

        [TestCaseSource(nameof(Targets))]
        public void CannotCallAssertThrowsAsyncWithNullExceptionType(ITestFramework testClass)
        {
            Assert.Throws<ArgumentNullException>(() => testClass.AssertThrowsAsync(default(TypeSyntax), Generate.Literal(1)));
        }

        [TestCaseSource(nameof(Targets))]
        public void CannotCallAssertThrowsAsyncWithNullMethodCall(ITestFramework testClass)
        {
            Assert.Throws<ArgumentNullException>(() => testClass.AssertThrowsAsync(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), default(ExpressionSyntax)));
        }

        [TestCaseSource(nameof(Targets))]
        public void CannotCallAssertThrowsWithNullExceptionType(ITestFramework testClass)
        {
            Assert.Throws<ArgumentNullException>(() => testClass.AssertThrows(default(TypeSyntax), Generate.Literal(1)));
        }

        [TestCaseSource(nameof(Targets))]
        public void CannotCallAssertThrowsWithNullMethodCall(ITestFramework testClass)
        {
            Assert.Throws<ArgumentNullException>(() => testClass.AssertThrows(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), default(ExpressionSyntax)));
        }

        [TestCaseSource(nameof(Targets))]
        public void CannotCallCreateSetupMethodWithInvalidTargetTypeName(ITestFramework testClass)
        {
            Assert.Throws<ArgumentNullException>(() => testClass.CreateSetupMethod(null));
        }

        [TestCaseSource(nameof(Targets))]
        public void CannotCallCreateTestCaseMethodWithNullName(ITestFramework testClass)
        {
            Assert.Throws<ArgumentNullException>(() => testClass.CreateTestCaseMethod(null, new NamingContext("class"), true, false, SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), new[] { new object(), new object(), new object() }));
        }

        [TestCaseSource(nameof(Targets))]
        public void CannotCallCreateTestCaseMethodWithNullTestValues(ITestFramework testClass)
        {
            Assert.Throws<ArgumentNullException>(() => testClass.CreateTestCaseMethod(new NameResolver("name"), new NamingContext("class"), false, true, SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), default(IEnumerable<object>)));
        }

        [TestCaseSource(nameof(Targets))]
        public void CannotCallCreateTestCaseMethodWithNullValueType(ITestFramework testClass)
        {
            Assert.Throws<ArgumentNullException>(() => testClass.CreateTestCaseMethod(new NameResolver("name"), new NamingContext("class"), true, false, default(TypeSyntax), new[] { new object(), new object(), new object() }));
        }

        [TestCaseSource(nameof(Targets))]
        public void CannotCallCreateTestMethodWithInvalidName(ITestFramework testClass)
        {
            Assert.Throws<ArgumentNullException>(() => testClass.CreateTestMethod(null, new NamingContext("class"), true, false));
        }

        private static ITestFramework CreateFramework(TestFrameworkTypes frameworkTypes)
        {
            switch (frameworkTypes)
            {
                case TestFrameworkTypes.MsTest:
                    return new MsTestTestFramework();
                case TestFrameworkTypes.NUnit2:
                    return new NUnit2TestFramework();
                case TestFrameworkTypes.NUnit3:
                    return new NUnit3TestFramework();
                case TestFrameworkTypes.XUnit:
                    return new XUnitTestFramework();
                default:
                    throw new ArgumentOutOfRangeException(nameof(frameworkTypes));
            }
        }
    }
}