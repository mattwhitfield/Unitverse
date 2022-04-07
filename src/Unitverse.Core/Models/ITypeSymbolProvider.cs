namespace Unitverse.Core.Models
{
    using Microsoft.CodeAnalysis;

    public interface ITypeSymbolProvider
    {
        string ClassName { get; }

        INamedTypeSymbol TypeSymbol { get; }
    }
}