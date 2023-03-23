namespace Unitverse.Core.Tests.Options.Editing
{
    using System.Collections.Generic;
    using FluentAssertions;
    using NUnit.Framework;
    using Unitverse.Core.Options;
    using Unitverse.Core.Options.Editing;
    using Unitverse.Options;

    [TestFixture]
    public class SessionConfigurationWriterTests
    {
        private SessionConfigurationWriter _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new SessionConfigurationWriter();
        }

        [Test]
        public void CanCallWriteSettings()
        {
            // Arrange
            var settings = new Dictionary<string, string>();
            settings[nameof(IGenerationOptions.ActComment)] = "get the stuff set up";
            var sourceProjectName = "TestValue1741200942";
            var targetProjectName = "TestValue1755919325";

            // Act
            _testClass.WriteSettings(settings, sourceProjectName, targetProjectName);

            // Assert
            var mutated = new List<string>();
            var test = new GenerationOptions();
            SessionConfigStore.RestoreSettings(test, setting => mutated.Add(setting));
            SessionConfigStore.ProjectMappings.Should().Contain(new KeyValuePair<string, string>(sourceProjectName, targetProjectName));
            mutated.Should().Contain(nameof(IGenerationOptions.ActComment));
            test.ActComment.Should().Be("get the stuff set up");
        }
    }
}