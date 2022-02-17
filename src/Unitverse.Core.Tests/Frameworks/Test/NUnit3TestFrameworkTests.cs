namespace Unitverse.Core.Tests.Frameworks.Test
{
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks.Test;
    using Unitverse.Core.Options;

    [TestFixture]
    public class NUnit3TestFrameworkTests
    {
        private NUnit3TestFramework _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new NUnit3TestFramework(Substitute.For<IUnitTestGeneratorOptions>());
        }
    }
}