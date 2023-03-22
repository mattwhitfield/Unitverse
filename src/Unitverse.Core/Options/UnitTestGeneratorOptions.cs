namespace Unitverse.Core.Options
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UnitTestGeneratorOptions : IUnitTestGeneratorOptions
    {
        private readonly Dictionary<string, ConfigurationSource> _configurationSources;

        public UnitTestGeneratorOptions(
            IGenerationOptions generationOptions,
            INamingOptions namingOptions,
            IStrategyOptions strategyOptions,
            bool statisticsCollectionEnabled,
            Dictionary<string, ConfigurationSource> configurationSources,
            Dictionary<string, string>? projectMappings = null)
        {
            GenerationOptions = generationOptions ?? throw new ArgumentNullException(nameof(generationOptions));
            NamingOptions = namingOptions ?? throw new ArgumentNullException(nameof(namingOptions));
            StrategyOptions = strategyOptions ?? throw new ArgumentNullException(nameof(strategyOptions));
            StatisticsCollectionEnabled = statisticsCollectionEnabled;
            _configurationSources = configurationSources ?? throw new ArgumentNullException(nameof(configurationSources));
            ProjectMappings = new Dictionary<string, string>(projectMappings ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase);
        }

        public IGenerationOptions GenerationOptions { get; }

        public INamingOptions NamingOptions { get; }

        public IStrategyOptions StrategyOptions { get; }

        public bool StatisticsCollectionEnabled { get; }

        public IEnumerable<KeyValuePair<string, int>> SourceCounts => _configurationSources.Where(x => x.Value.SourceType == ConfigurationSourceType.ConfigurationFile).GroupBy(x => x.Value.FileName).Select(x => new KeyValuePair<string, int>(x.Key ?? string.Empty, x.Count()));

        public Dictionary<string, string> ProjectMappings { get; }

        public ConfigurationSource GetFieldSource(string fieldName)
        {
            if (_configurationSources.TryGetValue(fieldName, out var value))
            {
                return value;
            }

            return new ConfigurationSource(ConfigurationSourceType.VisualStudio);
        }

        public IUnitTestGeneratorOptions OverrideGenerationOption(Action<MutableGenerationOptions> configure)
        {
            var mutable = new MutableGenerationOptions(GenerationOptions);
            configure(mutable);
            return new UnitTestGeneratorOptions(mutable, NamingOptions, StrategyOptions, StatisticsCollectionEnabled, _configurationSources, ProjectMappings);
        }
    }
}