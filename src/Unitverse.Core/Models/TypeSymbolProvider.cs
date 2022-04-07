namespace Unitverse.Core.Models
{
    using Microsoft.CodeAnalysis;

    public class TypeSymbolProvider : ITypeSymbolProvider
    {
        public TypeSymbolProvider(INamedTypeSymbol typeSymbol)
        {
            TypeSymbol = typeSymbol ?? throw new System.ArgumentNullException(nameof(typeSymbol));
        }

        public string ClassName => TypeSymbol.Name;

        public INamedTypeSymbol TypeSymbol { get; }
    }
}