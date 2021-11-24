namespace Unitverse.Core.Models
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    public interface IInterfaceModel
    {
        string InterfaceName { get; }

        bool IsGeneric { get; }

        IList<ITypeSymbol> GenericTypes { get; }

        INamedTypeSymbol InterfaceType { get; }
    }
}