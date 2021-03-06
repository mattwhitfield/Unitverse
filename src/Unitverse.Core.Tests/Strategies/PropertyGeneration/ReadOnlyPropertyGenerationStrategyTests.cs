namespace Unitverse.Core.Tests.Strategies.PropertyGeneration
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using Unitverse.Core.Strategies.PropertyGeneration;

    [TestFixture]
    public class ReadOnlyPropertyGenerationStrategyTests
    {
        private ReadOnlyPropertyGenerationStrategy _testClass;
        private IFrameworkSet _frameworkSet;

        [SetUp]
        public void SetUp()
        {
            _frameworkSet = Substitute.For<IFrameworkSet>();
            _testClass = new ReadOnlyPropertyGenerationStrategy(_frameworkSet);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new ReadOnlyPropertyGenerationStrategy(_frameworkSet);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullFrameworkSet()
        {
            Assert.Throws<ArgumentNullException>(() => new ReadOnlyPropertyGenerationStrategy(default(IFrameworkSet)));
        }

        [Test]
        public void CannotCallCanHandleWithNullProperty()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.CanHandle(default(IPropertyModel), ClassModelProvider.Instance));
        }

        [Test]
        public void CannotCallCanHandleWithNullModel()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.CanHandle(Substitute.For<IPropertyModel>(), default(ClassModel)));
        }

        [Test]
        public void CannotCallCreateWithNullProperty()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Create(default(IPropertyModel), ClassModelProvider.Instance, new NamingContext("class")).Consume());
        }

        [Test]
        public void CannotCallCreateWithNullModel()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Create(Substitute.For<IPropertyModel>(), default(ClassModel), new NamingContext("class")).Consume());
        }

        [Test]
        public void CanGetIsExclusive()
        {
            Assert.That(_testClass.IsExclusive, Is.EqualTo(false));
        }

        [Test]
        public void CanGetPriority()
        {
            Assert.That(_testClass.Priority, Is.EqualTo(2));
        }
    }
}