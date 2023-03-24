namespace Unitverse.Core.Strategies
{
    using System.Collections.Generic;
    using System.Linq;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public abstract class ItemGenerationStrategyFactory<T>
    {
        protected abstract IEnumerable<IGenerationStrategy<T>> Strategies { get; }

        public abstract bool ShouldGenerate(T item);

        public abstract NamingContext DecorateNamingContext(NamingContext baseContext, ClassModel classModel, T item);

        public abstract IEnumerable<T> GetItems(ClassModel model);

        public IEnumerable<SectionedMethodHandler> CreateFor(T item, ClassModel model, NamingContext namingContext, IStrategyOptions strategyOptions)
        {
            var strategies = Strategies.Where(x => x.CanHandle(item, model)).OrderByDescending(x => x.Priority);
            foreach (var strategy in strategies)
            {
                if (!strategy.IsEnabled(strategyOptions))
                {
                    continue;
                }

                foreach (var method in strategy.Create(item, model, namingContext))
                {
                    yield return method;
                }

                if (strategy.IsExclusive)
                {
                    break;
                }
            }
        }
    }
}