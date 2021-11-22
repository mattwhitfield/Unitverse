namespace SentryOne.UnitTestGenerator.Core.Tests.Options
{
    using System;
    using System.IO;
    using NSubstitute;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Options;

    [TestFixture]
    public static class UnitTestGeneratorOptionsFactoryTests
    {
        [Test]
        public static void CannotCallCreateWithNullGenerationOptions()
        {
            Assert.Throws<ArgumentNullException>(() => UnitTestGeneratorOptionsFactory.Create("TestValue1494081794", default(MutableGenerationOptions)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CanCallCreateWithInvalidSolutionFilePath(string value)
        {
            Assert.DoesNotThrow(() => UnitTestGeneratorOptionsFactory.Create(value, Substitute.For<IGenerationOptions>()));
        }

        [Test]
        public static void CreateLoadsConfigFiles()
        {
            string tempfolder = null;
            try
            {
                while (true)
                {
                    try
                    {
                        tempfolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                        Directory.CreateDirectory(tempfolder);
                        break;
                    }
                    catch (IOException)
                    {
                    }
                }

                var pathA = Path.Combine(tempfolder, "a");
                var pathB = Path.Combine(tempfolder, "a", "b");
                var pathC = Path.Combine(tempfolder, "a", "b", "c");
                Directory.CreateDirectory(pathC);

                var solutionFilePath = Path.Combine(pathC, "someSolution.sln");
                var generationOptions = Substitute.For<IGenerationOptions>();
                generationOptions.MockingFrameworkType.Returns(MockingFrameworkType.NSubstitute);

                File.WriteAllText(Path.Combine(pathA, ".unitTestGeneratorConfig"), "framework-type=XUnit");
                File.WriteAllText(Path.Combine(pathB, ".unitTestGeneratorConfig"), "framework-type=NUnit3");
                File.WriteAllText(Path.Combine(pathC, ".unitTestGeneratorConfig"), "framework-type=NUnit2");

                var result = UnitTestGeneratorOptionsFactory.Create(solutionFilePath, generationOptions);
                Assert.That(result.GenerationOptions.FrameworkType, Is.EqualTo(TestFrameworkTypes.NUnit2));
                Assert.That(result.GenerationOptions.MockingFrameworkType, Is.EqualTo(MockingFrameworkType.NSubstitute));
            }
            finally
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(tempfolder))
                    {
                        Directory.Delete(tempfolder, true);
                    }
                }
                catch (IOException)
                {
                }
            }
        }
    }
}