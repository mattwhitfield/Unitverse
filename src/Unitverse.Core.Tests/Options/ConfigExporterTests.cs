namespace Unitverse.Core.Tests.Options
{
    using Unitverse.Core.Options;
    using System;
    using NUnit.Framework;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.IO;

    [TestFixture]
    public static class ConfigExporterTests
    {
        private class TestObject1
        {
            public string Prop1 { get; set; }

            public bool ThisIsAProp { get; set; }
        }

        private class TestObject2
        {
            public string Prop2 { get; set; }

            public int ThisIsAnotherProp { get; set; }
        }

        [Test]
        public static void CanCallWriteTo()
        {
            var testObject1 = new TestObject1 { Prop1 = "prop1", ThisIsAProp = true };
            var testObject2 = new TestObject2 { Prop2 = "prop2", ThisIsAnotherProp = 52332 };
            var sources = new object[] { testObject1, testObject2 };
            var tempFile = Path.GetTempFileName();
            ConfigExporter.WriteTo(tempFile, sources);
            var text = File.ReadAllText(tempFile);
            File.Delete(tempFile);

            text.Should().Be(
                "[TestObject1]\r\n" +
                "Prop1=prop1\r\n" +
                "ThisIsAProp=True\r\n" +
                "\r\n" +
                "[TestObject2]\r\n" +
                "Prop2=prop2\r\n" +
                "ThisIsAnotherProp=52332\r\n");
        }

        [Test]
        public static void CannotCallWriteToWithNullSources()
        {
            FluentActions.Invoking(() => ConfigExporter.WriteTo("TestValue525457552", default(IEnumerable<object>))).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CannotCallWriteToWithInvalidTargetFileName(string value)
        {
            FluentActions.Invoking(() => ConfigExporter.WriteTo(value, new[] { new object(), new object(), new object() })).Should().Throw<ArgumentNullException>();
        }
    }
}