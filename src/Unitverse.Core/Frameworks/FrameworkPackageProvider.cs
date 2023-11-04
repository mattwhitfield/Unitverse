namespace Unitverse.Core.Frameworks
{
    using System;
    using System.Collections.Generic;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public static class FrameworkPackageProvider
    {
        public static IEnumerable<INugetPackageReference> Get(IGenerationOptions generationOptions)
        {
            if (generationOptions is null)
            {
                throw new ArgumentNullException(nameof(generationOptions));
            }

            yield return new NugetPackageReference("coverlet.collector", null);

            if (generationOptions.UseFluentAssertions)
            {
                yield return new NugetPackageReference("FluentAssertions", null);
            }
            else if (generationOptions.UseShouldly)
            {
                yield return new NugetPackageReference("Shouldly", null);
            }

            if (generationOptions.UseAutoFixture)
            {
                yield return new NugetPackageReference("AutoFixture", null);

                if (generationOptions.CanUseAutoFixtureForMocking())
                {
                    switch (generationOptions.MockingFrameworkType)
                    {
                        case MockingFrameworkType.NSubstitute:
                            yield return new NugetPackageReference("AutoFixture.AutoNSubstitute", null);
                            break;
                        case MockingFrameworkType.Moq:
                        case MockingFrameworkType.MoqAutoMock:
                            yield return new NugetPackageReference("AutoFixture.AutoMoq", null);
                            break;
                        case MockingFrameworkType.FakeItEasy:
                            yield return new NugetPackageReference("AutoFixture.AutoFakeItEasy", null);
                            break;
                    }
                }
            }

            switch (generationOptions.FrameworkType)
            {
                case TestFrameworkTypes.NUnit2:
                    yield return new NugetPackageReference("nunit", "2.7.1");
                    yield return new NugetPackageReference("NUnitTestAdapter", null);
                    break;
                case TestFrameworkTypes.NUnit3:
                case TestFrameworkTypes.NUnit3Lifecycle:
                    yield return new NugetPackageReference("nunit", null);
                    yield return new NugetPackageReference("NUnit3TestAdapter", null);
                    break;
                case TestFrameworkTypes.MsTest:
                    yield return new NugetPackageReference("MSTest.TestFramework", null);
                    yield return new NugetPackageReference("MSTest.TestAdapter", null);
                    break;
                case TestFrameworkTypes.XUnit:
                    yield return new NugetPackageReference("xunit", null);
                    yield return new NugetPackageReference("xunit.runner.visualstudio", null);
                    break;
            }

            switch (generationOptions.MockingFrameworkType)
            {
                case MockingFrameworkType.NSubstitute:
                    yield return new NugetPackageReference("NSubstitute", null);
                    break;
                case MockingFrameworkType.Moq:
                    yield return new NugetPackageReference("Moq", null);
                    break;
                case MockingFrameworkType.FakeItEasy:
                    yield return new NugetPackageReference("FakeItEasy", null);
                    break;
                case MockingFrameworkType.MoqAutoMock:
                    yield return new NugetPackageReference("Moq", null);
                    yield return new NugetPackageReference("Moq.AutoMock", null);
                    break;
                case MockingFrameworkType.JustMock:
                    yield return new NugetPackageReference("JustMock", null);
                    break;
            }
        }
    }
}
