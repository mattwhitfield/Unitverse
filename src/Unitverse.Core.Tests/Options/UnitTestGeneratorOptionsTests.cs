namespace Unitverse.Core.Tests.Options
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Options;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class UnitTestGeneratorOptionsTests
    {
        private UnitTestGeneratorOptions _testClass;
        private IGenerationOptions _generationOptions;
        private INamingOptions _namingOptions;
        private IStrategyOptions _strategyOptions;
        private bool _statisticsCollectionEnabled;
        private Dictionary<string, string> _membersSetByFilename;

        [SetUp]
        public void SetUp()
        {
            _generationOptions = Substitute.For<IGenerationOptions>();
            _namingOptions = Substitute.For<INamingOptions>();
            _strategyOptions = Substitute.For<IStrategyOptions>();
            _statisticsCollectionEnabled = true;
            _membersSetByFilename = new Dictionary<string, string>();
            _membersSetByFilename["A"] = "A.File";
            _membersSetByFilename["B"] = "B.File";
            _membersSetByFilename["Other"] = "A.File";
            _testClass = new UnitTestGeneratorOptions(_generationOptions, _namingOptions, _strategyOptions, _statisticsCollectionEnabled, _membersSetByFilename);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new UnitTestGeneratorOptions(_generationOptions, _namingOptions, _strategyOptions, _statisticsCollectionEnabled, _membersSetByFilename);

            // Assert
            instance.Should().NotBeNull();
        }

        [Test]
        public void CannotConstructWithNullGenerationOptions()
        {
            FluentActions.Invoking(() => new UnitTestGeneratorOptions(default(IGenerationOptions), Substitute.For<INamingOptions>(), Substitute.For<IStrategyOptions>(), false, new Dictionary<string, string>())).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotConstructWithNullNamingOptions()
        {
            FluentActions.Invoking(() => new UnitTestGeneratorOptions(Substitute.For<IGenerationOptions>(), default(INamingOptions), Substitute.For<IStrategyOptions>(), true, new Dictionary<string, string>())).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotConstructWithNullStrategyOptions()
        {
            FluentActions.Invoking(() => new UnitTestGeneratorOptions(Substitute.For<IGenerationOptions>(), Substitute.For<INamingOptions>(), default(IStrategyOptions), false, new Dictionary<string, string>())).Should().Throw<ArgumentNullException>();
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

        [Test]
        public void CannotConstructWithNullMembersSetByFilename()
        {
            FluentActions.Invoking(() => new UnitTestGeneratorOptions(Substitute.For<IGenerationOptions>(), Substitute.For<INamingOptions>(), Substitute.For<IStrategyOptions>(), false, default(Dictionary<string, string>))).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CanCallGetFieldSourceFileName()
        {
            // Assert
            _testClass.GetFieldSourceFileName("A").Should().Be("A.File");
            _testClass.GetFieldSourceFileName("B").Should().Be("B.File");
            _testClass.GetFieldSourceFileName("C").Should().Be(null);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallGetFieldSourceFileNameWithInvalidFieldName(string value)
        {
            FluentActions.Invoking(() => _testClass.GetFieldSourceFileName(value)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CanGetSourceCounts()
        {
            // Act
            var sourceCounts = _testClass.SourceCounts.ToList();

            // Assert
            sourceCounts.Should().Contain(x => x.Key == "A.File" && x.Value == 2);
            sourceCounts.Should().Contain(x => x.Key == "B.File" && x.Value == 1);
        }
    }
}