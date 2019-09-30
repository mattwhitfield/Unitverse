namespace SentryOne.UnitTestGenerator.Core.Frameworks.Test
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Options;

    public class NUnit2TestFramework : NUnitTestFramework
    {
        public override AttributeSyntax SingleThreadedApartmentAttribute => Generate.Attribute("RequiresSTA");

        public override IEnumerable<INugetPackageReference> ReferencedNugetPackages(IVersioningOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            // NUnit is a special case - because we want to support an older version. Hence if the version specified is 'latest' (i.e. null) we go with 2.6.4
            var version = options.NUnit2NugetPackageVersion;

            if (string.IsNullOrWhiteSpace(version))
            {
                version = "2.6.4";
            }

            yield return new NugetPackageReference("NUnit", version);
        }
    }
}