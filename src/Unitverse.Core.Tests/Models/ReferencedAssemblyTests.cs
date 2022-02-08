namespace Unitverse.Core.Tests.Models
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class ReferencedAssemblyTests
    {
        private ReferencedAssembly _testClass;
        private string _assemblyName;
        private int _majorVersion;

        [SetUp]
        public void SetUp()
        {
            _assemblyName = "TestValue1339049862";
            _majorVersion = 1103154485;
            _testClass = new ReferencedAssembly(_assemblyName, _majorVersion);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new ReferencedAssembly(_assemblyName, _majorVersion);
            Assert.That(instance, Is.Not.Null);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidAssemblyName(string value)
        {
            Assert.Throws<ArgumentNullException>(() => new ReferencedAssembly(value, 1199382251));
        }

        [Test]
        public void AssemblyNameIsInitializedCorrectly()
        {
            Assert.That(_testClass.AssemblyName, Is.EqualTo(_assemblyName));
        }

        [Test]
        public void MajorVersionIsInitializedCorrectly()
        {
            Assert.That(_testClass.MajorVersion, Is.EqualTo(_majorVersion));
        }
    }
}