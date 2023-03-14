namespace Unitverse.Core.Tests.Templating
{
    using System;
    using AutoFixture;
    using AutoFixture.AutoMoq;
    using FluentAssertions;
    using NUnit.Framework;
    using Unitverse.Core.Templating;

    [TestFixture]
    public static class TemplateReaderTests
    {
        [Test]
        public static void CanCallReadFromWithFileName()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var fileName = fixture.Create<string>();

            // Act
            var result = TemplateReader.ReadFrom(fileName);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CannotCallReadFromWithFileNameWithInvalidFileName(string value)
        {
            FluentActions.Invoking(() => TemplateReader.ReadFrom(value)).Should().Throw<ArgumentNullException>().WithParameterName("fileName");
        }

        [Test]
        public static void CanCallReadFromWithLinesAndFileName()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var lines = fixture.Create<string[]>();
            var fileName = fixture.Create<string>();

            // Act
            var result = TemplateReader.ReadFrom(lines, fileName);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public static void CannotCallReadFromWithLinesAndFileNameWithNullLines()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            FluentActions.Invoking(() => TemplateReader.ReadFrom(default(string[]), fixture.Create<string>())).Should().Throw<ArgumentNullException>().WithParameterName("lines");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CannotCallReadFromWithLinesAndFileNameWithInvalidFileName(string value)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            FluentActions.Invoking(() => TemplateReader.ReadFrom(fixture.Create<string[]>(), value)).Should().Throw<ArgumentNullException>().WithParameterName("fileName");
        }
    }
}