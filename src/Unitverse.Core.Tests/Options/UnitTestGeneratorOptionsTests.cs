namespace Unitverse.Core.Tests.Options
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Options;
    using FluentAssertions;

    [TestFixture]
    public class UnitTestGeneratorOptionsTests
    {
        private UnitTestGeneratorOptions _testClass;
        private IGenerationOptions _generationOptions;
        private INamingOptions _namingOptions;
        private IStrategyOptions _strategyOptions;
        private bool _statisticsCollectionEnabled;

        [SetUp]
        public void SetUp()
        {
            _generationOptions = Substitute.For<IGenerationOptions>();
            _namingOptions = Substitute.For<INamingOptions>();
            _strategyOptions = Substitute.For<IStrategyOptions>();
            _statisticsCollectionEnabled = true;
            _testClass = new UnitTestGeneratorOptions(_generationOptions, _namingOptions, _strategyOptions, _statisticsCollectionEnabled);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new UnitTestGeneratorOptions(_generationOptions, _namingOptions, _strategyOptions, _statisticsCollectionEnabled);
            instance.Should().NotBeNull();
        }

        [Test]
        public void CannotConstructWithNullGenerationOptions()
        {
            FluentActions.Invoking(() => new UnitTestGeneratorOptions(default(IGenerationOptions), Substitute.For<INamingOptions>(), Substitute.For<IStrategyOptions>(), true)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotConstructWithNullNamingOptions()
        {
            FluentActions.Invoking(() => new UnitTestGeneratorOptions(Substitute.For<IGenerationOptions>(), default(INamingOptions), Substitute.For<IStrategyOptions>(), true)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotConstructWithNullStrategyOptions()
        {
            FluentActions.Invoking(() => new UnitTestGeneratorOptions(Substitute.For<IGenerationOptions>(), Substitute.For<INamingOptions>(), default(IStrategyOptions), true)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void GenerationOptionsIsInitializedCorrectly()
        {
            _testClass.GenerationOptions.Should().BeSameAs(_generationOptions);
        }

        [Test]
        public void NamingOptionsIsInitializedCorrectly()
        {
            _testClass.NamingOptions.Should().BeSameAs(_namingOptions);
        }

        [Test]
        public void StrategyOptionsIsInitializedCorrectly()
        {
            _testClass.StrategyOptions.Should().BeSameAs(_strategyOptions);
        }

        [Test]
        public void StatisticsCollectionEnabledIsInitializedCorrectly()
        {
            _testClass.StatisticsCollectionEnabled.Should().Be(_statisticsCollectionEnabled);
        }
    }
}