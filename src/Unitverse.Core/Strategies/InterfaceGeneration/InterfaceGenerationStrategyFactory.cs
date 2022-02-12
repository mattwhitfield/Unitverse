namespace Unitverse.Core.Strategies.InterfaceGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;

    public class InterfaceGenerationStrategyFactory : ItemGenerationStrategyFactory<ClassModel>
    {
        private static readonly Dictionary<string, Func<IFrameworkSet, IGenerationStrategy<ClassModel>>> StrategyFactories = new Dictionary<string, Func<IFrameworkSet, IGenerationStrategy<ClassModel>>>
        {
            { EquatableGenerationStrategy.InterfaceNameForMatch, frameworkSet => new EquatableGenerationStrategy(frameworkSet) },
            { ComparableGenerationStrategy.InterfaceNameForMatch, frameworkSet => new ComparableGenerationStrategy(frameworkSet) },
            { EnumerableGenerationStrategy.InterfaceNameForMatch, frameworkSet => new EnumerableGenerationStrategy(frameworkSet) },
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