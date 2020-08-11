namespace SentryOne.UnitTestGenerator.Core.Tests.Options
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Options;

    [TestFixture]
    public class MutableVersioningOptionsTests
    {
        private MutableVersioningOptions _testClass;
        private IVersioningOptions _options;

        [SetUp]
        public void SetUp()
        {
            _options = Substitute.For<IVersioningOptions>();
            _testClass = new MutableVersioningOptions(_options);
        }

        [Test]
        public void CanConstruct()
        {
            _options.NUnit2NugetPackageVersion.Returns("1");
            _options.NUnit3NugetPackageVersion.Returns("2");
            _options.XUnitNugetPackageVersion.Returns("3");
            _options.MsTestNugetPackageVersion.Returns("4");
            _options.FakeItEasyNugetPackageVersion.Returns("5");
            _options.MoqNugetPackageVersion.Returns("6");
            _options.NSubstituteNugetPackageVersion.Returns("7");
            _options.RhinoMocksNugetPackageVersion.Returns("8");

            _testClass = new MutableVersioningOptions(_options);
            Assert.That(_testClass.NUnit2NugetPackageVersion, Is.EqualTo(_options.NUnit2NugetPackageVersion));
            Assert.That(_testClass.NUnit3NugetPackageVersion, Is.EqualTo(_options.NUnit3NugetPackageVersion));
            Assert.That(_testClass.XUnitNugetPackageVersion, Is.EqualTo(_options.XUnitNugetPackageVersion));
            Assert.That(_testClass.MsTestNugetPackageVersion, Is.EqualTo(_options.MsTestNugetPackageVersion));
            Assert.That(_testClass.FakeItEasyNugetPackageVersion, Is.EqualTo(_options.FakeItEasyNugetPackageVersion));
            Assert.That(_testClass.MoqNugetPackageVersion, Is.EqualTo(_options.MoqNugetPackageVersion));
            Assert.That(_testClass.NSubstituteNugetPackageVersion, Is.EqualTo(_options.NSubstituteNugetPackageVersion));
            Assert.That(_testClass.RhinoMocksNugetPackageVersion, Is.EqualTo(_options.RhinoMocksNugetPackageVersion));
        }

        [Test]
        public void CannotConstructWithNullOptions()
        {
            Assert.Throws<ArgumentNullException>(() => new MutableVersioningOptions(default(IVersioningOptions)));
        }

        [Test]
        public void CanSetAndGetNUnit2NugetPackageVersion()
        {
            var testValue = "TestValue1062178473";
            _testClass.NUnit2NugetPackageVersion = testValue;
            Assert.That(_testClass.NUnit2NugetPackageVersion, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetNUnit3NugetPackageVersion()
        {
            var testValue = "TestValue1330446902";
            _testClass.NUnit3NugetPackageVersion = testValue;
            Assert.That(_testClass.NUnit3NugetPackageVersion, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetXUnitNugetPackageVersion()
        {
            var testValue = "TestValue152547983";
            _testClass.XUnitNugetPackageVersion = testValue;
            Assert.That(_testClass.XUnitNugetPackageVersion, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetMsTestNugetPackageVersion()
        {
            var testValue = "TestValue966405659";
            _testClass.MsTestNugetPackageVersion = testValue;
            Assert.That(_testClass.MsTestNugetPackageVersion, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetFakeItEasyNugetPackageVersion()
        {
            var testValue = "TestValue17506298";
            _testClass.FakeItEasyNugetPackageVersion = testValue;
            Assert.That(_testClass.FakeItEasyNugetPackageVersion, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetMoqNugetPackageVersion()
        {
            var testValue = "TestValue2013518628";
            _testClass.MoqNugetPackageVersion = testValue;
            Assert.That(_testClass.MoqNugetPackageVersion, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetNSubstituteNugetPackageVersion()
        {
            var testValue = "TestValue935604466";
            _testClass.NSubstituteNugetPackageVersion = testValue;
            Assert.That(_testClass.NSubstituteNugetPackageVersion, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetRhinoMocksNugetPackageVersion()
        {
            var testValue = "TestValue1080525660";
            _testClass.RhinoMocksNugetPackageVersion = testValue;
            Assert.That(_testClass.RhinoMocksNugetPackageVersion, Is.EqualTo(testValue));
        }
    }
}