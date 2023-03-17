namespace Unitverse.Core.Tests.Helpers
{
    using Unitverse.Core.Helpers;
    using System;
    using NUnit.Framework;
    using NSubstitute;
    using System.Collections.Generic;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    [TestFixture]
    public static class FrameworkDetectionTests
    {
        [TestCase("nunit.framework", 2, TestFrameworkTypes.XUnit, TestFrameworkTypes.NUnit2)]
        [TestCase("nunit.framework", 3, TestFrameworkTypes.XUnit, TestFrameworkTypes.NUnit3)]
        [TestCase("xunit.assert", 3, TestFrameworkTypes.MsTest, TestFrameworkTypes.XUnit)]
        [TestCase("Microsoft.VisualStudio.TestPlatform.TestFramework", 3, TestFrameworkTypes.XUnit, TestFrameworkTypes.MsTest)]
        [TestCase("fred", 3, TestFrameworkTypes.XUnit, TestFrameworkTypes.XUnit)]
        [TestCase("fred", 3, TestFrameworkTypes.NUnit3, TestFrameworkTypes.NUnit3)]
        public static void ResolveTargetFrameworksIdentifiesTestFrameworks(string assemblyName, int version, TestFrameworkTypes baseType, TestFrameworkTypes detectedType)
        {
            var referencedAssemblies = new[] { new ReferencedAssembly(assemblyName, version), new ReferencedAssembly("TestValue297538669", 369638268), new ReferencedAssembly("TestValue542242818", 1475656439) };
            var baseOptions = Substitute.For<IGenerationOptions>();
            baseOptions.FrameworkType.Returns(baseType);
            baseOptions.AutoDetectFrameworkTypes.Returns(true);
            var result = FrameworkDetection.ResolveTargetFrameworks(referencedAssemblies, baseOptions);
            Assert.That(result.FrameworkType, Is.EqualTo(detectedType));
        }

        [TestCase("FakeItEasy", MockingFrameworkType.NSubstitute, MockingFrameworkType.FakeItEasy)]
        [TestCase("NSubstitute", MockingFrameworkType.Moq, MockingFrameworkType.NSubstitute)]
        [TestCase("Moq", MockingFrameworkType.NSubstitute, MockingFrameworkType.Moq)]
        [TestCase("Moq.AutoMock", MockingFrameworkType.NSubstitute, MockingFrameworkType.MoqAutoMock)]
        [TestCase("fred", MockingFrameworkType.NSubstitute, MockingFrameworkType.NSubstitute)]
        [TestCase("fred", MockingFrameworkType.Moq, MockingFrameworkType.Moq)]
        public static void ResolveTargetFrameworksIdentifiesMockingFrameworks(string assemblyName, MockingFrameworkType baseType, MockingFrameworkType detectedType)
        {
            var referencedAssemblies = new[] { new ReferencedAssembly(assemblyName, 1), new ReferencedAssembly("TestValue297538669", 369638268), new ReferencedAssembly("TestValue542242818", 1475656439) };
            var baseOptions = Substitute.For<IGenerationOptions>();
            baseOptions.MockingFrameworkType.Returns(baseType);
            baseOptions.AutoDetectFrameworkTypes.Returns(true);
            var result = FrameworkDetection.ResolveTargetFrameworks(referencedAssemblies, baseOptions);
            Assert.That(result.MockingFrameworkType, Is.EqualTo(detectedType));
        }

        [TestCase("FluentAssertions", false, true)]
        [TestCase("fred", false, false)]
        [TestCase("fred", true, true)]
        public static void ResolveTargetFrameworksIdentifiesMockingFrameworks(string assemblyName, bool baseShouldUse, bool detectedShouldUseFluentAssertions)
        {
            var referencedAssemblies = new[] { new ReferencedAssembly(assemblyName, 1), new ReferencedAssembly("TestValue297538669", 369638268), new ReferencedAssembly("TestValue542242818", 1475656439) };
            var baseOptions = Substitute.For<IGenerationOptions>();
            baseOptions.UseFluentAssertions.Returns(baseShouldUse);
            baseOptions.AutoDetectFrameworkTypes.Returns(true);
            var result = FrameworkDetection.ResolveTargetFrameworks(referencedAssemblies, baseOptions);
            Assert.That(result.UseFluentAssertions, Is.EqualTo(detectedShouldUseFluentAssertions));
        }

        [TestCase("Shouldly", false, true)]
        [TestCase("fred", false, false)]
        [TestCase("fred", true, true)]
        public static void ResolveTargetFrameworksIdentifiesMockingFrameworksWithShouldly(string assemblyName, bool baseShouldUse, bool detectedShouldUseShouldly)
        {
            var referencedAssemblies = new[] { new ReferencedAssembly(assemblyName, 1), new ReferencedAssembly("TestValue297538669", 369638268), new ReferencedAssembly("TestValue542242818", 1475656439) };
            var baseOptions = Substitute.For<IGenerationOptions>();
            baseOptions.UseShouldly.Returns(baseShouldUse);
            baseOptions.AutoDetectFrameworkTypes.Returns(true);
            var result = FrameworkDetection.ResolveTargetFrameworks(referencedAssemblies, baseOptions);
            Assert.That(result.UseShouldly, Is.EqualTo(detectedShouldUseShouldly));
        }


        [Test]
        public static void ResolveTargetFrameworksReturnsBaseOptionsIfAutoDetectionDisabled()
        {
            var referencedAssemblies = new[] { new ReferencedAssembly("TestValue794932277", 1164213418), new ReferencedAssembly("TestValue297538669", 369638268), new ReferencedAssembly("TestValue542242818", 1475656439) };
            var baseOptions = Substitute.For<IGenerationOptions>();
            baseOptions.AutoDetectFrameworkTypes.Returns(false);
            var result = FrameworkDetection.ResolveTargetFrameworks(referencedAssemblies, baseOptions);
            Assert.That(result, Is.SameAs(baseOptions));
        }

        [Test]
        public static void CannotCallResolveTargetFrameworksWithNullReferencedAssemblies()
        {
            Assert.Throws<ArgumentNullException>(() => FrameworkDetection.ResolveTargetFrameworks(default(IEnumerable<ReferencedAssembly>), Substitute.For<IGenerationOptions>()));
        }

        [Test]
        public static void CannotCallResolveTargetFrameworksWithNullBaseOptions()
        {
            Assert.Throws<ArgumentNullException>(() => FrameworkDetection.ResolveTargetFrameworks(new[] { new ReferencedAssembly("TestValue797128655", 1676751731), new ReferencedAssembly("TestValue1783600907", 1279137469), new ReferencedAssembly("TestValue198342193", 718576829) }, default(IGenerationOptions)));
        }
    }
}