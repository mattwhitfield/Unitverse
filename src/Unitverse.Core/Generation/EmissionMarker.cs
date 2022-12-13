namespace Unitverse.Core.Generation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unitverse.Core.Models;
    using Unitverse.Core.Strategies;
    using Unitverse.Core.Strategies.IndexerGeneration;
    using Unitverse.Core.Strategies.MethodGeneration;
    using Unitverse.Core.Strategies.OperatorGeneration;
    using Unitverse.Core.Strategies.PropertyGeneration;
    using Unitverse.Core.Strategies.ValueGeneration;

    internal static class EmissionMarker
    {
        public static void MarkEmittedItems(ModelGenerationContext context)
        {
            MarkEmittedItems(context, new MethodGenerationStrategyFactory(context.FrameworkSet), x => x.Methods);
            MarkEmittedItems(context, new OperatorGenerationStrategyFactory(context.FrameworkSet), x => x.Operators);
            MarkEmittedItems(context, new PropertyGenerationStrategyFactory(context.FrameworkSet), x => x.Properties);
            MarkEmittedItems(context, new IndexerGenerationStrategyFactory(context.FrameworkSet), x => x.Indexers);
            ValueGenerationStrategyFactory.ResetSeed();
        }

        private static void MarkEmittedItems<T>(ModelGenerationContext generationContext, ItemGenerationStrategyFactory<T> factory, Func<ClassModel, IEnumerable<T>> selector)
            where T : ITestableModel
        {
            foreach (var member in selector(generationContext.Model))
            {
                member.MarkedForGeneration =
                    member.ShouldGenerate &&
                    factory.CreateFor(member, generationContext.Model, generationContext.BaseNamingContext, generationContext.FrameworkSet.Options.StrategyOptions).Any();
            }
        }
    }
}
