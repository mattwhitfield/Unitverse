namespace SentryOne.UnitTestGenerator.Tests.Options.Internal
{
    using SentryOne.UnitTestGenerator.Options.Internal;
    using System;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Options;
    using SentryOne.UnitTestGenerator.Core.Options;

    [TestFixture]
    public class InternalGenerationOptionsTests
    {
        private InternalGenerationOptions _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new InternalGenerationOptions(new GenerationOptions { FrameworkType = TestFrameworkTypes.MsTest, MockingFrameworkType = MockingFrameworkType.Moq, CreateProjectAutomatically = false, AddReferencesAutomatically = true, AllowGenerationWithoutTargetProject = false, TestProjectNaming = "TestValue1764251425", TestFileNaming = "TestValue820933084", TestTypeNaming = "TestValue932260225" });
        }

        [Test]
        public void CanConstruct()
        {
            var options = new GenerationOptions { FrameworkType = TestFrameworkTypes.NUnit3, MockingFrameworkType = MockingFrameworkType.NSubstitute, CreateProjectAutomatically = true, AddReferencesAutomatically = false, AllowGenerationWithoutTargetProject = false, TestProjectNaming = "TestValue460938778", TestFileNaming = "TestValue2008872683", TestTypeNaming = "TestValue1485974937" };
            var result = new InternalGenerationOptions(options);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotConstructWithNullOptions()
        {
            Assert.Throws<ArgumentNullException>(() => new InternalGenerationOptions(default(GenerationOptions)));
        }

        [Test]
        public void ConstructorPerformsMapping()
        {
            var options = new GenerationOptions { FrameworkType = TestFrameworkTypes.NUnit3, MockingFrameworkType = MockingFrameworkType.Moq, CreateProjectAutomatically = true, AddReferencesAutomatically = true, AllowGenerationWithoutTargetProject = false, TestProjectNaming = "TestValue1144105985", TestFileNaming = "TestValue1370175379", TestTypeNaming = "TestValue1673837291" };
            var result = new InternalGenerationOptions(options);
            Assert.That(result.FrameworkType, Is.EqualTo(options.FrameworkType));
            Assert.That(result.MockingFrameworkType, Is.EqualTo(options.MockingFrameworkType));
            Assert.That(result.CreateProjectAutomatically, Is.EqualTo(options.CreateProjectAutomatically));
            Assert.That(result.AddReferencesAutomatically, Is.EqualTo(options.AddReferencesAutomatically));
            Assert.That(result.AllowGenerationWithoutTargetProject, Is.EqualTo(options.AllowGenerationWithoutTargetProject));
            Assert.That(result.TestProjectNaming, Is.EqualTo(options.TestProjectNaming));
            Assert.That(result.TestFileNaming, Is.EqualTo(options.TestFileNaming));
            Assert.That(result.TestTypeNaming, Is.EqualTo(options.TestTypeNaming));
        }

        [Test]
        public void CanSetAndGetFrameworkType()
        {
            var testValue = TestFrameworkTypes.NUnit2;
            _testClass.FrameworkType = testValue;
            Assert.That(_testClass.FrameworkType, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetMockingFrameworkType()
        {
            var testValue = MockingFrameworkType.FakeItEasy;
            _testClass.MockingFrameworkType = testValue;
            Assert.That(_testClass.MockingFrameworkType, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetCreateProjectAutomatically()
        {
            var testValue = false;
            _testClass.CreateProjectAutomatically = testValue;
            Assert.That(_testClass.CreateProjectAutomatically, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetAddReferencesAutomatically()
        {
            var testValue = true;
            _testClass.AddReferencesAutomatically = testValue;
            Assert.That(_testClass.AddReferencesAutomatically, Is.EqualTo(testValue));
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
            var testValue = "TestValue1235560686";
            _testClass.TestProjectNaming = testValue;
            Assert.That(_testClass.TestProjectNaming, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetTestFileNaming()
        {
            var testValue = "TestValue807903778";
            _testClass.TestFileNaming = testValue;
            Assert.That(_testClass.TestFileNaming, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetTestTypeNaming()
        {
            var testValue = "TestValue1233864319";
            _testClass.TestTypeNaming = testValue;
            Assert.That(_testClass.TestTypeNaming, Is.EqualTo(testValue));
        }
    }
}