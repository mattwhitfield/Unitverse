namespace SentryOne.UnitTestGenerator.Core.Strategies.InterfaceGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;

    public class InterfaceGenerationStrategyFactory : ItemGenerationStrategyFactory<ClassModel>
    {
        private static readonly Dictionary<string, Func<IFrameworkSet, IGenerationStrategy<ClassModel>>> StrategyFactories = new Dictionary<string, Func<IFrameworkSet, IGenerationStrategy<ClassModel>>>
        {
            { "System.IComparable", frameworkSet => new ComparableGenerationStrategy(frameworkSet) },
            { "System.Collections.Generic.IEnumerable", frameworkSet => new EnumerableGenerationStrategy(frameworkSet) },
        };

        private readonly IFrameworkSet _frameworkSet;

        public InterfaceGenerationStrategyFactory(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        protected override IEnumerable<IGenerationStrategy<ClassModel>> Strategies => StrategyFactories.Values.Select(x => x(_frameworkSet)).ToList();

        public static bool Supports(TypeInfo typeInfo)
        {
            if (typeInfo.Type == null)
            {
                return false;
            }

            return StrategyFactories.ContainsKey(typeInfo.Type.ToFullName());
        }
    }
}