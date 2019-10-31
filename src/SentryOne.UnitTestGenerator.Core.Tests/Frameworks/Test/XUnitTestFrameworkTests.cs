namespace SentryOne.UnitTestGenerator.Core.Tests.Frameworks.Test
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Frameworks.Test;
    using SentryOne.UnitTestGenerator.Core.Options;

    [TestFixture]
    public class XUnitTestFrameworkTests
    {
        private XUnitTestFramework _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new XUnitTestFramework();
        }

        [Test]
        public void CannotCallReferencedNugetPackagesWithNullOptions()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.ReferencedNugetPackages(default(IVersioningOptions)).Consume());
        }
    }
}