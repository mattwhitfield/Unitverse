namespace Unitverse.Core.Strategies.MethodGeneration
{
    using System;
    using System.Collections.Generic;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public class MethodGenerationStrategyFactory : ItemGenerationStrategyFactory<IMethodModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public MethodGenerationStrategyFactory(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        protected override IEnumerable<IGenerationStrategy<IMethodModel>> Strategies =>
            new IGenerationStrategy<IMethodModel>[]
            {
                new CanCallMethodGenerationStrategy(_frameworkSet),
                new NullParameterCheckMethodGenerationStrategy(_frameworkSet),
                new StringParameterCheckMethodGenerationStrategy(_frameworkSet),
                new MappingMethodGenerationStrategy(_frameworkSet),
            };

        public override NamingContext DecorateNamingContext(NamingContext baseContext, ClassModel classModel, IMethodModel item)
        {
            return baseContext.WithMemberName(classModel.GetMethodUniqueName(item), item.Name);
        }

        public override IEnumerable<IMethodModel> GetItems(ClassModel model)
        {
            return model.Methods;
        }

        public override bool ShouldGenerate(IMethodModel item)
        {
            return item.ShouldGenerate;
        }
    }
}