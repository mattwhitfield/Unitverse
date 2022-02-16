namespace Unitverse.Core.Strategies
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public abstract class ItemGenerationStrategyFactory<T>
    {
        protected abstract IEnumerable<IGenerationStrategy<T>> Strategies { get; }

        public IEnumerable<MethodDeclarationSyntax> CreateFor(T item, ClassModel model, NamingContext namingContext, IStrategyOptions strategyOptions)
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