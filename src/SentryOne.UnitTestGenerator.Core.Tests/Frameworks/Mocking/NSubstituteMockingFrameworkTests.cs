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
    using SentryOne.UnitTestGenerator.Core.Options;

    [TestFixture]
    public class NSubstituteMockingFrameworkTests
    {
        private NSubstituteMockingFramework _testClass;
        private IGenerationContext _context;

        [SetUp]
        public void SetUp()
        {
            _context = Substitute.For<IGenerationContext>();
            _testClass = new NSubstituteMockingFramework(_context);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new NSubstituteMockingFramework(_context);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullContext()
        {
            Assert.Throws<ArgumentNullException>(() => new NSubstituteMockingFramework(default(IGenerationContext)));
        }

        [Test]
        public void CanCallGetUsings()
        {
            var result = _testClass.GetUsings().ToList();
            Assert.That(result.Select(x => x.NormalizeWhitespace().ToFullString()).Any(x => x == "using NSubstitute;"));
        }

        [Test]
        public void CanCallMockInterface()
        {
            var type = SyntaxFactory.ParseTypeName("ISomeInterface");
            var result = _testClass.MockInterface(type);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo("Substitute.For<ISomeInterface>()"));
        }

        [Test]
        public void CannotCallMockInterfaceWithNullType()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.MockInterface(default(TypeSyntax)));
        }
    }
}