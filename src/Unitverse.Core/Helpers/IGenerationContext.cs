namespace Unitverse.Core.Helpers
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public interface IGenerationContext : IGenerationStatistics
    {
        IEnumerable<ITypeSymbol> EmittedTypes { get; }

        IGenerationOptions Options { get; }

        INamingProvider NamingProvider { get; }

        IEnumerable<string> GenericTypesVisited { get; }

        IDictionary<string, ITypeSymbol?> GenericTypes { get; }

        bool MocksUsed { get; set; }

        void AddEmittedType(ITypeSymbol typeInfo);

        void AddVisitedGenericType(string identifier);

        ClassModel? CurrentModel { get; set; }

        SectionedMethodHandler CurrentMethod { get; set; }
    }
}