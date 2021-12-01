namespace Unitverse.Core.Tests.Frameworks.Mocking
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks.Mocking;
    using Unitverse.Core.Helpers;

    [TestFixture]
    public class MoqMockingFrameworkTests
    {
        private MoqMockingFramework _testClass;
        private IGenerationContext _context;

        [SetUp]
        public void SetUp()
        {
            _context = Substitute.For<IGenerationContext>();
            _testClass = new MoqMockingFramework(_context);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new MoqMockingFramework(_context);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullContext()
        {
            Assert.Throws<ArgumentNullException>(() => new MoqMockingFramework(default(IGenerationContext)));
        }

        [Test]
        public void CanCallGetUsings()
        {
            var result = _testClass.GetUsings().ToList();
            Assert.That(result.Select(x => x.NormalizeWhitespace().ToFullString()).Any(x => x == "using Moq;"));
        }

        [Test]
        public void CanCallMockInterface()
        {
            var type = SyntaxFactory.ParseTypeName("ISomeInterface");
            var result = _testClass.GetFieldInitializer(type);
            Assert.That(result.NormalizeWhitespace().ToFullString(), Is.EqualTo("new Mock<ISomeInterface>()"));
        }

        [Test]
        public void CannotCallMockInterfaceWithNullType()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.GetFieldInitializer(default(TypeSyntax)));
        }

        [Test]
        public void CannotCallGetFieldTypeWithNullType()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.GetFieldType(default(TypeSyntax)));
        }

        [Test]
        public void CannotCallGetFieldReferenceWithNullFieldReference()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.GetFieldReference(default(ExpressionSyntax)));
        }

        [Test]
        public void CannotCallGetThrowawayReferenceWithNullType()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.GetThrowawayReference(default(TypeSyntax)));
        }
    }
}