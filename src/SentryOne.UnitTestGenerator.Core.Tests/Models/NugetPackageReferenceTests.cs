namespace SentryOne.UnitTestGenerator.Core.Tests.Models
{
    using System;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Models;

    [TestFixture]
    public class NugetPackageReferenceTests
    {
        private NugetPackageReference _testClass;
        private string _name;
        private string _version;

        [SetUp]
        public void SetUp()
        {
            _name = "TestValue371271030";
            _version = "TestValue67196216";
            _testClass = new NugetPackageReference(_name, _version);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new NugetPackageReference(_name, _version);
            Assert.That(instance, Is.Not.Null);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidName(string value)
        {
            Assert.Throws<ArgumentNullException>(() => new NugetPackageReference(value, "TestValue1390261023"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CanConstructWithInvalidVersion(string value)
        {
            Assert.That(new NugetPackageReference("TestValue1554148770", value).Version, Is.Null);
        }

        [Test]
        public void NameIsInitializedCorrectly()
        {
            Assert.That(_testClass.Name, Is.EqualTo(_name));
        }

        [Test]
        public void VersionIsInitializedCorrectly()
        {
            Assert.That(_testClass.Version, Is.EqualTo(_version));
        }
    }
}