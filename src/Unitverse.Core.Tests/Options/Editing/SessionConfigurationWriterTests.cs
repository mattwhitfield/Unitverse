namespace Unitverse.Core.Tests.Options.Editing
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using NUnit.Framework;
    using Unitverse.Core.Options.Editing;

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
            var sourceProjectName = "TestValue1741200942";
            var targetProjectName = "TestValue1755919325";

            // Act
            _testClass.WriteSettings(settings, sourceProjectName, targetProjectName);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallWriteSettingsWithNullSettings()
        {
            FluentActions.Invoking(() => _testClass.WriteSettings(default(Dictionary<string, string>), "TestValue1490166552", "TestValue1333441253")).Should().Throw<ArgumentNullException>().WithParameterName("settings");
        }

        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallWriteSettingsWithInvalidSourceProjectName(string value)
        {
            FluentActions.Invoking(() => _testClass.WriteSettings(new Dictionary<string, string>(), value, "TestValue446331582")).Should().Throw<ArgumentNullException>().WithParameterName("sourceProjectName");
        }

        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallWriteSettingsWithInvalidTargetProjectName(string value)
        {
            FluentActions.Invoking(() => _testClass.WriteSettings(new Dictionary<string, string>(), "TestValue761041122", value)).Should().Throw<ArgumentNullException>().WithParameterName("targetProjectName");
        }
    }
}