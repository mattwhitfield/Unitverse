namespace SentryOne.UnitTestGenerator.Core.Strategies.PropertyGeneration
{
    using System;
    using System.Collections.Generic;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Models;

    public class PropertyGenerationStrategyFactory : ItemGenerationStrategyFactory<IPropertyModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public PropertyGenerationStrategyFactory(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        protected override IEnumerable<IGenerationStrategy<IPropertyModel>> Strategies => new IGenerationStrategy<IPropertyModel>[]
        {
            new NotifyPropertyChangedGenerationStrategy(_frameworkSet),
            new MultiConstructorInitializedPropertyGenerationStrategy(_frameworkSet),
            new SingleConstructorInitializedPropertyGenerationStrategy(_frameworkSet),
            new ReadWritePropertyGenerationStrategy(_frameworkSet),
            new ReadOnlyPropertyGenerationStrategy(_frameworkSet),
            new WriteOnlyPropertyGenerationStrategy(_frameworkSet),
        };
    }
}