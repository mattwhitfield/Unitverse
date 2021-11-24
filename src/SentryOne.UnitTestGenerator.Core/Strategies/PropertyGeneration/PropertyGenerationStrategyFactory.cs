namespace Unitverse.Core.Strategies.PropertyGeneration
{
    using System;
    using System.Collections.Generic;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Models;

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