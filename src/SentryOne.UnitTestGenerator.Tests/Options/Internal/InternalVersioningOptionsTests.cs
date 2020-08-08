namespace SentryOne.UnitTestGenerator.Tests.Options.Internal
{
    using SentryOne.UnitTestGenerator.Options.Internal;
    using System;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Options;

    [TestFixture]
    public class InternalVersioningOptionsTests
    {
        private InternalVersioningOptions _testClass;
        private VersioningOptions _options;

        [SetUp]
        public void SetUp()
        {
            _options = new VersioningOptions { NUnit2NugetPackageVersion = "TestValue442570332", NUnit3NugetPackageVersion = "TestValue1492007113", XUnitNugetPackageVersion = "TestValue158211288", MsTestNugetPackageVersion = "TestValue1542947643", FakeItEasyNugetPackageVersion = "TestValue2052952067", MoqNugetPackageVersion = "TestValue1973157387", NSubstituteNugetPackageVersion = "TestValue2125707732", RhinoMocksNugetPackageVersion = "TestValue9980845" };
            _testClass = new InternalVersioningOptions(_options);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new InternalVersioningOptions(_options);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullOptions()
        {
            Assert.Throws<ArgumentNullException>(() => new InternalVersioningOptions(default(VersioningOptions)));
        }

        [Test]
        public void CanSetAndGetNUnit2NugetPackageVersion()
        {
            var testValue = "TestValue1576687523";
            _testClass.NUnit2NugetPackageVersion = testValue;
            Assert.That(_testClass.NUnit2NugetPackageVersion, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetNUnit3NugetPackageVersion()
        {
            var testValue = "TestValue1678457956";
            _testClass.NUnit3NugetPackageVersion = testValue;
            Assert.That(_testClass.NUnit3NugetPackageVersion, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetXUnitNugetPackageVersion()
        {
            var testValue = "TestValue1646992244";
            _testClass.XUnitNugetPackageVersion = testValue;
            Assert.That(_testClass.XUnitNugetPackageVersion, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetMsTestNugetPackageVersion()
        {
            var testValue = "TestValue1625777022";
            _testClass.MsTestNugetPackageVersion = testValue;
            Assert.That(_testClass.MsTestNugetPackageVersion, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetFakeItEasyNugetPackageVersion()
        {
            var testValue = "TestValue2116618120";
            _testClass.FakeItEasyNugetPackageVersion = testValue;
            Assert.That(_testClass.FakeItEasyNugetPackageVersion, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetMoqNugetPackageVersion()
        {
            var testValue = "TestValue634295081";
            _testClass.MoqNugetPackageVersion = testValue;
            Assert.That(_testClass.MoqNugetPackageVersion, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetNSubstituteNugetPackageVersion()
        {
            var testValue = "TestValue1735732753";
            _testClass.NSubstituteNugetPackageVersion = testValue;
            Assert.That(_testClass.NSubstituteNugetPackageVersion, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetRhinoMocksNugetPackageVersion()
        {
            var testValue = "TestValue1731365463";
            _testClass.RhinoMocksNugetPackageVersion = testValue;
            Assert.That(_testClass.RhinoMocksNugetPackageVersion, Is.EqualTo(testValue));
        }
    }
}