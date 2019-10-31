namespace SentryOne.UnitTestGenerator.Core.Tests.Strategies.IndexerGeneration
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Strategies.IndexerGeneration;

    [TestFixture]
    public class IndexerGenerationStrategyFactoryTests
    {
        private IndexerGenerationStrategyFactory _testClass;
        private IFrameworkSet _frameworkSet;

        [SetUp]
        public void SetUp()
        {
            _frameworkSet = Substitute.For<IFrameworkSet>();
            _testClass = new IndexerGenerationStrategyFactory(_frameworkSet);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new IndexerGenerationStrategyFactory(_frameworkSet);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullFrameworkSet()
        {
            Assert.Throws<ArgumentNullException>(() => new IndexerGenerationStrategyFactory(default(IFrameworkSet)));
        }
    }
}