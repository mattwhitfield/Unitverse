namespace Unitverse.Core.Tests.Options
{
    using System;
    using System.IO;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Options;

    [TestFixture]
    public static class UnitTestGeneratorOptionsFactoryTests
    {
        [Test]
        public static void CannotCallCreateWithNullGenerationOptions()
        {
            Assert.Throws<ArgumentNullException>(() => UnitTestGeneratorOptionsFactory.Create("TestValue1494081794", default(IGenerationOptions), Substitute.For<INamingOptions>(), false));
        }

        [Test]
        public static void CannotCallCreateWithNullNamingOptions()
        {
            Assert.Throws<ArgumentNullException>(() => UnitTestGeneratorOptionsFactory.Create("TestValue1494081794", Substitute.For<IGenerationOptions>(), default(INamingOptions), false));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CanCallCreateWithInvalidSolutionFilePath(string value)
        {
            Assert.DoesNotThrow(() => UnitTestGeneratorOptionsFactory.Create(value, Substitute.For<IGenerationOptions>(), Substitute.For<INamingOptions>(), false));
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
                var namingOptions = Substitute.For<INamingOptions>();
                generationOptions.MockingFrameworkType.Returns(MockingFrameworkType.NSubstitute);

                File.WriteAllText(Path.Combine(pathA, CoreConstants.ConfigFileName), "framework-type=XUnit");
                File.WriteAllText(Path.Combine(pathB, CoreConstants.ConfigFileName), "framework-type=NUnit3");
                File.WriteAllText(Path.Combine(pathC, CoreConstants.ConfigFileName), "framework-type=NUnit2");

                var result = UnitTestGeneratorOptionsFactory.Create(solutionFilePath, generationOptions, namingOptions, false);
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