namespace SentryOne.UnitTestGenerator.Core.Options
{
    public interface IVersioningOptions
    {
        string NUnit2NugetPackageVersion { get; }

        string NUnit3NugetPackageVersion { get; }

        string XUnitNugetPackageVersion { get; }

        string MsTestNugetPackageVersion { get; }

        string FakeItEasyNugetPackageVersion { get; }

        string MoqNugetPackageVersion { get; }

        string NSubstituteNugetPackageVersion { get; }

        string RhinoMocksNugetPackageVersion { get; }
    }
}