namespace SentryOne.UnitTestGenerator.Tests.Options
{
    using SentryOne.UnitTestGenerator.Options;
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class VersioningOptionsTests
    {
        private VersioningOptions _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new VersioningOptions();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new VersioningOptions();
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CanSetAndGetNUnit2NugetPackageVersion()
        {
            var testValue = "TestValue354452418";
            _testClass.NUnit2NugetPackageVersion = testValue;
            Assert.That(_testClass.NUnit2NugetPackageVersion, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetNUnit3NugetPackageVersion()
        {
            var testValue = "TestValue275178333";
            _testClass.NUnit3NugetPackageVersion = testValue;
            Assert.That(_testClass.NUnit3NugetPackageVersion, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetXUnitNugetPackageVersion()
        {
            var testValue = "TestValue1126599687";
            _testClass.XUnitNugetPackageVersion = testValue;
            Assert.That(_testClass.XUnitNugetPackageVersion, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetMsTestNugetPackageVersion()
        {
            var testValue = "TestValue1504140398";
            _testClass.MsTestNugetPackageVersion = testValue;
            Assert.That(_testClass.MsTestNugetPackageVersion, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetFakeItEasyNugetPackageVersion()
        {
            var testValue = "TestValue738232825";
            _testClass.FakeItEasyNugetPackageVersion = testValue;
            Assert.That(_testClass.FakeItEasyNugetPackageVersion, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetMoqNugetPackageVersion()
        {
            var testValue = "TestValue1496856104";
            _testClass.MoqNugetPackageVersion = testValue;
            Assert.That(_testClass.MoqNugetPackageVersion, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetNSubstituteNugetPackageVersion()
        {
            var testValue = "TestValue1026683030";
            _testClass.NSubstituteNugetPackageVersion = testValue;
            Assert.That(_testClass.NSubstituteNugetPackageVersion, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetRhinoMocksNugetPackageVersion()
        {
            var testValue = "TestValue301035158";
            _testClass.RhinoMocksNugetPackageVersion = testValue;
            Assert.That(_testClass.RhinoMocksNugetPackageVersion, Is.EqualTo(testValue));
        }
    }
}