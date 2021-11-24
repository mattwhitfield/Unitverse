namespace Unitverse.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    public class GenerationContext : IGenerationContext
    {
        private readonly List<ITypeSymbol> _emittedTypes = new List<ITypeSymbol>();
        private readonly HashSet<string> _emittedTypeFullNames = new HashSet<string>();
        private readonly HashSet<string> _genericTypes = new HashSet<string>();

        public IEnumerable<ITypeSymbol> EmittedTypes => _emittedTypes;

        public IEnumerable<string> GenericTypes => _genericTypes;

        public bool MocksUsed { get; set; }

        public void AddEmittedType(ITypeSymbol typeInfo)
        {
            if (typeInfo == null)
            {
                throw new ArgumentNullException(nameof(typeInfo));
            }

            var fullName = typeInfo.ToDisplayString(new SymbolDisplayFormat(
                SymbolDisplayGlobalNamespaceStyle.Omitted,
                SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                SymbolDisplayGenericsOptions.IncludeTypeParameters,
                miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes));

            if (_emittedTypeFullNames.Add(fullName))
            {
                _emittedTypes.Add(typeInfo);
            }
        }

        public void AddGenericType(string identifier)
        {
            _genericTypes.Add(identifier);
        }
    }
}
