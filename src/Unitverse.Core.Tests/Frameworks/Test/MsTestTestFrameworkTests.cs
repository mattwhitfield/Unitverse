namespace Unitverse.Core.Tests.Frameworks.Test
{
    using NUnit.Framework;
    using Unitverse.Core.Frameworks.Test;
    using System;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.CSharp;
    using NSubstitute;
    using Unitverse.Core.Options;

    [TestFixture]
    public class MsTestTestFrameworkTests
    {
        private MsTestTestFramework _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new MsTestTestFramework(Substitute.For<IUnitTestGeneratorOptions>());
        }

        [Test]
        public void CannotCallAssertGreaterThanWithNullActual()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.AssertGreaterThan(default(ExpressionSyntax), SyntaxFactory.IdentifierName("name")));
        }

        [Test]
        public void CannotCallAssertGreaterThanWithNullExpected()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.AssertGreaterThan(SyntaxFactory.IdentifierName("name"), default(ExpressionSyntax)));
        }

        [Test]
        public void CannotCallAssertLessThanWithNullActual()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.AssertLessThan(default(ExpressionSyntax), SyntaxFactory.IdentifierName("name")));
        }

        [Test]
        public void CannotCallAssertLessThanWithNullExpected()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.AssertLessThan(SyntaxFactory.IdentifierName("name"), default(ExpressionSyntax)));
        }
    }
}