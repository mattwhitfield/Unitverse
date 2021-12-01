namespace Unitverse.Core.Tests.Helpers
{
    using Unitverse.Core.Helpers;
    using System;
    using NUnit.Framework;
    using NSubstitute;
    using Unitverse.Core.Options;

    [TestFixture]
    public class DetectedGenerationOptionsTests
    {
        private IGenerationOptions _baseOptions;

        [SetUp]
        public void SetUp()
        {
            _baseOptions = Substitute.For<IGenerationOptions>();
        }

        [Test]
        public void CannotConstructWithNullBaseOptions()
        {
            Assert.Throws<ArgumentNullException>(() => new DetectedGenerationOptions(default(IGenerationOptions), false, TestFrameworkTypes.XUnit, MockingFrameworkType.FakeItEasy));
        }

        [Test]
        public void CanGetFrameworkType()
        {
            _baseOptions.FrameworkType.Returns(TestFrameworkTypes.XUnit);
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null);
            Assert.That(instance.FrameworkType, Is.EqualTo(TestFrameworkTypes.XUnit));
            instance = new DetectedGenerationOptions(_baseOptions, null, TestFrameworkTypes.NUnit2, null);
            Assert.That(instance.FrameworkType, Is.EqualTo(TestFrameworkTypes.NUnit2));
        }

        [Test]
        public void CanGetMockingFrameworkType()
        {
            _baseOptions.MockingFrameworkType.Returns(MockingFrameworkType.FakeItEasy);
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null);
            Assert.That(instance.MockingFrameworkType, Is.EqualTo(MockingFrameworkType.FakeItEasy));
            instance = new DetectedGenerationOptions(_baseOptions, null, null, MockingFrameworkType.NSubstitute);
            Assert.That(instance.MockingFrameworkType, Is.EqualTo(MockingFrameworkType.NSubstitute));
        }

        [Test]
        public void CanGetUseFluentAssertions()
        {
            _baseOptions.UseFluentAssertions.Returns(true);
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null);
            Assert.That(instance.UseFluentAssertions, Is.EqualTo(true));
            instance = new DetectedGenerationOptions(_baseOptions, false, null, null);
            Assert.That(instance.UseFluentAssertions, Is.EqualTo(false));
            _baseOptions.UseFluentAssertions.Returns(false);
            instance = new DetectedGenerationOptions(_baseOptions, true, null, null);
            Assert.That(instance.UseFluentAssertions, Is.EqualTo(true));
        }

        [Test]
        public void CanGetAutoDetectFrameworkTypes()
        {
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null);
            _baseOptions.AutoDetectFrameworkTypes.Returns(true);
            Assert.That(instance.AutoDetectFrameworkTypes, Is.EqualTo(true));
            _baseOptions.AutoDetectFrameworkTypes.Returns(false);
            Assert.That(instance.AutoDetectFrameworkTypes, Is.EqualTo(false));
        }

        [Test]
        public void CanGetAllowGenerationWithoutTargetProject()
        {
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null);
            _baseOptions.AllowGenerationWithoutTargetProject.Returns(true);
            Assert.That(instance.AllowGenerationWithoutTargetProject, Is.EqualTo(true));
            _baseOptions.AllowGenerationWithoutTargetProject.Returns(false);
            Assert.That(instance.AllowGenerationWithoutTargetProject, Is.EqualTo(false));
        }

        [Test]
        public void CanGetTestProjectNaming()
        {
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null);
            _baseOptions.TestProjectNaming.Returns("fgwerfgwe");
            Assert.That(instance.TestProjectNaming, Is.EqualTo("fgwerfgwe"));
        }

        [Test]
        public void CanGetTestFileNaming()
        {
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null);
            _baseOptions.TestFileNaming.Returns("retgerg");
            Assert.That(instance.TestFileNaming, Is.EqualTo("retgerg"));
        }

        [Test]
        public void CanGetTestTypeNaming()
        {
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null);
            _baseOptions.TestTypeNaming.Returns("5252532");
            Assert.That(instance.TestTypeNaming, Is.EqualTo("5252532"));
        }
    }
}