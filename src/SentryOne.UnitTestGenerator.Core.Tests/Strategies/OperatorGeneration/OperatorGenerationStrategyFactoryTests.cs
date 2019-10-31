namespace SentryOne.UnitTestGenerator.Core.Tests.Strategies.OperatorGeneration
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Strategies.OperatorGeneration;

    [TestFixture]
    public class OperatorGenerationStrategyFactoryTests
    {
        private OperatorGenerationStrategyFactory _testClass;
        private IFrameworkSet _frameworkSet;

        [SetUp]
        public void SetUp()
        {
            _frameworkSet = Substitute.For<IFrameworkSet>();
            _testClass = new OperatorGenerationStrategyFactory(_frameworkSet);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new OperatorGenerationStrategyFactory(_frameworkSet);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullFrameworkSet()
        {
            Assert.Throws<ArgumentNullException>(() => new OperatorGenerationStrategyFactory(default(IFrameworkSet)));
        }
    }
}