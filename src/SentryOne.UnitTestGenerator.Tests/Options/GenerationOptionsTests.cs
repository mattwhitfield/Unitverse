namespace SentryOne.UnitTestGenerator.Tests.Options
{
    using SentryOne.UnitTestGenerator.Options;
    using System;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Options;

    [TestFixture]
    public class GenerationOptionsTests
    {
        private GenerationOptions _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new GenerationOptions();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new GenerationOptions();
            Assert.That(instance, Is.Not.Null);
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
            var testValue = false;
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
            var testValue = "TestValue106238151";
            _testClass.TestProjectNaming = testValue;
            Assert.That(_testClass.TestProjectNaming, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetTestFileNaming()
        {
            var testValue = "TestValue2110409327";
            _testClass.TestFileNaming = testValue;
            Assert.That(_testClass.TestFileNaming, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetTestTypeNaming()
        {
            var testValue = "TestValue18585459";
            _testClass.TestTypeNaming = testValue;
            Assert.That(_testClass.TestTypeNaming, Is.EqualTo(testValue));
        }
    }
}