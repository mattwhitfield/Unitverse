namespace Unitverse.Core.Tests.Strategies.ClassLevelGeneration
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Strategies.ClassLevelGeneration;

    [TestFixture]
    public class ClassLevelGenerationStrategyFactoryTests
    {
        private ClassLevelGenerationStrategyFactory _testClass;
        private IFrameworkSet _frameworkSet;

        [SetUp]
        public void SetUp()
        {
            _frameworkSet = Substitute.For<IFrameworkSet>();
            _testClass = new ClassLevelGenerationStrategyFactory(_frameworkSet);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new ClassLevelGenerationStrategyFactory(_frameworkSet);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullFrameworkSet()
        {
            Assert.Throws<ArgumentNullException>(() => new ClassLevelGenerationStrategyFactory(default(IFrameworkSet)));
        }
    }
}