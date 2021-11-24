namespace Unitverse.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    // TODO - tests
    public static class FrameworkDetection
    {
        private class Matcher<T>
        {
            public Matcher(string assemblyName, Func<int, bool> majorVersionMatch, T result)
            {
                AssemblyName = assemblyName;
                MajorVersionMatch = majorVersionMatch ?? (_ => true);
                Result = result;
            }

            public string AssemblyName { get; }

            public Func<int, bool> MajorVersionMatch { get; }

            public T Result { get; }

            public bool IsMatch(string referenceName, int referenceMajorVersion)
            {
                return string.Equals(AssemblyName, referenceName, StringComparison.OrdinalIgnoreCase) && MajorVersionMatch(referenceMajorVersion);
            }
        }

        private static readonly IList<Matcher<bool>> FluentAssertionsMatchers = new[] { new Matcher<bool>("FluentAssertions", null, true) };

        private static readonly IList<Matcher<TestFrameworkTypes>> TestFrameworkMatchers = new[]
        {
            new Matcher<TestFrameworkTypes>("nunit.framework", x => x == 2, TestFrameworkTypes.NUnit2),
            new Matcher<TestFrameworkTypes>("nunit.framework", x => x > 2, TestFrameworkTypes.NUnit3),
            new Matcher<TestFrameworkTypes>("xunit.assert", null, TestFrameworkTypes.XUnit),
            new Matcher<TestFrameworkTypes>("Microsoft.VisualStudio.TestPlatform.TestFramework", null, TestFrameworkTypes.MsTest),
        };

        private static readonly IList<Matcher<MockingFrameworkType>> MockingFrameworkMatchers = new[]
        {
            new Matcher<MockingFrameworkType>("FakeItEasy", null, MockingFrameworkType.FakeItEasy),
            new Matcher<MockingFrameworkType>("NSubstitute", null, MockingFrameworkType.NSubstitute),
            new Matcher<MockingFrameworkType>("Moq", null, MockingFrameworkType.Moq),
        };

        private static void Resolve<T>(ref T? value, IList<Matcher<T>> matchers, string referenceName, int referenceMajorVersion)
            where T : struct
        {
            if (!value.HasValue)
            {
                var match = matchers.FirstOrDefault(x => x.IsMatch(referenceName, referenceMajorVersion));
                if (match != null)
                {
                    value = match.Result;
                }
            }
        }

        public static IGenerationOptions ResolveTargetFrameworks(IEnumerable<ReferencedAssembly> referencedAssemblies, IGenerationOptions baseOptions)
        {
            if (!baseOptions.AutoDetectFrameworkTypes)
            {
                return baseOptions;
            }

            bool? fluentAssertionsPresent = null;
            TestFrameworkTypes? detectedTestFramework = null;
            MockingFrameworkType? detectedMockingFramework = null;

            foreach (var reference in referencedAssemblies)
            {
                Resolve(ref fluentAssertionsPresent, FluentAssertionsMatchers, reference.AssemblyName, reference.MajorVersion);
                Resolve(ref detectedTestFramework, TestFrameworkMatchers, reference.AssemblyName, reference.MajorVersion);
                Resolve(ref detectedMockingFramework, MockingFrameworkMatchers, reference.AssemblyName, reference.MajorVersion);

                if (fluentAssertionsPresent.HasValue && detectedTestFramework.HasValue && detectedMockingFramework.HasValue)
                {
                    break;
                }
            }

            return new DetectedGenerationOptions(baseOptions, fluentAssertionsPresent, detectedTestFramework, detectedMockingFramework);
        }
    }
}
