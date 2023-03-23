namespace Unitverse.Core.Tests.Options
{
    using FluentAssertions;
    using NUnit.Framework;
    using Unitverse.Core.Options;

    [TestFixture]
    public class ConfigurationSourceTests
    {
        private ConfigurationSource _testClass;
        private ConfigurationSourceType _sourceType;
        private string _fileName;

        [SetUp]
        public void SetUp()
        {
            _sourceType = ConfigurationSourceType.VisualStudio;
            _fileName = "TestValue288296536";
            _testClass = new ConfigurationSource(_sourceType, _fileName);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new ConfigurationSource(_sourceType, _fileName);

            // Assert
            instance.Should().NotBeNull();
        }

        [Test]
        public void SourceTypeIsInitializedCorrectly()
        {
            _testClass.SourceType.Should().Be(_sourceType);
        }

        [Test]
        public void FileNameIsInitializedCorrectly()
        {
            _testClass.FileName.Should().Be(_fileName);
        }
    }
}