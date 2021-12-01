namespace Unitverse.Core.Tests.Helpers
{
    using Unitverse.Core.Helpers;
    using T = System.String;
    using System;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public static class EnumerableExtensionsTests
    {
        [Test]
        public static void CanCallEach()
        {
            var target = new List<string>();
            var source = new[] { "TestValue379072063", "TestValue1271184155", "TestValue1251609047" };
            Action<string> action = x => target.Add(x);
            source.Each<T>(action);
            Assert.That(source.SequenceEqual(target));
        }

        [Test]
        public static void CannotCallEachWithNullSource()
        {
            Assert.Throws<ArgumentNullException>(() => default(IEnumerable<T>).Each<T>(default(Action<T>)));
        }

        [Test]
        public static void CannotCallEachWithNullAction()
        {
            Assert.Throws<ArgumentNullException>(() => new[] { "TestValue1894072627", "TestValue386801265", "TestValue1810365181" }.Each<T>(default(Action<T>)));
        }
    }
}