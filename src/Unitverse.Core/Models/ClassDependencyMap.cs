namespace Unitverse.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    public class ClassDependencyMap
    {
        private readonly Dictionary<string, HashSet<ParameterModel>> _setFields;
        private readonly Dictionary<string, ITypeSymbol> _fieldTypes;

        public IEnumerable<string> MappedFields => _setFields.Keys;

        public IEnumerable<string> MappedInterfaceFields
        {
            get
            {
                foreach (var field in _setFields.Keys)
                {
                    if (_fieldTypes.TryGetValue(field, out ITypeSymbol type))
                    {
                        if (type.TypeKind == TypeKind.Interface)
                        {
                            yield return field;
                        }
                    }
                }
            }
        }

        public ClassDependencyMap(Dictionary<string, HashSet<ParameterModel>> setFields, Dictionary<string, ITypeSymbol> fieldTypes)
        {
            _setFields = setFields ?? throw new ArgumentNullException(nameof(setFields));
            _fieldTypes = fieldTypes ?? throw new ArgumentNullException(nameof(fieldTypes));
        }

        public ITypeSymbol GetTypeSymbolFor(string fieldName)
        {
            if (_fieldTypes.TryGetValue(fieldName, out var typeSymbol))
            {
                return typeSymbol;
            }

            return null;
        }

        public IEnumerable<ParameterModel> GetConstructorParametersFor(string fieldName)
        {
            if (_setFields.TryGetValue(fieldName, out var constructorParameters))
            {
                return constructorParameters;
            }

            return Enumerable.Empty<ParameterModel>();
        }
    }
}
