namespace SentryOne.UnitTestGenerator.Core.Tests.Helpers
{
    using System;
    using System.Linq;
    using NSubstitute;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Options;

    [TestFixture]
    public static class StandardReferenceHelperTests
    {
        [Test]
        [TestCase(TestFrameworkTypes.MsTest, MockingFrameworkType.FakeItEasy, "MSTest.TestFramework", "FakeItEasy")]
        [TestCase(TestFrameworkTypes.NUnit2, MockingFrameworkType.Moq, "NUnit", "Moq")]
        [TestCase(TestFrameworkTypes.NUnit3, MockingFrameworkType.NSubstitute, "NUnit", "NSubstitute")]
        [TestCase(TestFrameworkTypes.XUnit, MockingFrameworkType.RhinoMocks, "xunit", "RhinoMocks")]
        public static void CanCallGetReferencedAssemblies(TestFrameworkTypes testFramework, MockingFrameworkType mockingFramework, string expectedTestFramework, string expectedMockingFramework)
        {
            var options = Substitute.For<IUnitTestGeneratorOptions>();
            options.GenerationOptions.TestTypeNaming.Returns("{0}Tests");
            options.GenerationOptions.FrameworkType.Returns(testFramework);
            options.GenerationOptions.MockingFrameworkType.Returns(mockingFramework);
            var result = StandardReferenceHelper.GetReferencedNugetPackages(options);
            Assert.That(result.Any(x => string.Equals(x.Name, expectedTestFramework, StringComparison.OrdinalIgnoreCase)));
            Assert.That(result.Any(x => string.Equals(x.Name, expectedMockingFramework, StringComparison.OrdinalIgnoreCase)));
        }

        [Test]
        public static void CannotCallGetReferencedAssembliesWithNullOptions()
        {
            Assert.Throws<ArgumentNullException>(() => StandardReferenceHelper.GetReferencedNugetPackages(default(IUnitTestGeneratorOptions)));
        }
    }
}