namespace SentryOne.UnitTestGenerator.Core.Tests.Strategies.InterfaceGeneration
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Strategies.InterfaceGeneration;

    [TestFixture]
    public class EnumerableGenerationStrategyTests
    {
        private EnumerableGenerationStrategy _testClass;
        private IFrameworkSet _frameworkSet;

        [SetUp]
        public void SetUp()
        {
            _frameworkSet = Substitute.For<IFrameworkSet>();
            _testClass = new EnumerableGenerationStrategy(_frameworkSet);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new EnumerableGenerationStrategy(_frameworkSet);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullFrameworkSet()
        {
            Assert.Throws<ArgumentNullException>(() => new EnumerableGenerationStrategy(default(IFrameworkSet)));
        }

        [Test]
        public void CannotCallCreateWithNullClassModel()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Create(default(ClassModel), ClassModelProvider.Instance).Consume());
        }

        [Test]
        public void CannotCallCreateWithNullModel()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Create(ClassModelProvider.Instance, default(ClassModel)).Consume());
        }

        [Test]
        public void CanGetSupportedInterfaceName()
        {
            Assert.That(_testClass.SupportedInterfaceName, Is.EqualTo("System.Collections.Generic.IEnumerable"));
        }
    }
}