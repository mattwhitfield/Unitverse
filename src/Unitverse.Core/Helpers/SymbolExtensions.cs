namespace Unitverse.Core.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    public static class SymbolExtensions
    {
        public static IEnumerable<INamedTypeSymbol> GetBaseTypes(this INamedTypeSymbol namedTypeSymbol)
        {
            while (namedTypeSymbol.BaseType != null)
            {
                yield return namedTypeSymbol.BaseType;

                namedTypeSymbol = namedTypeSymbol.BaseType;
            }
        }

        public static bool IsAwaitableNonDynamic(this IMethodSymbol symbol)
        {
            if (symbol == null)
            {
                return false;
            }

            return symbol.ReturnType.GetMembers(WellKnownMemberNames.GetAwaiter).OfType<IMethodSymbol>().Where(x => !x.Parameters.Any()).Any(VerifyGetAwaiter);
        }

        private static bool VerifyGetAwaiter(IMethodSymbol getAwaiter)
        {
            var returnType = getAwaiter.ReturnType;
            if (returnType != null)
            {
                if (returnType.GetMembers().OfType<IPropertySymbol>().Any(p => p.Name == WellKnownMemberNames.IsCompleted && p.Type.SpecialType == SpecialType.System_Boolean && p.GetMethod != null))
                {
                    var methods = returnType.GetMembers().OfType<IMethodSymbol>().ToList();

                    return methods.Any(x => x.Name == WellKnownMemberNames.OnCompleted && x.ReturnsVoid && x.Parameters.Length == 1 && x.Parameters.First().Type.TypeKind == TypeKind.Delegate) && methods.Any(m => m.Name == WellKnownMemberNames.GetResult && !m.Parameters.Any());
                }
            }

            return false;
        }
    }
}
