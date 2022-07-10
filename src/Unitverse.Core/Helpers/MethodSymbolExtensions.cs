namespace Unitverse.Core.Helpers
{
    using Microsoft.CodeAnalysis;

    internal static class MethodSymbolExtensions
    {
        public static bool IsAsyncCallable(this IMethodSymbol methodSymbol)
        {
            return methodSymbol.ReturnType is INamedTypeSymbol namedType && namedType.Name == "Task" && namedType.ContainingNamespace.ToDisplayString() == "System.Threading.Tasks";
        }
    }
}
