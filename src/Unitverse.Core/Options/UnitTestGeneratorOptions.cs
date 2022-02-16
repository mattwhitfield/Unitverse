namespace Unitverse.Core.Options
{
    using System;

    public class UnitTestGeneratorOptions : IUnitTestGeneratorOptions
    {
        public UnitTestGeneratorOptions(IGenerationOptions generationOptions, INamingOptions namingOptions, IStrategyOptions strategyOptions, bool statisticsCollectionEnabled)
        {
            GenerationOptions = generationOptions ?? throw new ArgumentNullException(nameof(generationOptions));
            NamingOptions = namingOptions ?? throw new ArgumentNullException(nameof(namingOptions));
            StrategyOptions = strategyOptions ?? throw new ArgumentNullException(nameof(strategyOptions));
            StatisticsCollectionEnabled = statisticsCollectionEnabled;
        }

        public IGenerationOptions GenerationOptions { get; }

        public INamingOptions NamingOptions { get; }

        public IStrategyOptions StrategyOptions { get; }

        public bool StatisticsCollectionEnabled { get; }
    }
}