namespace SentryOne.UnitTestGenerator.Core.Tests.Strategies.ClassLevelGeneration
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Strategies.ClassLevelGeneration;

    [TestFixture]
    public class CanConstructNoConstructorGenerationStrategyTests
    {
        private CanConstructNoConstructorGenerationStrategy _testClass;
        private IFrameworkSet _frameworkSet;

        [SetUp]
        public void SetUp()
        {
            _frameworkSet = Substitute.For<IFrameworkSet>();
            _testClass = new CanConstructNoConstructorGenerationStrategy(_frameworkSet);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new CanConstructNoConstructorGenerationStrategy(_frameworkSet);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullFrameworkSet()
        {
            Assert.Throws<ArgumentNullException>(() => new CanConstructNoConstructorGenerationStrategy(default(IFrameworkSet)));
        }

        [Test]
        public void CannotCallCanHandleWithNullMethod()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.CanHandle(default(ClassModel), ClassModelProvider.Instance));
        }

        [Test]
        public void CannotCallCanHandleWithNullModel()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.CanHandle(ClassModelProvider.Instance, default(ClassModel)));
        }

        [Test]
        public void CannotCallCreateWithNullMethod()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Create(default(ClassModel), ClassModelProvider.Instance).Consume());
        }

        [Test]
        public void CannotCallCreateWithNullModel()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Create(ClassModelProvider.Instance, default(ClassModel)).Consume());
        }

        [Test]
        public void CanGetIsExclusive()
        {
            Assert.That(_testClass.IsExclusive, Is.EqualTo(false));
        }

        [Test]
        public void CanGetPriority()
        {
            Assert.That(_testClass.Priority, Is.EqualTo(1));
        }
    }
}