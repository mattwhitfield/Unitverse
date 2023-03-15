namespace Unitverse.Core.Tests.Templating
{
    using System;
    using AutoFixture;
    using AutoFixture.AutoMoq;
    using FluentAssertions;
    using NUnit.Framework;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Templating;

    [TestFixture]
    public static class TemplateStoreTests
    {
        [Test]
        public static void CanCallLoadTemplatesFor()
        {
            // Arrange
            //var fixture = new Fixture().Customize(new AutoMoqCustomization());
            //var folder = fixture.Create<string>();
            //var messageLogger = fixture.Create<IMessageLogger>();

            // Act
            //var result = TemplateStore.LoadTemplatesFor(folder, messageLogger);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public static void CannotCallLoadTemplatesForWithNullMessageLogger()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            FluentActions.Invoking(() => TemplateStore.LoadTemplatesFor(fixture.Create<string>(), default(IMessageLogger))).Should().Throw<ArgumentNullException>().WithParameterName("messageLogger");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CannotCallLoadTemplatesForWithInvalidFolder(string value)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            FluentActions.Invoking(() => TemplateStore.LoadTemplatesFor(value, fixture.Create<IMessageLogger>())).Should().Throw<ArgumentNullException>().WithParameterName("folder");
        }
    }
}