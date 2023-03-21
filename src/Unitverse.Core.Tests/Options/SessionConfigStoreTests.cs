namespace Unitverse.Core.Tests.Options
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using NUnit.Framework;
    using Unitverse.Core.Options;
    using T = System.String;

    [TestFixture]
    public static class SessionConfigStoreTests
    {
        [Test]
        public static void CanCallStoreSettings()
        {
            // Arrange
            var settings = new Dictionary<string, string>();

            // Act
            SessionConfigStore.StoreSettings(settings);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public static void CanCallRestoreSettings()
        {
            // Arrange
            var target = "TestValue2116604866";
            Action<string> onMemberSet = x => { };

            // Act
            SessionConfigStore.RestoreSettings<T>(target, onMemberSet);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public static void CanCallSetTargetFor()
        {
            // Arrange
            var sourceProjectName = "TestValue2121102834";
            var targetProjectName = "TestValue1116763453";

            // Act
            SessionConfigStore.SetTargetFor(sourceProjectName, targetProjectName);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public static void CanCallAddModifiedValuesToDictionary()
        {
            // Arrange
            var modifiedSettings = "TestValue1003885563";
            var baseSettings = "TestValue251107448";
            var target = new Dictionary<string, string>();

            // Act
            SessionConfigStore.AddModifiedValuesToDictionary<T>(modifiedSettings, baseSettings, target);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public static void CanGetProjectMappings()
        {
            // Assert
            SessionConfigStore.ProjectMappings.Should().BeAssignableTo<Dictionary<string, string>>();

            Assert.Fail("Create or modify test");
        }
    }
}