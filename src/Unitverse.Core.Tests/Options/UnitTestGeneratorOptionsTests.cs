namespace Unitverse.Core.Tests.Options
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Options;

    [TestFixture]
    public class UnitTestGeneratorOptionsTests
    {
        private UnitTestGeneratorOptions _testClass;
        private IGenerationOptions _generationOptions;
        private INamingOptions _namingOptions;
        private IStrategyOptions _strategyOptions;
        private bool _statisticsCollectionEnabled;
        private Dictionary<string, ConfigurationSource> _membersSetByFilename;
        private Dictionary<string, string> _projectMappings;
        private string _solutionPath;
        private string _sourceProjectPath;
        private Dictionary<string, ConfigurationSource> _configurationSources;

        [SetUp]
        public void SetUp()
        {
            _configurationSources = new Dictionary<string, ConfigurationSource>();
            _generationOptions = Substitute.For<IGenerationOptions>();
            _namingOptions = Substitute.For<INamingOptions>();
            _strategyOptions = Substitute.For<IStrategyOptions>();
            _statisticsCollectionEnabled = true;
            _membersSetByFilename = new Dictionary<string, ConfigurationSource>();
            _membersSetByFilename["A"] = new ConfigurationSource(ConfigurationSourceType.ConfigurationFile, "A.File");
            _membersSetByFilename["B"] = new ConfigurationSource(ConfigurationSourceType.ConfigurationFile, "B.File");
            _membersSetByFilename["Other"] = new ConfigurationSource(ConfigurationSourceType.ConfigurationFile, "A.File");
            _projectMappings = new Dictionary<string, string> { { "A", "B" }, { "C", "D" } };
            _solutionPath = "TestValue243761049";
            _sourceProjectPath = "TestValue902166532";
            _testClass = new UnitTestGeneratorOptions(_generationOptions, _namingOptions, _strategyOptions, _statisticsCollectionEnabled, _membersSetByFilename, _projectMappings, _solutionPath, _sourceProjectPath);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new UnitTestGeneratorOptions(_generationOptions, _namingOptions, _strategyOptions, _statisticsCollectionEnabled, _configurationSources, _projectMappings, _solutionPath, _sourceProjectPath);

            // Assert
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullGenerationOptions()
        {
            Assert.Throws<ArgumentNullException>(() => new UnitTestGeneratorOptions(default(IGenerationOptions), Substitute.For<INamingOptions>(), Substitute.For<IStrategyOptions>(), false, new Dictionary<string, ConfigurationSource>(), new Dictionary<string, string>(), "TestValue846045550", "TestValue1119157727"));
        }

        [Test]
        public void CannotConstructWithNullNamingOptions()
        {
            Assert.Throws<ArgumentNullException>(() => new UnitTestGeneratorOptions(Substitute.For<IGenerationOptions>(), default(INamingOptions), Substitute.For<IStrategyOptions>(), true, new Dictionary<string, ConfigurationSource>(), new Dictionary<string, string>(), "TestValue968781388", "TestValue1505497627"));
        }

        [Test]
        public void CannotConstructWithNullStrategyOptions()
        {
            Assert.Throws<ArgumentNullException>(() => new UnitTestGeneratorOptions(Substitute.For<IGenerationOptions>(), Substitute.For<INamingOptions>(), default(IStrategyOptions), false, new Dictionary<string, ConfigurationSource>(), new Dictionary<string, string>(), "TestValue230891483", "TestValue1399860540"));
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
        public void CanCallGetFieldSourceFileName()
        {
            // Assert
            _testClass.GetFieldSource("A").FileName.Should().Be("A.File");
            _testClass.GetFieldSource("B").FileName.Should().Be("B.File");
            _testClass.GetFieldSource("C").FileName.Should().Be(null);
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

        [Test]
        public void ProjectMappingsIsInitializedCorrectly()
        {
            _testClass.ProjectMappings.Should().BeEquivalentTo(_projectMappings);
        }

        [Test]
        public void SolutionPathIsInitializedCorrectly()
        {
            _testClass.SolutionPath.Should().Be(_solutionPath);
        }

        [Test]
        public void SourceProjectPathIsInitializedCorrectly()
        {
            _testClass.SourceProjectPath.Should().Be(_sourceProjectPath);
        }
    }
}