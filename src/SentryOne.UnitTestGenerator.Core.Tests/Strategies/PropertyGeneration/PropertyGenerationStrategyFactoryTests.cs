namespace Unitverse.Core.Tests.Strategies.PropertyGeneration
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Strategies.PropertyGeneration;

    [TestFixture]
    public class PropertyGenerationStrategyFactoryTests
    {
        private PropertyGenerationStrategyFactory _testClass;
        private IFrameworkSet _frameworkSet;

        [SetUp]
        public void SetUp()
        {
            _frameworkSet = Substitute.For<IFrameworkSet>();
            _testClass = new PropertyGenerationStrategyFactory(_frameworkSet);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new PropertyGenerationStrategyFactory(_frameworkSet);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullFrameworkSet()
        {
            Assert.Throws<ArgumentNullException>(() => new PropertyGenerationStrategyFactory(default(IFrameworkSet)));
        }
    }
}