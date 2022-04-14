namespace Unitverse.Core.Tests.Helpers
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    [TestFixture]
    public static class TargetNameTransformTests
    {
        [Test]
        public static void CanCallGetTargetProjectName()
        {
            var options = Substitute.For<IGenerationOptions>();
            options.TestProjectNaming.Returns("{0}.Tests");
            var sourceProjectName = "TestValue1494137907";
            var result = options.GetTargetProjectNames(sourceProjectName).First();
            Assert.That(result, Is.EqualTo("TestValue1494137907.Tests"));
            options.TestProjectNaming.Returns("{0");
            Assert.Throws<InvalidOperationException>(() => options.GetTargetProjectNames(sourceProjectName).ToList());
        }

        [Test]
        public static void CanCallGetTargetProjectNamesWithMultiplePatterns()
        {
            var options = Substitute.For<IGenerationOptions>();
            options.TestProjectNaming.Returns("{0}.Tests;{0}.UnitTests;{0}.UT;fred");
            var sourceProjectName = "TestValue1494137907";
            var result = options.GetTargetProjectNames(sourceProjectName).ToList();
            result.Should().BeEquivalentTo(new[] { "TestValue1494137907.Tests", "TestValue1494137907.UnitTests", "TestValue1494137907.UT", "fred" });
        }

        [Test]
        public static void CannotCallGetTargetProjectNameWithNullOptions()
        {
            Assert.Throws<ArgumentNullException>(() => default(IGenerationOptions).GetTargetProjectNames("TestValue1037312724").ToList());
        }

        [Test]
        public static void CannotCallGetTargetProjectNameWithNullSourceProjectName()
        {
            Assert.Throws<ArgumentNullException>(() => Substitute.For<IGenerationOptions>().GetTargetProjectNames(default(string)).ToList());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CannotCallGetTargetProjectNameWithInvalidSourceProjectName(string value)
        {
            Assert.Throws<ArgumentNullException>(() => Substitute.For<IGenerationOptions>().GetTargetProjectNames(value).ToList());
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

        [Test]
        public static void CanCallGetTargetTypeName()
        {
            var frameworkSet = Substitute.For<IFrameworkSet>();
            frameworkSet.Options.GenerationOptions.TestTypeNaming.Returns("{0}Tests");
            var classModel = ClassModelProvider.Instance;
            var result = frameworkSet.GetTargetTypeName(classModel);
            Assert.That(result, Is.EqualTo(ClassModelProvider.Instance.ClassName + "Tests"));
        }

        [Test]
        public static void CanCallGetTargetTypeNameWithGenericDisambiguation()
        {
            var frameworkSet = Substitute.For<IFrameworkSet>();
            frameworkSet.Options.GenerationOptions.TestTypeNaming.Returns("{0}Tests");
            var classModel = ClassModelProvider.GenericInstance;
            var result = frameworkSet.GetTargetTypeName(classModel);
            Assert.That(result, Is.EqualTo(ClassModelProvider.GenericInstance.ClassName + "_1Tests"));
        }

        [Test]
        public static void CannotCallGetTargetTypeNameWithNullFrameworkSet()
        {
            Assert.Throws<ArgumentNullException>(() => default(IFrameworkSet).GetTargetTypeName(ClassModelProvider.Instance));
        }

        [Test]
        public static void CannotCallGetTargetTypeNameWithNullClassModel()
        {
            Assert.Throws<ArgumentNullException>(() => Substitute.For<IFrameworkSet>().GetTargetTypeName(default(ClassModel)));
        }
    }
}