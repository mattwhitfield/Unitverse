namespace Unitverse.Core.Tests.Frameworks.Test
{
    using NUnit.Framework;
    using Unitverse.Core.Frameworks.Test;

    [TestFixture]
    public class NUnit3TestFrameworkTests
    {
        private NUnit3TestFramework _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new NUnit3TestFramework();
        }
    }
}