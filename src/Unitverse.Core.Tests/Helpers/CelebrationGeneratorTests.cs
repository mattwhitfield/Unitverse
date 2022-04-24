namespace Unitverse.Core.Tests.Helpers
{
    using Unitverse.Core.Helpers;
    using System;
    using NUnit.Framework;
    using FluentAssertions;
    using NSubstitute;

    [TestFixture]
    public static class CelebrationGeneratorTests
    {
        private static IGenerationStatistics GetStatistics()
        {
            return new GenerationStatistics { InterfacesMocked = 123, TestClassesGenerated = 314, TestMethodsGenerated = 1023, TestMethodsRegenerated = 24, TypesConstructed = 3020, ValuesGenerated = 2412 };
        }

        [Test]
        public static void CanCallGetAnimal()
        {
            // Act
            var result = CelebrationGenerator.GetAnimal(out var name);

            // Assert
            result.Should().NotBeEmpty();
            name.Should().NotBeEmpty();
        }

        [Test]
        public static void CanCallGetMessageWithGenerationStatistics()
        {
            // Arrange
            var generationStatistics = GetStatistics();
            
            // Act
            var result = CelebrationGenerator.GetMessage(generationStatistics);

            // Assert
            Console.WriteLine(result);
            result.Should().Contain("You have created 1,023 test methods with Unitverse!");
        }

        [Test]
        public static void CannotCallGetMessageWithGenerationStatisticsWithNullGenerationStatistics()
        {
            FluentActions.Invoking(() => CelebrationGenerator.GetMessage(default(IGenerationStatistics))).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public static void CanCallGetMessageWithAnimalAndNameAndGenerationStatistics()
        {
            // Arrange
            var animal = "TestValue1055610820";
            var name = "TestValue1764101598";
            var generationStatistics = GetStatistics();
            
            // Act
            var result = CelebrationGenerator.GetMessage(animal, name, generationStatistics);
            
            // Assert
            Console.WriteLine(result);
            result.Should().Be("TestValue1055610820   TestValue1764101598 says:\r\n" +
                               "                      C O N G R A T U L A T I O N S !\r\n" +
                               "\r\n" +
                               "                      You have created 1,023 test methods with Unitverse!");
        }

        [Test]
        public static void CannotCallGetMessageWithAnimalAndNameAndGenerationStatisticsWithNullGenerationStatistics()
        {
            FluentActions.Invoking(() => CelebrationGenerator.GetMessage("TestValue1338827180", "TestValue255112063", default(IGenerationStatistics))).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CannotCallGetMessageWithAnimalAndNameAndGenerationStatisticsWithInvalidAnimal(string value)
        {
            FluentActions.Invoking(() => CelebrationGenerator.GetMessage(value, "TestValue1751383614", Substitute.For<IGenerationStatistics>())).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CannotCallGetMessageWithAnimalAndNameAndGenerationStatisticsWithInvalidName(string value)
        {
            FluentActions.Invoking(() => CelebrationGenerator.GetMessage("TestValue429365945", value, Substitute.For<IGenerationStatistics>())).Should().Throw<ArgumentNullException>();
        }
    }
}