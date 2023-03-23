namespace Unitverse.Core.Options
{
    using System;
    using System.Collections.Generic;

    public interface IUnitTestGeneratorOptions
    {
        ConfigurationSource GetFieldSource(string fieldName);

        bool StatisticsCollectionEnabled { get; }

        IGenerationOptions GenerationOptions { get; }

        INamingOptions NamingOptions { get; }

        IStrategyOptions StrategyOptions { get; }

        Dictionary<string, string> ProjectMappings { get; }

        IEnumerable<KeyValuePair<string, int>> SourceCounts { get; }

        IUnitTestGeneratorOptions OverrideGenerationOption(Action<MutableGenerationOptions> configure);
    }
}