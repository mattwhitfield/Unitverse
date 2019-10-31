namespace SentryOne.UnitTestGenerator.Core.Tests.Frameworks.Test
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Frameworks.Test;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Options;

    [TestFixture]
    public class NUnitTestFrameworkTests
    {
        private class TestNUnitTestFramework : NUnitTestFramework
        {
            public override IEnumerable<INugetPackageReference> ReferencedNugetPackages(IVersioningOptions options)
            {
                return default(IEnumerable<INugetPackageReference>);
            }

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