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
    public class RhinoMocksMockingFrameworkTests
    {
        private RhinoMocksMockingFramework _testClass;
        private IGenerationContext _context;

        [SetUp]
        public void SetUp()
        {
            _context = Substitute.For<IGenerationContext>();
            _testClass = new RhinoMocksMockingFramework(_context);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new RhinoMocksMockingFramework(_context);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullContext()
        {
            Assert.Throws<ArgumentNullException>(() => new RhinoMocksMockingFramework(default(IGenerationContext)));
        }

        [Test]
        public void CanCallGetUsings()
        {
            var result = _testClass.GetUsings().ToList();
            Assert.That(result.Select(x => x.NormalizeWhitespace().ToFullString()).Any(x => x == "using Rhino.Mocks;"));
        }

        [Test]
        public void CanCallMockInterface()
        {
            var type = SyntaxFactory.ParseTypeName("ISomeInterface");
            var result = _testClass.MockInterface(type);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo("MockRepository.GenerateStub<ISomeInterface>()"));
        }

        [Test]
        public void CannotCallMockInterfaceWithNullType()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.MockInterface(default(TypeSyntax)));
        }
    }
}