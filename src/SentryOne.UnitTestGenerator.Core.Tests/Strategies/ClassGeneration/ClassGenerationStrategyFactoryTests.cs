namespace SentryOne.UnitTestGenerator.Core.Tests.Strategies.ClassGeneration
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Strategies.ClassGeneration;

    [TestFixture]
    public class ClassGenerationStrategyFactoryTests
    {
        private ClassGenerationStrategyFactory _testClass;
        private IFrameworkSet _frameworkSet;

        [SetUp]
        public void SetUp()
        {
            _frameworkSet = Substitute.For<IFrameworkSet>();
            _testClass = new ClassGenerationStrategyFactory(_frameworkSet);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new ClassGenerationStrategyFactory(_frameworkSet);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullFrameworkSet()
        {
            Assert.Throws<ArgumentNullException>(() => new ClassGenerationStrategyFactory(default(IFrameworkSet)));
        }

        [Test]
        public void CannotCallCreateForWithNullModel()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.CreateFor(default(ClassModel)));
        }
    }
}