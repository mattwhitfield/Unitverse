namespace Unitverse.Core.Tests.Helpers
{
    using System;
    using NUnit.Framework;
    using Unitverse.Core.Helpers;

    [TestFixture]
    public static class NamespaceTransformTests
    {
        [Test]
        public static void CanCallCreate()
        {
            var sourceNameSpaceRoot = "TestValue1446777119";
            var targetNameSpaceRoot = "TestValue70491992";
            var result = NamespaceTransform.Create(sourceNameSpaceRoot, targetNameSpaceRoot);
            Assert.That(result("TestValue1446777119.Some.Other.Bit"), Is.EqualTo("TestValue70491992.Some.Other.Bit"));
            Assert.That(result("TestValue1446777119"), Is.EqualTo("TestValue70491992"));
            Assert.That(result(string.Empty), Is.EqualTo("TestValue70491992"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CannotCallCreateWithInvalidSourceNameSpaceRoot(string value)
        {
            Assert.Throws<ArgumentNullException>(() => NamespaceTransform.Create(value, "TestValue1220339710"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CannotCallCreateWithInvalidTargetNameSpaceRoot(string value)
        {
            Assert.Throws<ArgumentNullException>(() => NamespaceTransform.Create("TestValue2131007467", value));
        }
    }
}