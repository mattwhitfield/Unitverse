namespace Unitverse.Core.Tests.Helpers
{
    using NUnit.Framework;
    using Unitverse.Core.Helpers;

    [TestFixture]
    public static class IgnoreTests
    {
        [Test]
        public static void CanCallHResult()
        {
            var result = 1512227133;
            Assert.DoesNotThrow(() => Ignore.HResult(result));
        }
    }
}