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
            Assert.That(!result.IsNewerThan(SemanticVersion.Parse("1.2.3-alpha.2")));
            Assert.That(!result.IsNewerThan(SemanticVersion.Parse("1.2.3-beta.1")));
            Assert.That(result.IsNewerThan(null));

            var version2 = "1.2.3";
            var result2 = SemanticVersion.Parse(version2);
            Assert.That(result2.IsNewerThan(SemanticVersion.Parse("1.2.3-alpha.1")));
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