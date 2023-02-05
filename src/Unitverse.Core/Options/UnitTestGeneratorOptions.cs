namespace Unitverse.Core.Options
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UnitTestGeneratorOptions : IUnitTestGeneratorOptions
    {
        private readonly Dictionary<string, string> _membersSetByFilename;

        public UnitTestGeneratorOptions(IGenerationOptions generationOptions, INamingOptions namingOptions, IStrategyOptions strategyOptions, bool statisticsCollectionEnabled, Dictionary<string, string> membersSetByFilename)
        {
            GenerationOptions = generationOptions ?? throw new ArgumentNullException(nameof(generationOptions));
            NamingOptions = namingOptions ?? throw new ArgumentNullException(nameof(namingOptions));
            StrategyOptions = strategyOptions ?? throw new ArgumentNullException(nameof(strategyOptions));
            StatisticsCollectionEnabled = statisticsCollectionEnabled;
            _membersSetByFilename = membersSetByFilename ?? throw new ArgumentNullException(nameof(membersSetByFilename));
        }

        public IGenerationOptions GenerationOptions { get; }

        public INamingOptions NamingOptions { get; }

        public IStrategyOptions StrategyOptions { get; }

        public bool StatisticsCollectionEnabled { get; }

        public IEnumerable<KeyValuePair<string, int>> SourceCounts => _membersSetByFilename.GroupBy(x => x.Value).Select(x => new KeyValuePair<string, int>(x.Key, x.Count()));

        public string? GetFieldSourceFileName(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                throw new ArgumentNullException(nameof(fieldName));
            }

            if (_membersSetByFilename.TryGetValue(fieldName, out var value))
            {
                return value;
            }

            return null;
        }

        public IUnitTestGeneratorOptions OverrideGenerationOption(Action<MutableGenerationOptions> configure)
        {
            var mutable = new MutableGenerationOptions(GenerationOptions);
            configure(mutable);
            return new UnitTestGeneratorOptions(mutable, NamingOptions, StrategyOptions, StatisticsCollectionEnabled, _membersSetByFilename);
        }
    }
}