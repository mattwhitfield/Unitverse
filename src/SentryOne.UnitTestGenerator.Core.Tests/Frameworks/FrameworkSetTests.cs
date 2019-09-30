namespace SentryOne.UnitTestGenerator.Core.Tests.Frameworks
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Options;

    [TestFixture]
    public class FrameworkSetTests
    {
        private FrameworkSet _testClass;
        private ITestFramework _testFramework;
        private IMockingFramework _mockingFramework;
        private IGenerationContext _context;
        private IUnitTestGeneratorOptions _options;

        [SetUp]
        public void SetUp()
        {
            _testFramework = Substitute.For<ITestFramework>();
            _mockingFramework = Substitute.For<IMockingFramework>();
            _context = Substitute.For<IGenerationContext>();
            _options = Substitute.For<IUnitTestGeneratorOptions>();
            _testClass = new FrameworkSet(_testFramework, _mockingFramework, _context, _options);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new FrameworkSet(_testFramework, _mockingFramework, _context, _options);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullTestFramework()
        {
            Assert.Throws<ArgumentNullException>(() => new FrameworkSet(default(ITestFramework), Substitute.For<IMockingFramework>(), Substitute.For<IGenerationContext>(), Substitute.For<IUnitTestGeneratorOptions>()));
        }

        [Test]
        public void CannotConstructWithNullMockingFramework()
        {
            Assert.Throws<ArgumentNullException>(() => new FrameworkSet(Substitute.For<ITestFramework>(), default(IMockingFramework), Substitute.For<IGenerationContext>(), Substitute.For<IUnitTestGeneratorOptions>()));
        }

        [Test]
        public void CannotConstructWithNullContext()
        {
            Assert.Throws<ArgumentNullException>(() => new FrameworkSet(Substitute.For<ITestFramework>(), Substitute.For<IMockingFramework>(), default(IGenerationContext), Substitute.For<IUnitTestGeneratorOptions>()));
        }

        [Test]
        public void CannotConstructWithNullOptions()
        {
            Assert.Throws<ArgumentNullException>(() => new FrameworkSet(Substitute.For<ITestFramework>(), Substitute.For<IMockingFramework>(), Substitute.For<IGenerationContext>(), default(IUnitTestGeneratorOptions)));
        }

        [Test]
        public void TestFrameworkIsInitializedCorrectly()
        {
            Assert.That(_testClass.TestFramework, Is.EqualTo(_testFramework));
        }

        [Test]
        public void MockingFrameworkIsInitializedCorrectly()
        {
            Assert.That(_testClass.MockingFramework, Is.EqualTo(_mockingFramework));
        }

        [Test]
        public void ContextIsInitializedCorrectly()
        {
            Assert.That(_testClass.Context, Is.EqualTo(_context));
        }

        [Test]
        public void OptionsIsInitializedCorrectly()
        {
            Assert.That(_testClass.Options, Is.EqualTo(_options));
        }
    }
}