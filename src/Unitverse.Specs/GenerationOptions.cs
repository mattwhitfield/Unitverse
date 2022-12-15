namespace Unitverse.Specs
{
    using System.Collections.Generic;
    using Unitverse.Core.Options;
    using Unitverse.Tests.Common;

    // ENHANCE - add fluent assertions to specs
    public static class GenerationOptions
    {
        public static IUnitTestGeneratorOptions Get(TestFrameworkTypes testFramework, MockingFrameworkType mockFramework)
        {
            var generationOptions = new DefaultGenerationOptions
            {
                FrameworkType = testFramework,
                MockingFrameworkType = mockFramework,
                UseFluentAssertions = false,
            };

            return new UnitTestGeneratorOptions(generationOptions, new DefaultNamingOptions(), new DefaultStrategyOptions(), false, new Dictionary<string, string>());
        }
    }
}