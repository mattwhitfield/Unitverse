namespace Unitverse.Core.Tests.Frameworks
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;
    using FluentAssertions;

    [TestFixture]
    public class FrameworkSetTests
    {
        private FrameworkSet _testClass;
        private ITestFramework _testFramework;
        private IMockingFramework _mockingFramework;
        private IAssertionFramework _assertionFramework;
        private IGenerationContext _context;
        private string _testTypeNaming;
        private IUnitTestGeneratorOptions _options;
        private INamingProvider _namingProvider;

        [SetUp]
        public void SetUp()
        {
            _testFramework = Substitute.For<ITestFramework>();
            _mockingFramework = Substitute.For<IMockingFramework>();
            _assertionFramework = Substitute.For<IAssertionFramework>();
            _namingProvider = Substitute.For<INamingProvider>();
            _context = Substitute.For<IGenerationContext>();
            _testTypeNaming = "TestValue455103231";
            _options = Substitute.For<IUnitTestGeneratorOptions>();

            _testClass = new FrameworkSet(_testFramework, _mockingFramework, _assertionFramework, _namingProvider, _context, _testTypeNaming, _options);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new FrameworkSet(_testFramework, _mockingFramework, _assertionFramework, _namingProvider, _context, _testTypeNaming, _options);
            instance.Should().NotBeNull();
        }

        [Test]
        public void CannotConstructWithNullTestFramework()
        {
            FluentActions.Invoking(() => new FrameworkSet(default(ITestFramework), Substitute.For<IMockingFramework>(), Substitute.For<IAssertionFramework>(), Substitute.For<INamingProvider>(), Substitute.For<IGenerationContext>(), "TestValue800970825", Substitute.For<IUnitTestGeneratorOptions>())).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotConstructWithNullMockingFramework()
        {
            FluentActions.Invoking(() => new FrameworkSet(Substitute.For<ITestFramework>(), default(IMockingFramework), Substitute.For<IAssertionFramework>(), Substitute.For<INamingProvider>(), Substitute.For<IGenerationContext>(), "TestValue1430299096", Substitute.For<IUnitTestGeneratorOptions>())).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotConstructWithNullAssertionFramework()
        {
            FluentActions.Invoking(() => new FrameworkSet(Substitute.For<ITestFramework>(), Substitute.For<IMockingFramework>(), default(IAssertionFramework), Substitute.For<INamingProvider>(), Substitute.For<IGenerationContext>(), "TestValue1900892022", Substitute.For<IUnitTestGeneratorOptions>())).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotConstructWithNullNamingProvider()
        {
            FluentActions.Invoking(() => new FrameworkSet(Substitute.For<ITestFramework>(), Substitute.For<IMockingFramework>(), Substitute.For<IAssertionFramework>(), default(INamingProvider), Substitute.For<IGenerationContext>(), "TestValue1239307533", Substitute.For<IUnitTestGeneratorOptions>())).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotConstructWithNullContext()
        {
            FluentActions.Invoking(() => new FrameworkSet(Substitute.For<ITestFramework>(), Substitute.For<IMockingFramework>(), Substitute.For<IAssertionFramework>(), Substitute.For<INamingProvider>(), default(IGenerationContext), "TestValue1680076439", Substitute.For<IUnitTestGeneratorOptions>())).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidTestTypeNaming(string value)
        {
            FluentActions.Invoking(() => new FrameworkSet(Substitute.For<ITestFramework>(), Substitute.For<IMockingFramework>(), Substitute.For<IAssertionFramework>(), Substitute.For<INamingProvider>(), Substitute.For<IGenerationContext>(), value, Substitute.For<IUnitTestGeneratorOptions>())).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotConstructWithNullOptions()
        {
            FluentActions.Invoking(() => new FrameworkSet(Substitute.For<ITestFramework>(), Substitute.For<IMockingFramework>(), Substitute.For<IAssertionFramework>(), Substitute.For<INamingProvider>(), Substitute.For<IGenerationContext>(), "TestValue969625404", default(IUnitTestGeneratorOptions))).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void TestFrameworkIsInitializedCorrectly()
        {
            _testClass.TestFramework.Should().BeSameAs(_testFramework);
        }

        [Test]
        public void MockingFrameworkIsInitializedCorrectly()
        {
            _testClass.MockingFramework.Should().BeSameAs(_mockingFramework);
        }

        [Test]
        public void AssertionFrameworkIsInitializedCorrectly()
        {
            _testClass.AssertionFramework.Should().BeSameAs(_assertionFramework);
        }

        [Test]
        public void ContextIsInitializedCorrectly()
        {
            _testClass.Context.Should().BeSameAs(_context);
        }

        [Test]
        public void TestTypeNamingIsInitializedCorrectly()
        {
            _testClass.TestTypeNaming.Should().BeSameAs(_testTypeNaming);
        }

        [Test]
        public void NamingProviderIsInitializedCorrectly()
        {
            _testClass.NamingProvider.Should().BeSameAs(_namingProvider);
        }

        [Test]
        public void OptionsIsInitializedCorrectly()
        {
            _testClass.Options.Should().BeSameAs(_options);
        }
    }
}