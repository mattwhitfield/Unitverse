namespace Unitverse.Core.Tests.Frameworks.Test
{
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks.Test;
    using Unitverse.Core.Options;

    [TestFixture]
    public class XUnitTestFrameworkTests
    {
        private XUnitTestFramework _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new XUnitTestFramework(Substitute.For<IUnitTestGeneratorOptions>());
        }
    }
}