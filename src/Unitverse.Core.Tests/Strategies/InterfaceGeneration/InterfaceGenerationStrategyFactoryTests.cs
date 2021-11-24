namespace Unitverse.Core.Tests.Strategies.InterfaceGeneration
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Strategies.InterfaceGeneration;

    [TestFixture]
    public class InterfaceGenerationStrategyFactoryTests
    {
        private InterfaceGenerationStrategyFactory _testClass;
        private IFrameworkSet _frameworkSet;

        [SetUp]
        public void SetUp()
        {
            _frameworkSet = Substitute.For<IFrameworkSet>();
            _testClass = new InterfaceGenerationStrategyFactory(_frameworkSet);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new InterfaceGenerationStrategyFactory(_frameworkSet);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullFrameworkSet()
        {
            Assert.Throws<ArgumentNullException>(() => new InterfaceGenerationStrategyFactory(default(IFrameworkSet)));
        }
    }
}