namespace Unitverse.Core.Strategies.OperatorGeneration
{
    using System;
    using System.Collections.Generic;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Models;

    public class OperatorGenerationStrategyFactory : ItemGenerationStrategyFactory<IOperatorModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public OperatorGenerationStrategyFactory(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        protected override IEnumerable<IGenerationStrategy<IOperatorModel>> Strategies =>
            new IGenerationStrategy<IOperatorModel>[]
            {
                new CanCallOperatorGenerationStrategy(_frameworkSet),
                new NullParameterCheckOperatorGenerationStrategy(_frameworkSet),
            };
    }
}