namespace Unitverse.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Unitverse.Core.Options;

    public class GenerationContext : IGenerationContext
    {
        public GenerationContext(IGenerationOptions options)
        {
            CurrentMethod = new SectionedMethodHandler(SyntaxFactory.MethodDeclaration(SyntaxFactory.IdentifierName("void"), "Dummy"), options);
        }

        private readonly List<ITypeSymbol> _emittedTypes = new List<ITypeSymbol>();
        private readonly HashSet<string> _emittedTypeFullNames = new HashSet<string>();
        private readonly HashSet<string> _visitedGenericTypes = new HashSet<string>();

        public IEnumerable<ITypeSymbol> EmittedTypes => _emittedTypes;

        public bool MocksUsed { get; set; }

        public IDictionary<string, ITypeSymbol?> GenericTypes { get; } = new Dictionary<string, ITypeSymbol?>();

        public IEnumerable<string> GenericTypesVisited => _visitedGenericTypes;

        public long InterfacesMocked { get; set; }

        public long TypesConstructed { get; set; }

        public long ValuesGenerated { get; set; }

        public long TestClassesGenerated { get; set; }

        public long TestMethodsGenerated { get; set; }

        public long TestMethodsRegenerated { get; set; }

        public bool CurrentModelIsStatic { get; set; }

        public SectionedMethodHandler CurrentMethod { get; set; }

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

        public void AddVisitedGenericType(string identifier)
        {
            if (!GenericTypes.ContainsKey(identifier))
            {
                _visitedGenericTypes.Add(identifier);
            }
        }
    }
}
