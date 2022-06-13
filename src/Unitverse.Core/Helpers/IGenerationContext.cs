namespace Unitverse.Core.Helpers
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Unitverse.Core.Models;

    public interface IGenerationContext : IGenerationStatistics
    {
        IEnumerable<ITypeSymbol> EmittedTypes { get; }

        IEnumerable<string> GenericTypesVisited { get; }

        IDictionary<string, ITypeSymbol> GenericTypes { get; }

        bool MocksUsed { get; set; }

        void AddEmittedType(ITypeSymbol typeInfo);

        void AddVisitedGenericType(string identifier);

        ModelGenerationContext CurrentModel { get; set; }

        SectionedMethodHandler CurrentMethod { get; set; }
    }
}