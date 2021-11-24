namespace Unitverse.Core.Tests.Frameworks.Test
{
    using NUnit.Framework;
    using Unitverse.Core.Frameworks.Test;

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