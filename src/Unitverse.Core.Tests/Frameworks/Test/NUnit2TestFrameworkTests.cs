namespace Unitverse.Core.Tests.Frameworks.Test
{
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks.Test;
    using Unitverse.Core.Options;

    [TestFixture]
    public class NUnit2TestFrameworkTests
    {
        private NUnit2TestFramework _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new NUnit2TestFramework(Substitute.For<IUnitTestGeneratorOptions>());
        }
    }
}