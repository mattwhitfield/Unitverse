namespace SentryOne.UnitTestGenerator.Core.Strategies.IndexerGeneration
{
    using System;
    using System.Collections.Generic;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Models;

    public class IndexerGenerationStrategyFactory : ItemGenerationStrategyFactory<IIndexerModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public IndexerGenerationStrategyFactory(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        protected override IEnumerable<IGenerationStrategy<IIndexerModel>> Strategies => new IGenerationStrategy<IIndexerModel>[]
        {
            new ReadOnlyIndexerGenerationStrategy(_frameworkSet),
            new ReadWriteIndexerGenerationStrategy(_frameworkSet),
            new WriteOnlyIndexerGenerationStrategy(_frameworkSet),
        };
    }
}