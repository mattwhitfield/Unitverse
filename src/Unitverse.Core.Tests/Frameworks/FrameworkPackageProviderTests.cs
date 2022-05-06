namespace Unitverse.Core.Tests.Frameworks
{
    using Unitverse.Core.Frameworks;
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;
    using Unitverse.Core.Options;
    using NSubstitute;
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class FrameworkPackageProviderTests
    {
        [TestCase(TestFrameworkTypes.NUnit2, "nunit:2.7.1", "NUnitTestAdapter:*")]
        [TestCase(TestFrameworkTypes.NUnit3, "nunit:*", "NUnit3TestAdapter:*")]
        [TestCase(TestFrameworkTypes.MsTest, "MSTest.TestFramework:*", "MSTest.TestAdapter:*")]
        [TestCase(TestFrameworkTypes.XUnit, "xunit:*", "xunit.runner.visualstudio:*")]
        public void CanCallGetForTestFramework(TestFrameworkTypes type, string expected, string expected2)
        {
            // Arrange
            var generationOptions = Substitute.For<IGenerationOptions>();

            generationOptions.UseFluentAssertions.Returns(true);
            generationOptions.FrameworkType.Returns(type);

            // Act
            var result = FrameworkPackageProvider.Get(generationOptions).ToList();

            // Assert
            result.Should().Contain(x => x.Name == "coverlet.collector");
            result.Should().Contain(x => x.Name == "FluentAssertions");
            result.Should().Contain(x => x.Name + ":" + (string.IsNullOrWhiteSpace(x.Version) ? "*" : x.Version) == expected);
            result.Should().Contain(x => x.Name + ":" + (string.IsNullOrWhiteSpace(x.Version) ? "*" : x.Version) == expected2);
        }

        [TestCase(MockingFrameworkType.FakeItEasy, "FakeItEasy:*", "")]
        [TestCase(MockingFrameworkType.Moq, "Moq:*", "")]
        [TestCase(MockingFrameworkType.MoqAutoMock, "Moq:*", "Moq.AutoMock:*")]
        [TestCase(MockingFrameworkType.NSubstitute, "NSubstitute:*", "")]
        public void CanCallGetForMockingFramework(MockingFrameworkType type, string expected, string expected2)
        {
            // Arrange
            var generationOptions = Substitute.For<IGenerationOptions>();

            generationOptions.UseFluentAssertions.Returns(false);
            generationOptions.MockingFrameworkType.Returns(type);

            // Act
            var result = FrameworkPackageProvider.Get(generationOptions).ToList();

            // Assert
            result.Should().Contain(x => x.Name == "coverlet.collector");
            result.Should().NotContain(x => x.Name == "FluentAssertions");
            result.Should().Contain(x => x.Name + ":" + (string.IsNullOrWhiteSpace(x.Version) ? "*" : x.Version) == expected);
            if (!string.IsNullOrEmpty(expected2))
            {
                result.Should().Contain(x => x.Name + ":" + (string.IsNullOrWhiteSpace(x.Version) ? "*" : x.Version) == expected2);
            }
        }

        [Test]
        public void CannotCallGetWithNullGenerationOptions()
        {
            FluentActions.Invoking(() => FrameworkPackageProvider.Get(default(IGenerationOptions)).ToList()).Should().Throw<ArgumentNullException>();
        }
    }
}