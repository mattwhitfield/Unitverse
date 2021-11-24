namespace Unitverse.Core.Tests.Strategies.IndexerGeneration
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Models;
    using Unitverse.Core.Strategies.IndexerGeneration;

    [TestFixture]
    public class ReadOnlyIndexerGenerationStrategyTests
    {
        private ReadOnlyIndexerGenerationStrategy _testClass;
        private IFrameworkSet _frameworkSet;

        [SetUp]
        public void SetUp()
        {
            _frameworkSet = Substitute.For<IFrameworkSet>();
            _testClass = new ReadOnlyIndexerGenerationStrategy(_frameworkSet);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new ReadOnlyIndexerGenerationStrategy(_frameworkSet);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullFrameworkSet()
        {
            Assert.Throws<ArgumentNullException>(() => new ReadOnlyIndexerGenerationStrategy(default(IFrameworkSet)));
        }

        [Test]
        public void CannotCallCanHandleWithNullIndexer()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.CanHandle(default(IIndexerModel), ClassModelProvider.Instance));
        }

        [Test]
        public void CannotCallCanHandleWithNullModel()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.CanHandle(Substitute.For<IIndexerModel>(), default(ClassModel)));
        }

        [Test]
        public void CannotCallCreateWithNullIndexer()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Create(default(IIndexerModel), ClassModelProvider.Instance).Consume());
        }

        [Test]
        public void CannotCallCreateWithNullModel()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Create(Substitute.For<IIndexerModel>(), default(ClassModel)).Consume());
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