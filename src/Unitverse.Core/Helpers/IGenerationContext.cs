namespace Unitverse.Core.Helpers
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    public interface IGenerationContext
    {
        IEnumerable<ITypeSymbol> EmittedTypes { get; }

        IEnumerable<string> GenericTypes { get; }

        bool MocksUsed { get; set; }

        void AddEmittedType(ITypeSymbol typeInfo);

        void AddGenericType(string identifier);
    }
}