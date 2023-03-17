namespace Unitverse.Tests.Common
{
    using System;
    using System.Collections.Generic;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Options;

    public static class DefaultFrameworkSet
    {
        private static IUnitTestGeneratorOptions Options { get; } =
            new UnitTestGeneratorOptions(new DefaultGenerationOptions(), new DefaultNamingOptions(), new DefaultStrategyOptions(), true, new Dictionary<string, string>());

        public static IFrameworkSet Create()
        {
            return FrameworkSetFactory.Create(Options);
        }

        public static IFrameworkSet CreateWithNamingOptions(Action<DefaultNamingOptions> mutator)
        {
            var namingOptions = new DefaultNamingOptions();
            mutator(namingOptions);

            var options = new UnitTestGeneratorOptions(new DefaultGenerationOptions(), namingOptions, new DefaultStrategyOptions(), true, new Dictionary<string, string>());

            return FrameworkSetFactory.Create(options);
        }
    }
}
