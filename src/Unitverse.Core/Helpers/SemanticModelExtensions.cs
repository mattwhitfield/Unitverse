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

        public static SymbolInfo? GetSymbolInfoSafe(this SemanticModel semanticModel, SyntaxNode node)
        {
            if (semanticModel.SyntaxTree == node.SyntaxTree)
            {
                return semanticModel.GetSymbolInfo(node);
            }

            var loader = SemanticModelLoaderProvider.ModelLoader;
            if (loader != null)
            {
                var model = loader.GetSemanticModel(node);
                return model.GetSymbolInfo(node);
            }

            return null;
        }

        public static ISymbol? GetDeclaredSymbolSafe(this SemanticModel semanticModel, SyntaxNode node)
        {
            if (semanticModel.SyntaxTree == node.SyntaxTree)
            {
                return semanticModel.GetDeclaredSymbol(node);
            }

            var loader = SemanticModelLoaderProvider.ModelLoader;
            if (loader != null)
            {
                var model = loader.GetSemanticModel(node);
                return model.GetDeclaredSymbol(node);
            }

            return null;
        }
    }
}
