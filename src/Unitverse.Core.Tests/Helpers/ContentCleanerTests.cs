namespace Unitverse.Core.Tests.Helpers
{
    using Unitverse.Core.Helpers;
    using System;
    using NUnit.Framework;
    using FluentAssertions;

    [TestFixture]
    public static class ContentCleanerTests
    {
        [Test]
        public static void CanCallClean()
        {
            // Arrange
            var content = "TestValue186491743\r\n    \r\n  someValue\n    \nSomeOtherValue\nSomething";
            
            // Act
            var result = ContentCleaner.Clean(content);

            // Assert
            result.Should().Be("TestValue186491743\r\n\r\n  someValue\n\nSomeOtherValue\nSomething");

        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CanCallCleanWithInvalidContent(string value)
        {
            ContentCleaner.Clean(value).Should().Be(value);
        }
    }
}