namespace Unitverse.Core.Tests.Options.Editing
{
    using Unitverse.Core.Options.Editing;
    using System;
    using NUnit.Framework;
    using FluentAssertions;

    [TestFixture]
    public class TargetSelectionRegisterTests
    {
        private TargetSelectionRegister _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = TargetSelectionRegister.Instance;
        }

        [Test]
        public void CanCallSetAndGetTargetFor()
        {
            // Arrange
            var sourceProjectUniqueName = "TestValue1610880985";

            _testClass.GetTargetFor(sourceProjectUniqueName).Should().BeNull();
            _testClass.SetTargetFor(sourceProjectUniqueName, "spuds");
            _testClass.GetTargetFor(sourceProjectUniqueName).Should().Be("spuds");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallGetTargetForWithInvalidSourceProjectUniqueName(string value)
        {
            FluentActions.Invoking(() => _testClass.GetTargetFor(value)).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallSetTargetForWithInvalidSourceProjectUniqueName(string value)
        {
            FluentActions.Invoking(() => _testClass.SetTargetFor(value, "TestValue1910985443")).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallSetTargetForWithInvalidTargetProjectName(string value)
        {
            FluentActions.Invoking(() => _testClass.SetTargetFor("TestValue145029784", value)).Should().Throw<ArgumentNullException>();
        }
    }
}