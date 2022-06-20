namespace Unitverse.Core.Options
{
    using System.Collections.Generic;

    public interface IUnitTestGeneratorOptions
    {
        string? GetFieldSourceFileName(string fieldName);

        bool StatisticsCollectionEnabled { get; }

        IGenerationOptions GenerationOptions { get; }

        INamingOptions NamingOptions { get; }

        IStrategyOptions StrategyOptions { get; }

        IEnumerable<KeyValuePair<string, int>> SourceCounts { get; }
    }
}