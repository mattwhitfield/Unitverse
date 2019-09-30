namespace SentryOne.UnitTestGenerator.Options
{
    using System.ComponentModel;
    using Microsoft.VisualStudio.Shell;
    using SentryOne.UnitTestGenerator.Core.Options;

    public class VersioningOptions : DialogPage, IVersioningOptions
    {
        [Category("Test framework versions")]
        [DisplayName("NUnit 2 version")]
        [Description("The version of the NUnit 2 package to use. Leave this blank to use 2.6.4.")]
        public string NUnit2NugetPackageVersion { get; set; } = string.Empty;

        [Category("Test framework versions")]
        [DisplayName("NUnit 3 version")]
        [Description("The version of the NUnit 3 package to use. Leave this blank to use the latest available.")]
        public string NUnit3NugetPackageVersion { get; set; } = string.Empty;

        [Category("Test framework versions")]
        [DisplayName("xunit version")]
        [Description("The version of the xunit package to use. Leave this blank to use the latest available.")]
        public string XUnitNugetPackageVersion { get; set; } = string.Empty;

        [Category("Test framework versions")]
        [DisplayName("MsTest version")]
        [Description("The version of the MsTest package to use. Leave this blank to use the latest available.")]
        public string MsTestNugetPackageVersion { get; set; } = string.Empty;

        [Category("Mocking framework versions")]
        [DisplayName("Fake It Easy version")]
        [Description("The version of the Fake It Easy package to use. Leave this blank to use the latest available.")]
        public string FakeItEasyNugetPackageVersion { get; set; } = string.Empty;

        [Category("Mocking framework versions")]
        [DisplayName("Moq version")]
        [Description("The version of the Moq package to use. Leave this blank to use the latest available.")]
        public string MoqNugetPackageVersion { get; set; } = string.Empty;

        [Category("Mocking framework versions")]
        [DisplayName("NSubstitute version")]
        [Description("The version of the NSubstitute package to use. Leave this blank to use the latest available.")]
        public string NSubstituteNugetPackageVersion { get; set; } = string.Empty;

        [Category("Mocking framework versions")]
        [DisplayName("Rhino Mocks version")]
        [Description("The version of the Rhino Mocks package to use. Leave this blank to use the latest available.")]
        public string RhinoMocksNugetPackageVersion { get; set; } = string.Empty;
    }
}