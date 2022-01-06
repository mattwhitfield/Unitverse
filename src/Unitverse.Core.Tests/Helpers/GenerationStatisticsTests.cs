namespace Unitverse.Core.Tests.Helpers
{
    using Unitverse.Core.Helpers;
    using System;
    using NUnit.Framework;
    using FluentAssertions;

    [TestFixture]
    public class GenerationStatisticsTests
    {
        private GenerationStatistics _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new GenerationStatistics();
        }

        [Test]
        public void CanSetAndGetInterfacesMocked()
        {
            var testValue = 835620083L;
            _testClass.InterfacesMocked = testValue;
            _testClass.InterfacesMocked.Should().Be(testValue);
        }

        [Test]
        public void CanSetAndGetTypesConstructed()
        {
            var testValue = 1841568590L;
            _testClass.TypesConstructed = testValue;
            _testClass.TypesConstructed.Should().Be(testValue);
        }

        [Test]
        public void CanSetAndGetValuesGenerated()
        {
            var testValue = 1109033682L;
            _testClass.ValuesGenerated = testValue;
            _testClass.ValuesGenerated.Should().Be(testValue);
        }

        [Test]
        public void CanSetAndGetTestClassesGenerated()
        {
            var testValue = 1204831756L;
            _testClass.TestClassesGenerated = testValue;
            _testClass.TestClassesGenerated.Should().Be(testValue);
        }

        [Test]
        public void CanSetAndGetTestMethodsGenerated()
        {
            var testValue = 2012767536L;
            _testClass.TestMethodsGenerated = testValue;
            _testClass.TestMethodsGenerated.Should().Be(testValue);
        }

        [Test]
        public void CanSetAndGetTestMethodsRegenerated()
        {
            var testValue = 1313966917L;
            _testClass.TestMethodsRegenerated = testValue;
            _testClass.TestMethodsRegenerated.Should().Be(testValue);
        }
    }
}