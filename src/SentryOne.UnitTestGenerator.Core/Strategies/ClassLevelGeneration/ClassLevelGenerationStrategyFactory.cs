namespace SentryOne.UnitTestGenerator.Core.Strategies.ClassLevelGeneration
{
    using System;
    using System.Collections.Generic;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Models;

    public class ClassLevelGenerationStrategyFactory : ItemGenerationStrategyFactory<ClassModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public ClassLevelGenerationStrategyFactory(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        protected override IEnumerable<IGenerationStrategy<ClassModel>> Strategies => new List<IGenerationStrategy<ClassModel>>
        {
            new CanConstructNoConstructorGenerationStrategy(_frameworkSet),
            new CanConstructSingleConstructorGenerationStrategy(_frameworkSet),
            new CanConstructMultiConstructorGenerationStrategy(_frameworkSet),
            new NullParameterCheckConstructorGenerationStrategy(_frameworkSet),
            new StringParameterCheckConstructorGenerationStrategy(_frameworkSet),
        };
    }
}