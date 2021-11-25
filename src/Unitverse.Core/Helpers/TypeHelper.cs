namespace Unitverse.Core.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    public static class TypeHelper
    {
        public static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol @namespace)
        {
            foreach (var type in @namespace.GetTypeMembers())
            {
                yield return type;
            }

            foreach (var nestedNamespace in @namespace.GetNamespaceMembers())
            {
                foreach (var type in GetAllTypes(nestedNamespace))
                {
                    yield return type;
                }
            }
        }

        public static bool IsDerivedFrom(ITypeSymbol baseType, ITypeSymbol derivedType)
        {
            var currentType = derivedType;
            while (currentType != null)
            {
                if (currentType.Equals(baseType))
                {
                    return true;
                }

                if (currentType.Interfaces.Any(i => i.Equals(baseType)))
                {
                    return true;
                }

                currentType = currentType.BaseType;
            }

            return false;
        }

        public static INamedTypeSymbol FindDerivedNonAbstractType(params ITypeSymbol[] baseTypes)
        {
            var nameSpaces = new HashSet<INamespaceSymbol>(baseTypes.Select(x => x.ContainingAssembly.GlobalNamespace));
            var potentialTypes = nameSpaces.SelectMany(x => GetAllTypes(x));

            return potentialTypes.FirstOrDefault(x => !x.IsAbstract && baseTypes.All(baseType => IsDerivedFrom(baseType, x)));
        }
    }
}
