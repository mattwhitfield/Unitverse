namespace Unitverse.Core.Helpers
{
    using Microsoft.CodeAnalysis;

    public static class SemanticModelExtensions
    {
        public static INamedTypeSymbol? GetNamedTypeSymbol(this SemanticModel model, SyntaxNode node)
        {
            var symbol = model.GetSymbolInfo(node).Symbol ?? model.GetDeclaredSymbol(node);

            if (symbol == null)
            {
                return null;
            }

            if (symbol is INamedTypeSymbol namedTypeSymbol)
            {
                return namedTypeSymbol;
            }

            return symbol.ContainingType;
        }
    }
}
