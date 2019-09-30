namespace SentryOne.UnitTestGenerator.Core.Tests.Models
{
    using System;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Models;

    [TestFixture]
    public class SemanticVersionTests
    {
        [Test]
        public void CanCallParseAndCompare()
        {
            var version = "1.2.3-alpha.1";
            var result = SemanticVersion.Parse(version);
            Assert.That(result.IsNewerThan(SemanticVersion.Parse("1.2.2")));
            Assert.That(!result.IsNewerThan(SemanticVersion.Parse("1.2.3")));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallParseWithInvalidVersion(string value)
        {
            Assert.Throws<ArgumentNullException>(() => SemanticVersion.Parse(value));
        }
    }
}