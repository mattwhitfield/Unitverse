namespace Unitverse.Core.Tests.Frameworks
{
    using Unitverse.Core.Frameworks;
    using System;
    using NUnit.Framework;
    using FluentAssertions;
    using NSubstitute;
    using Unitverse.Core.Models;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis;
    using Unitverse.Core.Options;
    using Unitverse.Core.Helpers;

    [TestFixture]
    public class FrameworkSetTests
    {
        private FrameworkSet _testClass;
        private ITestFramework _testFramework;
        private IMockingFramework _mockingFramework;
        private IAssertionFramework _assertionFramework;
        private INamingProvider _namingProvider;
        private IGenerationContext _context;
        private IUnitTestGeneratorOptions _options;

        [SetUp]
        public void SetUp()
        {
            _testFramework = Substitute.For<ITestFramework>();
            _mockingFramework = Substitute.For<IMockingFramework>();
            _assertionFramework = Substitute.For<IAssertionFramework>();
            _namingProvider = Substitute.For<INamingProvider>();
            _context = Substitute.For<IGenerationContext>();
            _options = Substitute.For<IUnitTestGeneratorOptions>();
            _testClass = new FrameworkSet(_testFramework, _mockingFramework, _assertionFramework, _namingProvider, _context, _options);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new FrameworkSet(_testFramework, _mockingFramework, _assertionFramework, _namingProvider, _context, _options);
            
            // Assert
            instance.Should().NotBeNull();
        }

        [Test]
        public void CannotConstructWithNullTestFramework()
        {
            FluentActions.Invoking(() => new FrameworkSet(default(ITestFramework), Substitute.For<IMockingFramework>(), Substitute.For<IAssertionFramework>(), Substitute.For<INamingProvider>(), Substitute.For<IGenerationContext>(), Substitute.For<IUnitTestGeneratorOptions>())).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotConstructWithNullMockingFramework()
        {
            FluentActions.Invoking(() => new FrameworkSet(Substitute.For<ITestFramework>(), default(IMockingFramework), Substitute.For<IAssertionFramework>(), Substitute.For<INamingProvider>(), Substitute.For<IGenerationContext>(), Substitute.For<IUnitTestGeneratorOptions>())).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotConstructWithNullAssertionFramework()
        {
            FluentActions.Invoking(() => new FrameworkSet(Substitute.For<ITestFramework>(), Substitute.For<IMockingFramework>(), default(IAssertionFramework), Substitute.For<INamingProvider>(), Substitute.For<IGenerationContext>(), Substitute.For<IUnitTestGeneratorOptions>())).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotConstructWithNullNamingProvider()
        {
            FluentActions.Invoking(() => new FrameworkSet(Substitute.For<ITestFramework>(), Substitute.For<IMockingFramework>(), Substitute.For<IAssertionFramework>(), default(INamingProvider), Substitute.For<IGenerationContext>(), Substitute.For<IUnitTestGeneratorOptions>())).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotConstructWithNullContext()
        {
            FluentActions.Invoking(() => new FrameworkSet(Substitute.For<ITestFramework>(), Substitute.For<IMockingFramework>(), Substitute.For<IAssertionFramework>(), Substitute.For<INamingProvider>(), default(IGenerationContext), Substitute.For<IUnitTestGeneratorOptions>())).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotConstructWithNullOptions()
        {
            FluentActions.Invoking(() => new FrameworkSet(Substitute.For<ITestFramework>(), Substitute.For<IMockingFramework>(), Substitute.For<IAssertionFramework>(), Substitute.For<INamingProvider>(), Substitute.For<IGenerationContext>(), default(IUnitTestGeneratorOptions))).Should().Throw<ArgumentNullException>();
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
        public void NamingProviderIsInitializedCorrectly()
        {
            _testClass.NamingProvider.Should().BeSameAs(_namingProvider);
        }

        [Test]
        public void ContextIsInitializedCorrectly()
        {
            _testClass.Context.Should().BeSameAs(_context);
        }

        [Test]
        public void OptionsIsInitializedCorrectly()
        {
            _testClass.Options.Should().BeSameAs(_options);
        }
    }
}