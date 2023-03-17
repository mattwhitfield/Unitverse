namespace Unitverse.Core.Tests.Options
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Options;
    using FluentAssertions;

    [TestFixture]
    public class MutableGenerationOptionsTests
    {
        private MutableGenerationOptions _testClass;
        private IGenerationOptions _options;

        [SetUp]
        public void SetUp()
        {
            _options = Substitute.For<IGenerationOptions>();
            _testClass = new MutableGenerationOptions(_options);
        }

        [Test]
        public void CanConstruct()
        {
            _options.FrameworkType.Returns(TestFrameworkTypes.NUnit3);
            _options.MockingFrameworkType.Returns(MockingFrameworkType.Moq);
            _options.AllowGenerationWithoutTargetProject.Returns(true);
            _options.TestProjectNaming.Returns("tpn");
            _options.TestFileNaming.Returns("tfn");
            _options.TestTypeNaming.Returns("ttn");
            _options.UseFluentAssertions.Returns(true);
            _options.UseShouldly.Returns(true);
            _options.EmitUsingsOutsideNamespace.Returns(true);
            _options.AutoDetectFrameworkTypes.Returns(false);
            _options.PartialGenerationAllowed.Returns(true);
            _options.EmitTestsForInternals.Returns(true);
            _options.EmitSubclassForProtectedMethods.Returns(true);
            _options.AutomaticallyConfigureMocks.Returns(true);
            _options.ArrangeComment.Returns("Arr");
            _options.ActComment.Returns("Act");
            _options.AssertComment.Returns("Assert");
            _options.UserInterfaceMode.Returns(UserInterfaceModes.Always);
            _options.RememberManuallySelectedTargetProjectByDefault.Returns(true);
            _options.FallbackTargetFinding.Returns(FallbackTargetFindingMethod.TypeInAnyNamespace);

            _testClass = new MutableGenerationOptions(_options);
            Assert.That(_testClass.FrameworkType, Is.EqualTo(_options.FrameworkType));
            Assert.That(_testClass.MockingFrameworkType, Is.EqualTo(_options.MockingFrameworkType));
            Assert.That(_testClass.AllowGenerationWithoutTargetProject, Is.EqualTo(_options.AllowGenerationWithoutTargetProject));
            Assert.That(_testClass.TestProjectNaming, Is.EqualTo(_options.TestProjectNaming));
            Assert.That(_testClass.TestFileNaming, Is.EqualTo(_options.TestFileNaming));
            Assert.That(_testClass.TestTypeNaming, Is.EqualTo(_options.TestTypeNaming));
            Assert.That(_testClass.UseFluentAssertions, Is.EqualTo(_options.UseFluentAssertions));
            Assert.That(_testClass.UseShouldly, Is.EqualTo(_options.UseShouldly));
            Assert.That(_testClass.EmitUsingsOutsideNamespace, Is.EqualTo(_options.EmitUsingsOutsideNamespace));
            Assert.That(_testClass.AutoDetectFrameworkTypes, Is.EqualTo(_options.AutoDetectFrameworkTypes));
            Assert.That(_testClass.PartialGenerationAllowed, Is.EqualTo(_options.PartialGenerationAllowed));
            Assert.That(_testClass.EmitTestsForInternals, Is.EqualTo(_options.EmitTestsForInternals));
            Assert.That(_testClass.EmitSubclassForProtectedMethods, Is.EqualTo(_options.EmitSubclassForProtectedMethods));
            Assert.That(_testClass.AutomaticallyConfigureMocks, Is.EqualTo(_options.AutomaticallyConfigureMocks));
            Assert.That(_testClass.ArrangeComment, Is.EqualTo(_options.ArrangeComment));
            Assert.That(_testClass.ActComment, Is.EqualTo(_options.ActComment));
            Assert.That(_testClass.AssertComment, Is.EqualTo(_options.AssertComment));
            Assert.That(_testClass.UserInterfaceMode, Is.EqualTo(_options.UserInterfaceMode));
            Assert.That(_testClass.RememberManuallySelectedTargetProjectByDefault, Is.EqualTo(_options.RememberManuallySelectedTargetProjectByDefault));
            Assert.That(_testClass.FallbackTargetFinding, Is.EqualTo(_options.FallbackTargetFinding));
        }

        [Test]
        public void CannotConstructWithNullOptions()
        {
            Assert.Throws<ArgumentNullException>(() => new MutableGenerationOptions(default(IGenerationOptions)));
        }

        [Test]
        public void CanSetAndGetFrameworkType()
        {
            var testValue = TestFrameworkTypes.XUnit;
            _testClass.FrameworkType = testValue;
            Assert.That(_testClass.FrameworkType, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetMockingFrameworkType()
        {
            var testValue = MockingFrameworkType.Moq;
            _testClass.MockingFrameworkType = testValue;
            Assert.That(_testClass.MockingFrameworkType, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetAllowGenerationWithoutTargetProject()
        {
            var testValue = true;
            _testClass.AllowGenerationWithoutTargetProject = testValue;
            Assert.That(_testClass.AllowGenerationWithoutTargetProject, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetTestProjectNaming()
        {
            var testValue = "TestValue2008824805";
            _testClass.TestProjectNaming = testValue;
            Assert.That(_testClass.TestProjectNaming, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetTestFileNaming()
        {
            var testValue = "TestValue1629131383";
            _testClass.TestFileNaming = testValue;
            Assert.That(_testClass.TestFileNaming, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetTestTypeNaming()
        {
            var testValue = "TestValue609453222";
            _testClass.TestTypeNaming = testValue;
            Assert.That(_testClass.TestTypeNaming, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetUseFluentAssertions()
        {
            var testValue = false;
            _testClass.UseFluentAssertions = testValue;
            _testClass.UseFluentAssertions.Should().Be(testValue);
        }

        [Test]
        public void CanSetAndGetUseShouldly()
        {
            var testValue = false;
            _testClass.UseShouldly = testValue;
            _testClass.UseShouldly.Should().Be(testValue);
        }

        [Test]
        public void CanSetAndGetAutoDetectFrameworkTypes()
        {
            var testValue = false;
            _testClass.AutoDetectFrameworkTypes = testValue;
            _testClass.AutoDetectFrameworkTypes.Should().Be(testValue);
        }

        [Test]
        public void CanSetAndGetEmitUsingsOutsideNamespace()
        {
            var testValue = false;
            _testClass.EmitUsingsOutsideNamespace = testValue;
            _testClass.EmitUsingsOutsideNamespace.Should().Be(testValue);
        }
    }
}