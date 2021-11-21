namespace SentryOne.UnitTestGenerator.Core.Tests.Options
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Options;

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

            _testClass = new MutableGenerationOptions(_options);
            Assert.That(_testClass.FrameworkType, Is.EqualTo(_options.FrameworkType));
            Assert.That(_testClass.MockingFrameworkType, Is.EqualTo(_options.MockingFrameworkType));
            Assert.That(_testClass.AllowGenerationWithoutTargetProject, Is.EqualTo(_options.AllowGenerationWithoutTargetProject));
            Assert.That(_testClass.TestProjectNaming, Is.EqualTo(_options.TestProjectNaming));
            Assert.That(_testClass.TestFileNaming, Is.EqualTo(_options.TestFileNaming));
            Assert.That(_testClass.TestTypeNaming, Is.EqualTo(_options.TestTypeNaming));
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
    }
}