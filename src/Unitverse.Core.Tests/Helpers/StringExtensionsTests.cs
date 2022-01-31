namespace Unitverse.Core.Tests.Helpers
{
    using System.Linq;
    using NUnit.Framework;
    using Unitverse.Core.Helpers;

    [TestFixture]
    public static class StringExtensionsTests
    {
        [Test]
        public static void CanCallToCamelCase()
        {
            Assert.That("TestValue541057933".ToCamelCase(), Is.EqualTo("testValue541057933"));
            Assert.That("testValue541057933".ToCamelCase(), Is.EqualTo("testValue541057933"));
            Assert.That("T".ToCamelCase(), Is.EqualTo("t"));
            Assert.That("t".ToCamelCase(), Is.EqualTo("t"));
            Assert.That(string.Empty.ToCamelCase(), Is.EqualTo(string.Empty));
        }

        [Test]
        public static void CanCallToPascalCase()
        {
            Assert.That("TestValue541057933".ToPascalCase(), Is.EqualTo("TestValue541057933"));
            Assert.That("testValue541057933".ToPascalCase(), Is.EqualTo("TestValue541057933"));
            Assert.That("T".ToPascalCase(), Is.EqualTo("T"));
            Assert.That("t".ToPascalCase(), Is.EqualTo("T"));
            Assert.That(string.Empty.ToPascalCase(), Is.EqualTo(string.Empty));
        }

        [Test]
        public static void CanCallLines()
        {
            var input = "Test\r\nValue\r\nOne";
            var result = input.Lines();
            Assert.That(result.SequenceEqual(new[] { "Test", "Value", "One" }));
        }

        [TestCase(null)]
        [TestCase("")]
        public static void CanCallLinesWithInvalidInput(string value)
        {
            Assert.That(value.Lines(), Is.Empty);
        }
    }
}