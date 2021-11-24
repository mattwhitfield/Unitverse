namespace Unitverse.Core.Strategies
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Models;

    public abstract class ItemGenerationStrategyFactory<T>
    {
        protected abstract IEnumerable<IGenerationStrategy<T>> Strategies { get; }

        public IEnumerable<MethodDeclarationSyntax> CreateFor(T property, ClassModel model)
        {
            var strategies = Strategies.Where(x => x.CanHandle(property, model)).OrderByDescending(x => x.Priority);
            foreach (var strategy in strategies)
            {
                foreach (var method in strategy.Create(property, model))
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