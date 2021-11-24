namespace SentryOne.UnitTestGenerator.Core.Tests.Frameworks.Test
{
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Frameworks.Test;

    [TestFixture]
    public class XUnitTestFrameworkTests
    {
        private XUnitTestFramework _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new XUnitTestFramework();
        }
    }
}