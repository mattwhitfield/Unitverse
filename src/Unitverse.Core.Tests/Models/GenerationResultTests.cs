namespace Unitverse.Core.Tests.Models
{
    using NUnit.Framework;
    using Unitverse.Core.Models;
    using System;
    using FluentAssertions;
    using Unitverse.Core.Helpers;
    using NSubstitute;

    [TestFixture]
    public class GenerationResultTests
    {
        private GenerationResult _testClass;
        private string _fileContent;
        private bool _anyMethodsEmitted;
        private IGenerationStatistics _stats;
        private IGenerationStatistics _sourceStatistics;

        [SetUp]
        public void SetUp()
        {
            _fileContent = "TestValue1767881884";
            _anyMethodsEmitted = false;
            _sourceStatistics = Substitute.For<IGenerationStatistics>();
            _stats = Substitute.For<IGenerationStatistics>();
            _stats.InterfacesMocked.Returns(1);
            _stats.TestClassesGenerated.Returns(2);
            _stats.TestMethodsGenerated.Returns(3);
            _stats.TestMethodsRegenerated.Returns(4);
            _stats.TypesConstructed.Returns(5);
            _stats.ValuesGenerated.Returns(6);

            _testClass = new GenerationResult(_fileContent, _anyMethodsEmitted, _stats);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new GenerationResult(_fileContent, _anyMethodsEmitted, Substitute.For<IGenerationStatistics>());
            instance.Should().NotBeNull();
        }

        [Test]
        public void FileContentIsInitializedCorrectly()
        {
            _testClass.FileContent.Should().Be(_fileContent);
        }

        [Test]
        public void GenerationStatisticsAreInitializedCorrectly()
        {
            _testClass.InterfacesMocked.Should().Be(1);
            _testClass.TestClassesGenerated.Should().Be(2);
            _testClass.TestMethodsGenerated.Should().Be(3);
            _testClass.TestMethodsRegenerated.Should().Be(4);
            _testClass.TypesConstructed.Should().Be(5);
            _testClass.ValuesGenerated.Should().Be(6);
        }

        [Test]
        public void CanGetRequiredAssets()
        {
            _testClass.RequiredAssets.Should().NotBeNull();
            _testClass.RequiredAssets.Should().BeEmpty();
        }

        [Test]
        public void AnyMethodsEmittedIsInitializedCorrectly()
        {
            _testClass.AnyMethodsEmitted.Should().Be(_anyMethodsEmitted);
        }

        [Test]
        public void CannotConstructWithNullSourceStatistics()
        {
            FluentActions.Invoking(() => new GenerationResult("TestValue281816781", true, default(IGenerationStatistics))).Should().Throw<ArgumentNullException>();
        }
    }
}