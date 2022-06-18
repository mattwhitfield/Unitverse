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
            Assert.Throws<ArgumentNullException>(() => new DetectedGenerationOptions(default(IGenerationOptions), false, false, false, TestFrameworkTypes.XUnit, MockingFrameworkType.FakeItEasy));
        }

        [Test]
        public void CanGetFrameworkType()
        {
            _baseOptions.FrameworkType.Returns(TestFrameworkTypes.XUnit);
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null, null, null);
            Assert.That(instance.FrameworkType, Is.EqualTo(TestFrameworkTypes.XUnit));
            instance = new DetectedGenerationOptions(_baseOptions, null, null, null, TestFrameworkTypes.NUnit2, null);
            Assert.That(instance.FrameworkType, Is.EqualTo(TestFrameworkTypes.NUnit2));
        }

        [Test]
        public void CanGetMockingFrameworkType()
        {
            _baseOptions.MockingFrameworkType.Returns(MockingFrameworkType.FakeItEasy);
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null, null, null);
            Assert.That(instance.MockingFrameworkType, Is.EqualTo(MockingFrameworkType.FakeItEasy));
            instance = new DetectedGenerationOptions(_baseOptions, null, null, null, null, MockingFrameworkType.NSubstitute);
            Assert.That(instance.MockingFrameworkType, Is.EqualTo(MockingFrameworkType.NSubstitute));
        }

        [Test]
        public void CanGetUseFluentAssertions()
        {
            _baseOptions.UseFluentAssertions.Returns(true);
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null, null, null);
            Assert.That(instance.UseFluentAssertions, Is.EqualTo(true));
            instance = new DetectedGenerationOptions(_baseOptions, false, null, null, null, null);
            Assert.That(instance.UseFluentAssertions, Is.EqualTo(false));
            _baseOptions.UseFluentAssertions.Returns(false);
            instance = new DetectedGenerationOptions(_baseOptions, true, null, null, null, null);
            Assert.That(instance.UseFluentAssertions, Is.EqualTo(true));
        }

        [Test]
        public void CanGetUseAutoFixture()
        {
            _baseOptions.UseAutoFixture.Returns(true);
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null, null, null);
            Assert.That(instance.UseAutoFixture, Is.EqualTo(true));
            instance = new DetectedGenerationOptions(_baseOptions, null, false, null, null, null);
            Assert.That(instance.UseAutoFixture, Is.EqualTo(false));
            _baseOptions.UseFluentAssertions.Returns(false);
            instance = new DetectedGenerationOptions(_baseOptions, null, true, null, null, null);
            Assert.That(instance.UseAutoFixture, Is.EqualTo(true));
        }

        [Test]
        public void CanGetUseAutoFixtureForMocking()
        {
            _baseOptions.UseAutoFixtureForMocking.Returns(true);
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null, null, null);
            Assert.That(instance.UseAutoFixtureForMocking, Is.EqualTo(true));
            instance = new DetectedGenerationOptions(_baseOptions, null, null, false, null, null);
            Assert.That(instance.UseAutoFixtureForMocking, Is.EqualTo(false));
            _baseOptions.UseAutoFixtureForMocking.Returns(false);
            instance = new DetectedGenerationOptions(_baseOptions, null, null, true, null, null);
            Assert.That(instance.UseAutoFixtureForMocking, Is.EqualTo(true));
        }

        [Test]
        public void CanGetAutoDetectFrameworkTypes()
        {
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null, null, null);
            _baseOptions.AutoDetectFrameworkTypes.Returns(true);
            Assert.That(instance.AutoDetectFrameworkTypes, Is.EqualTo(true));
            _baseOptions.AutoDetectFrameworkTypes.Returns(false);
            Assert.That(instance.AutoDetectFrameworkTypes, Is.EqualTo(false));
        }

        [Test]
        public void CanGetAllowGenerationWithoutTargetProject()
        {
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null, null, null);
            _baseOptions.AllowGenerationWithoutTargetProject.Returns(true);
            Assert.That(instance.AllowGenerationWithoutTargetProject, Is.EqualTo(true));
            _baseOptions.AllowGenerationWithoutTargetProject.Returns(false);
            Assert.That(instance.AllowGenerationWithoutTargetProject, Is.EqualTo(false));
        }

        [Test]
        public void CanGetTestProjectNaming()
        {
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null, null, null);
            _baseOptions.TestProjectNaming.Returns("fgwerfgwe");
            Assert.That(instance.TestProjectNaming, Is.EqualTo("fgwerfgwe"));
        }

        [Test]
        public void CanGetTestFileNaming()
        {
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null, null, null);
            _baseOptions.TestFileNaming.Returns("retgerg");
            Assert.That(instance.TestFileNaming, Is.EqualTo("retgerg"));
        }

        [Test]
        public void CanGetTestTypeNaming()
        {
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null, null, null);
            _baseOptions.TestTypeNaming.Returns("5252532");
            Assert.That(instance.TestTypeNaming, Is.EqualTo("5252532"));
        }

        [Test]
        public void CanGetEmitUsingsOutsideNamespace()
        {
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null, null, null);
            _baseOptions.EmitUsingsOutsideNamespace.Returns(true);
            Assert.That(instance.EmitUsingsOutsideNamespace, Is.EqualTo(true));
            _baseOptions.EmitUsingsOutsideNamespace.Returns(false);
            Assert.That(instance.EmitUsingsOutsideNamespace, Is.EqualTo(false));
        }

        [Test]
        public void CanGetEmitTestsForInternals()
        {
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null, null, null);
            _baseOptions.EmitTestsForInternals.Returns(true);
            Assert.That(instance.EmitTestsForInternals, Is.EqualTo(true));
            _baseOptions.EmitTestsForInternals.Returns(false);
            Assert.That(instance.EmitTestsForInternals, Is.EqualTo(false));
        }

        [Test]
        public void CanGetPartialGenerationAllowed()
        {
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null, null, null);
            _baseOptions.PartialGenerationAllowed.Returns(true);
            Assert.That(instance.PartialGenerationAllowed, Is.EqualTo(true));
            _baseOptions.PartialGenerationAllowed.Returns(false);
            Assert.That(instance.PartialGenerationAllowed, Is.EqualTo(false));
        }

        [Test]
        public void CanGetEmitSubclassForProtectedMethods()
        {
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null, null, null);
            _baseOptions.EmitSubclassForProtectedMethods.Returns(true);
            Assert.That(instance.EmitSubclassForProtectedMethods, Is.EqualTo(true));
            _baseOptions.EmitSubclassForProtectedMethods.Returns(false);
            Assert.That(instance.EmitSubclassForProtectedMethods, Is.EqualTo(false));
        }

        [Test]
        public void CanGetAutomaticallyConfigureMocks()
        {
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null, null, null);
            _baseOptions.AutomaticallyConfigureMocks.Returns(true);
            Assert.That(instance.AutomaticallyConfigureMocks, Is.EqualTo(true));
            _baseOptions.AutomaticallyConfigureMocks.Returns(false);
            Assert.That(instance.AutomaticallyConfigureMocks, Is.EqualTo(false));
        }

        [Test]
        public void CanGetArrangeComment()
        {
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null, null, null);
            _baseOptions.ArrangeComment.Returns("ArrangeYo");
            Assert.That(instance.ArrangeComment, Is.EqualTo("ArrangeYo"));
        }

        [Test]
        public void CanGetActComment()
        {
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null, null, null);
            _baseOptions.ActComment.Returns("ActYo");
            Assert.That(instance.ActComment, Is.EqualTo("ActYo"));
        }

        [Test]
        public void CanGetAssertComment()
        {
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null, null, null);
            _baseOptions.AssertComment.Returns("AssertYo");
            Assert.That(instance.AssertComment, Is.EqualTo("AssertYo"));
        }

        [Test]
        public void CanGetUserInterfaceMode()
        {
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null, null, null);
            _baseOptions.UserInterfaceMode.Returns(UserInterfaceModes.OnlyWhenControlPressed);
            Assert.That(instance.UserInterfaceMode, Is.EqualTo(UserInterfaceModes.OnlyWhenControlPressed));
        }

        [Test]
        public void CanGetRememberManuallySelectedTargetProjectByDefault()
        {
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null, null, null);
            _baseOptions.RememberManuallySelectedTargetProjectByDefault.Returns(true);
            Assert.That(instance.RememberManuallySelectedTargetProjectByDefault, Is.EqualTo(true));
            _baseOptions.RememberManuallySelectedTargetProjectByDefault.Returns(false);
            Assert.That(instance.RememberManuallySelectedTargetProjectByDefault, Is.EqualTo(false));
        }

        [Test]
        public void CanGetFallbackTargetFinding()
        {
            var instance = new DetectedGenerationOptions(_baseOptions, null, null, null, null, null);
            _baseOptions.FallbackTargetFinding.Returns(FallbackTargetFindingMethod.TypeInAnyNamespace);
            Assert.That(instance.FallbackTargetFinding, Is.EqualTo(FallbackTargetFindingMethod.TypeInAnyNamespace));
        }
    }
}