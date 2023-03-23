namespace Unitverse.Core.Tests.Options
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using NUnit.Framework;
    using Unitverse.Core.Options;
    using Unitverse.Options;

    [TestFixture]
    public static class SessionConfigStoreTests
    {
        [Test]
        public static void CanCallStoreSettingsAndRestoreSettings()
        {
            // Arrange
            var settings = new Dictionary<string, string>();
            settings[nameof(IGenerationOptions.ArrangeComment)] = "some other thing";
            var mutated = new List<string>();
            var test = new GenerationOptions();

            // Act
            SessionConfigStore.StoreSettings(settings);
            SessionConfigStore.RestoreSettings(test, setting => mutated.Add(setting));

            // Assert
            mutated.Should().Contain(nameof(IGenerationOptions.ArrangeComment));
            test.ArrangeComment.Should().Be("some other thing");
        }

        [Test]
        public static void CanCallSetTargetFor()
        {
            SessionConfigStore.ProjectMappings.Clear();

            // Arrange
            var sourceProjectName = "TestValue2121102834";
            var targetProjectName = "TestValue1116763453";

            // Act
            SessionConfigStore.SetTargetFor(sourceProjectName, targetProjectName);

            // Assert
            SessionConfigStore.ProjectMappings.Should().Contain(new KeyValuePair<string, string>(sourceProjectName, targetProjectName));
        }

        [Test]
        public static void CanCallAddModifiedValuesToDictionary()
        {
            // Arrange
            var modifiedSettings = new GenerationOptions() { ArrangeComment = "some stuff" };
            var baseSettings = new GenerationOptions() { ActComment = "some other stuff" };
            var target = new Dictionary<string, string>();

            // Act
            SessionConfigStore.AddModifiedValuesToDictionary(modifiedSettings, baseSettings, target);

            // Assert
            target.Should().Contain(new KeyValuePair<string, string>(nameof(IGenerationOptions.ActComment), modifiedSettings.ActComment));
            target.Should().Contain(new KeyValuePair<string, string>(nameof(IGenerationOptions.ArrangeComment), modifiedSettings.ArrangeComment));
        }
    }
}