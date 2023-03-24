namespace Unitverse.Core.Tests.Options
{
    using System;
    using System.Collections.Generic;
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
            Assert.Throws<ArgumentNullException>(() => UnitTestGeneratorOptionsFactory.Create("slnPath", "TestValue1494081794", default(IGenerationOptions), Substitute.For<INamingOptions>(), Substitute.For<IStrategyOptions>(), false, new Dictionary<string, string>()));
        }

        [Test]
        public static void CannotCallCreateWithNullNamingOptions()
        {
            Assert.Throws<ArgumentNullException>(() => UnitTestGeneratorOptionsFactory.Create("slnPath", "TestValue1494081794", Substitute.For<IGenerationOptions>(), default(INamingOptions), Substitute.For<IStrategyOptions>(), false, new Dictionary<string, string>()));
        }

        [Test]
        public static void CannotCallCreateWithNullStrategyOptions()
        {
            Assert.Throws<ArgumentNullException>(() => UnitTestGeneratorOptionsFactory.Create("slnPath", "TestValue1494081794", Substitute.For<IGenerationOptions>(), Substitute.For<INamingOptions>(), default(IStrategyOptions), false, new Dictionary<string, string>()));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CanCallCreateWithInvalidSolutionFilePath(string value)
        {
            Assert.DoesNotThrow(() => UnitTestGeneratorOptionsFactory.Create("slnPath", value, Substitute.For<IGenerationOptions>(), Substitute.For<INamingOptions>(), Substitute.For<IStrategyOptions>(), false, new Dictionary<string, string>()));
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
                var strategyOptions = Substitute.For<IStrategyOptions>();
                generationOptions.MockingFrameworkType.Returns(MockingFrameworkType.NSubstitute);

                File.WriteAllText(Path.Combine(pathA, CoreConstants.ConfigFileName), "framework-type=XUnit");
                File.WriteAllText(Path.Combine(pathB, CoreConstants.ConfigFileName), "framework-type=NUnit3");
                File.WriteAllText(Path.Combine(pathC, CoreConstants.ConfigFileName), "framework-type=NUnit2");

                var result = UnitTestGeneratorOptionsFactory.Create("", solutionFilePath, generationOptions, namingOptions, strategyOptions, false, new Dictionary<string, string>());
                Assert.That(result.GenerationOptions.FrameworkType, Is.EqualTo(TestFrameworkTypes.NUnit2));
                Assert.That(result.GenerationOptions.MockingFrameworkType, Is.EqualTo(MockingFrameworkType.NSubstitute));

                Assert.That(result.GetFieldSource(nameof(IGenerationOptions.FrameworkType)).FileName, Is.EqualTo(Path.Combine(pathC, CoreConstants.ConfigFileName)));
                Assert.That(result.GetFieldSource(nameof(IGenerationOptions.MockingFrameworkType)).SourceType, Is.EqualTo(ConfigurationSourceType.VisualStudio));
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