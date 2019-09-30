namespace SentryOne.UnitTestGenerator.Core.Tests.Models
{
    using System;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Models;

    [TestFixture]
    public class ReferencedAssemblyTests
    {
        private ReferencedAssembly _testClass;
        private string _name;
        private Version _version;
        private string _location;

        [SetUp]
        public void SetUp()
        {
            _name = "TestValue1966998320";
            _version = new Version();
            _location = "TestValue1955409253";
            _testClass = new ReferencedAssembly(_name, _version, _location);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new ReferencedAssembly(_name, _version, _location);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullVersion()
        {
            Assert.Throws<ArgumentNullException>(() => new ReferencedAssembly("TestValue454056071", default(Version), "TestValue722973067"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidName(string value)
        {
            Assert.Throws<ArgumentNullException>(() => new ReferencedAssembly(value, new Version(), "TestValue85487208"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidLocation(string value)
        {
            Assert.Throws<ArgumentNullException>(() => new ReferencedAssembly("TestValue1860691568", new Version(), value));
        }

        [Test]
        public void NameIsInitializedCorrectly()
        {
            Assert.That(_testClass.Name, Is.EqualTo(_name));
        }

        [Test]
        public void CanGetLocatableName()
        {
            Assert.That(_testClass.LocatableName, Is.EqualTo(_name));
            _testClass = new ReferencedAssembly("nunit.framework", new Version(3, 1), _location);
            Assert.That(_testClass.LocatableName, Is.EqualTo("nunit.framework(3)"));
        }

        [Test]
        public void VersionIsInitializedCorrectly()
        {
            Assert.That(_testClass.Version, Is.EqualTo(_version));
        }

        [Test]
        public void LocationIsInitializedCorrectly()
        {
            Assert.That(_testClass.Location, Is.EqualTo(_location));
        }
    }
}