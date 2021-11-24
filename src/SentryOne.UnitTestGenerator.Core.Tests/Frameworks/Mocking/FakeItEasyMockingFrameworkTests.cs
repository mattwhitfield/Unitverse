namespace SentryOne.UnitTestGenerator.Core.Tests.Frameworks.Mocking
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NSubstitute;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Frameworks.Mocking;
    using SentryOne.UnitTestGenerator.Core.Helpers;

    [TestFixture]
    public class FakeItEasyMockingFrameworkTests
    {
        private FakeItEasyMockingFramework _testClass;
        private IGenerationContext _context;

        [SetUp]
        public void SetUp()
        {
            _context = Substitute.For<IGenerationContext>();
            _testClass = new FakeItEasyMockingFramework(_context);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new FakeItEasyMockingFramework(_context);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullContext()
        {
            Assert.Throws<ArgumentNullException>(() => new FakeItEasyMockingFramework(default(IGenerationContext)));
        }

        [Test]
        public void CanCallGetUsings()
        {
            var result = _testClass.GetUsings().ToList();
            Assert.That(result.Select(x => x.NormalizeWhitespace().ToFullString()).Any(x => x == "using FakeItEasy;"));
        }

        [Test]
        public void CanCallMockInterface()
        {
            var type = SyntaxFactory.ParseTypeName("ISomeInterface");
            var result = _testClass.GetFieldInitializer(type);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo("A.Fake<ISomeInterface>()"));
        }

        [Test]
        public void CannotCallMockInterfaceWithNullType()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.GetFieldInitializer(default(TypeSyntax)));
        }
    }
}