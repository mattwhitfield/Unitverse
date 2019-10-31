namespace SentryOne.UnitTestGenerator.Specs.Strategies
{
    using SentryOne.UnitTestGenerator.Core.Options;

    public class VersionOptions : IVersioningOptions
    {
        public string NUnit2NugetPackageVersion { get; } = string.Empty;
        public string NUnit3NugetPackageVersion { get; } = string.Empty;
        public string XUnitNugetPackageVersion { get; } = string.Empty;
        public string MsTestNugetPackageVersion { get; } = string.Empty;
        public string FakeItEasyNugetPackageVersion { get; } = string.Empty;
        public string MoqNugetPackageVersion { get; } = string.Empty;
        public string NSubstituteNugetPackageVersion { get; } = string.Empty;
        public string RhinoMocksNugetPackageVersion { get; } = string.Empty;
    }
}