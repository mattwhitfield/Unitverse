namespace Unitverse.Core.Tests.Frameworks
{
    using Unitverse.Core.Frameworks;
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;
    using FakeItEasy;
    using Unitverse.Core.Options;

    [TestClass]
    public class FrameworkPackageProviderTests
    {
        [TestMethod]
        public void CanCallGet()
        {
            // Arrange
            var generationOptions = A.Fake<IGenerationOptions>();

            // Act
            var result = FrameworkPackageProvider.Get(generationOptions);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [TestMethod]
        public void CannotCallGetWithNullGenerationOptions()
        {
            FluentActions.Invoking(() => FrameworkPackageProvider.Get(default(IGenerationOptions))).Should().Throw<ArgumentNullException>();
        }
    }
}