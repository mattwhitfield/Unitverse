namespace SentryOne.UnitTestGenerator.Core.Tests.Strategies.IndexerGeneration
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Strategies.IndexerGeneration;

    [TestFixture]
    public class ReadWriteIndexerGenerationStrategyTests
    {
        private ReadWriteIndexerGenerationStrategy _testClass;
        private IFrameworkSet _frameworkSet;

        [SetUp]
        public void SetUp()
        {
            _frameworkSet = Substitute.For<IFrameworkSet>();
            _testClass = new ReadWriteIndexerGenerationStrategy(_frameworkSet);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new ReadWriteIndexerGenerationStrategy(_frameworkSet);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullFrameworkSet()
        {
            Assert.Throws<ArgumentNullException>(() => new ReadWriteIndexerGenerationStrategy(default(IFrameworkSet)));
        }

        [Test]
        public void CannotCallCanHandleWithNullProperty()
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
            Assert.That(_testClass.Priority, Is.EqualTo(1));
        }
    }
}