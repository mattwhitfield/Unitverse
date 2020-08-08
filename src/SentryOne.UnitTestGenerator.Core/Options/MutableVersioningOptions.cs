namespace SentryOne.UnitTestGenerator.Core.Options
{
    using System;

    public class MutableVersioningOptions : IVersioningOptions
    {
        public MutableVersioningOptions(IVersioningOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            NUnit2NugetPackageVersion = options.NUnit2NugetPackageVersion;
            NUnit3NugetPackageVersion = options.NUnit3NugetPackageVersion;
            XUnitNugetPackageVersion = options.XUnitNugetPackageVersion;
            MsTestNugetPackageVersion = options.MsTestNugetPackageVersion;
            FakeItEasyNugetPackageVersion = options.FakeItEasyNugetPackageVersion;
            MoqNugetPackageVersion = options.MoqNugetPackageVersion;
            NSubstituteNugetPackageVersion = options.NSubstituteNugetPackageVersion;
            RhinoMocksNugetPackageVersion = options.RhinoMocksNugetPackageVersion;
        }

        public string NUnit2NugetPackageVersion { get; set; }

        public string NUnit3NugetPackageVersion { get; set; }

        public string XUnitNugetPackageVersion { get; set; }

        public string MsTestNugetPackageVersion { get; set; }

        public string FakeItEasyNugetPackageVersion { get; set; }

        public string MoqNugetPackageVersion { get; set; }

        public string NSubstituteNugetPackageVersion { get; set; }

        public string RhinoMocksNugetPackageVersion { get; set; }
    }
}