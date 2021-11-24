namespace Unitverse.Core.Tests.Frameworks.Test
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks.Test;

    [TestFixture]
    public class NUnitTestFrameworkTests
    {
        private class TestNUnitTestFramework : NUnitTestFramework
        {
            public override AttributeSyntax SingleThreadedApartmentAttribute { get; }
        }

        private TestNUnitTestFramework _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new TestNUnitTestFramework();
        }
    }
}