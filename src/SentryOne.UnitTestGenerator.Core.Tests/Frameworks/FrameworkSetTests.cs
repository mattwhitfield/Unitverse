namespace SentryOne.UnitTestGenerator.Core.Tests.Frameworks
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Helpers;

    [TestFixture]
    public class FrameworkSetTests
    {
        private FrameworkSet _testClass;
        private ITestFramework _testFramework;
        private IMockingFramework _mockingFramework;
        private IGenerationContext _context;
        private string _testTypeNaming;

        [SetUp]
        public void SetUp()
        {
            _testFramework = Substitute.For<ITestFramework>();
            _mockingFramework = Substitute.For<IMockingFramework>();
            _context = Substitute.For<IGenerationContext>();
            _testTypeNaming = "TestValue455103231";
            _testClass = new FrameworkSet(_testFramework, _mockingFramework, _context, _testTypeNaming);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new FrameworkSet(_testFramework, _mockingFramework, _context, _testTypeNaming);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullTestFramework()
        {
            Assert.Throws<ArgumentNullException>(() => new FrameworkSet(default(ITestFramework), Substitute.For<IMockingFramework>(), Substitute.For<IGenerationContext>(), "TestValue1808135505"));
        }

        [Test]
        public void CannotConstructWithNullMockingFramework()
        {
            Assert.Throws<ArgumentNullException>(() => new FrameworkSet(Substitute.For<ITestFramework>(), default(IMockingFramework), Substitute.For<IGenerationContext>(), "TestValue888012024"));
        }

        [Test]
        public void CannotConstructWithNullContext()
        {
            Assert.Throws<ArgumentNullException>(() => new FrameworkSet(Substitute.For<ITestFramework>(), Substitute.For<IMockingFramework>(), default(IGenerationContext), "TestValue1975092699"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidTestTypeNaming(string value)
        {
            Assert.Throws<ArgumentNullException>(() => new FrameworkSet(Substitute.For<ITestFramework>(), Substitute.For<IMockingFramework>(), Substitute.For<IGenerationContext>(), value));
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
        public void TestTypeNamingIsInitializedCorrectly()
        {
            Assert.That(_testClass.TestTypeNaming, Is.EqualTo(_testTypeNaming));
        }
    }
}