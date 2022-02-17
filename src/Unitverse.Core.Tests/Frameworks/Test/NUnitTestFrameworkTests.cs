namespace Unitverse.Core.Tests.Frameworks.Test
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks.Test;
    using Unitverse.Core.Options;

    [TestFixture]
    public class NUnitTestFrameworkTests
    {
        private class TestNUnitTestFramework : NUnitTestFramework
        {
            public TestNUnitTestFramework(IUnitTestGeneratorOptions options) : base(options)
            {
            }

            public override AttributeSyntax SingleThreadedApartmentAttribute { get; }
        }

        private TestNUnitTestFramework _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new TestNUnitTestFramework(Substitute.For<IUnitTestGeneratorOptions>());
        }
    }
}