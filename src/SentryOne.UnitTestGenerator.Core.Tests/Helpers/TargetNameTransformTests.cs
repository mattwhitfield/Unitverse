namespace SentryOne.UnitTestGenerator.Core.Tests.Helpers
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Options;

    [TestFixture]
    public static class TargetNameTransformTests
    {
        [Test]
        public static void CanCallGetTargetProjectName()
        {
            var options = Substitute.For<IGenerationOptions>();
            options.TestProjectNaming.Returns("{0}.Tests");
            var sourceProjectName = "TestValue1494137907";
            var result = options.GetTargetProjectName(sourceProjectName);
            Assert.That(result, Is.EqualTo("TestValue1494137907.Tests"));
            options.TestProjectNaming.Returns("{0");
            Assert.Throws<InvalidOperationException>(() => options.GetTargetProjectName(sourceProjectName));
        }

        [Test]
        public static void CannotCallGetTargetProjectNameWithNullOptions()
        {
            Assert.Throws<ArgumentNullException>(() => default(IGenerationOptions).GetTargetProjectName("TestValue1037312724"));
        }

        [Test]
        public static void CannotCallGetTargetProjectNameWithNullSourceProjectName()
        {
            Assert.Throws<ArgumentNullException>(() => Substitute.For<IGenerationOptions>().GetTargetProjectName(default(string)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CannotCallGetTargetProjectNameWithInvalidSourceProjectName(string value)
        {
            Assert.Throws<ArgumentNullException>(() => Substitute.For<IGenerationOptions>().GetTargetProjectName(value));
        }

        [Test]
        public static void CanCallGetTargetFileName()
        {
            var options = Substitute.For<IGenerationOptions>();
            options.TestFileNaming.Returns("{0}Tests");
            var sourceFileName = "TestValue1494137907";
            var result = options.GetTargetFileName(sourceFileName);
            Assert.That(result, Is.EqualTo("TestValue1494137907Tests"));
            options.TestFileNaming.Returns("{0");
            Assert.Throws<InvalidOperationException>(() => options.GetTargetFileName(sourceFileName));
        }

        [Test]
        public static void CannotCallGetTargetFileNameWithNullOptions()
        {
            Assert.Throws<ArgumentNullException>(() => default(IGenerationOptions).GetTargetFileName("TestValue613902463"));
        }

        [Test]
        public static void CannotCallGetTargetFileNameWithNullSourceFileName()
        {
            Assert.Throws<ArgumentNullException>(() => Substitute.For<IGenerationOptions>().GetTargetFileName(default(string)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CannotCallGetTargetFileNameWithInvalidSourceFileName(string value)
        {
            Assert.Throws<ArgumentNullException>(() => Substitute.For<IGenerationOptions>().GetTargetFileName(value));
        }
    }
}