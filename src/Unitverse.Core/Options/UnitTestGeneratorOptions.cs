namespace Unitverse.Core.Options
{
    using System;

    public class UnitTestGeneratorOptions : IUnitTestGeneratorOptions
    {
        public UnitTestGeneratorOptions(IGenerationOptions generationOptions, INamingOptions namingOptions, bool statisticsCollectionEnabled)
        {
            GenerationOptions = generationOptions ?? throw new ArgumentNullException(nameof(generationOptions));
            NamingOptions = namingOptions ?? throw new ArgumentNullException(nameof(namingOptions));
            StatisticsCollectionEnabled = statisticsCollectionEnabled;
        }

        public IGenerationOptions GenerationOptions { get; }

        public INamingOptions NamingOptions { get; }

        public bool StatisticsCollectionEnabled { get; }
    }
}