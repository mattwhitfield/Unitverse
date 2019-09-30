namespace SentryOne.UnitTestGenerator.Core.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Options;

    public static class StandardReferenceHelper
    {
        public static IList<INugetPackageReference> GetReferencedNugetPackages(IUnitTestGeneratorOptions options)
        {
            var set = FrameworkSetFactory.Create(options);

            return set.TestFramework.ReferencedNugetPackages(options.VersioningOptions).Concat(set.MockingFramework.ReferencedNugetPackages(options.VersioningOptions)).ToList();
        }
    }
}
