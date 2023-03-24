namespace Unitverse.Core.Strategies.ClassLevelGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

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
            new CanInitializeGenerationStrategy(_frameworkSet),
            new NullPropertyCheckInitializerGenerationStrategy(_frameworkSet),
            new StringPropertyCheckInitializerGenerationStrategy(_frameworkSet),
        };

        public override NamingContext DecorateNamingContext(NamingContext baseContext, ClassModel classModel, ClassModel item)
        {
            return baseContext;
        }

        public override IEnumerable<ClassModel> GetItems(ClassModel model)
        {
            yield return model;
        }

        public override bool ShouldGenerate(ClassModel item)
        {
            return item.Constructors.Any(c => c.ShouldGenerate) || (!item.Constructors.Any() && item.Properties.Any(p => p.HasInit));
        }
    }
}