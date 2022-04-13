namespace Unitverse.Core.Tests.Helpers
{
    using Unitverse.Core.Helpers;
    using NUnit.Framework;

    [TestFixture]
    public class InertLoggerTests
    {
        private InertLogger _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new InertLogger();
        }

        [Test]
        public void CanCallInitialize()
        {
            // Assert
            Assert.DoesNotThrow(() => _testClass.Initialize());
        }

        [Test]
        public void CanCallLogMessage()
        {
            // Arrange
            var message = "TestValue582786776";

            // Assert
            Assert.DoesNotThrow(() => _testClass.LogMessage(message));
        }
    }
}