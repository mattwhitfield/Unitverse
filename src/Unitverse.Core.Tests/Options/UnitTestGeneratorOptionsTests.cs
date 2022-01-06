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
        private bool _statisticsCollectionEnabled;

        [SetUp]
        public void SetUp()
        {
            _generationOptions = Substitute.For<IGenerationOptions>();
            _namingOptions = Substitute.For<INamingOptions>();
            _statisticsCollectionEnabled = true;
            _testClass = new UnitTestGeneratorOptions(_generationOptions, _namingOptions, _statisticsCollectionEnabled);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new UnitTestGeneratorOptions(_generationOptions, _namingOptions, _statisticsCollectionEnabled);
            instance.Should().NotBeNull();
        }

        [Test]
        public void CannotConstructWithNullGenerationOptions()
        {
            FluentActions.Invoking(() => new UnitTestGeneratorOptions(default(IGenerationOptions), Substitute.For<INamingOptions>(), true)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotConstructWithNullNamingOptions()
        {
            FluentActions.Invoking(() => new UnitTestGeneratorOptions(Substitute.For<IGenerationOptions>(), default(INamingOptions), true)).Should().Throw<ArgumentNullException>();
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
        public void StatisticsCollectionEnabledIsInitializedCorrectly()
        {
            _testClass.StatisticsCollectionEnabled.Should().Be(_statisticsCollectionEnabled);
        }
    }
}