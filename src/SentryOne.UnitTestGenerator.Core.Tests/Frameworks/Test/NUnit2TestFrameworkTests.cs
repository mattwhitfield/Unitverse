namespace SentryOne.UnitTestGenerator.Core.Tests.Frameworks.Test
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Frameworks.Test;
    using SentryOne.UnitTestGenerator.Core.Options;

    [TestFixture]
    public class NUnit2TestFrameworkTests
    {
        private NUnit2TestFramework _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new NUnit2TestFramework();
        }
    }
}