namespace SentryOne.UnitTestGenerator.Core.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using SentryOne.UnitTestGenerator.Core.Helpers;

    public class InterfaceModel : IInterfaceModel
    {
        public InterfaceModel(INamedTypeSymbol interfaceType)
        {
            InterfaceType = interfaceType ?? throw new ArgumentNullException(nameof(interfaceType));
            InterfaceName = interfaceType.ToFullName();
            GenericTypes = new List<ITypeSymbol>(interfaceType.TypeArguments);
            IsGeneric = GenericTypes.Count > 0;
        }

        public string InterfaceName { get; }

        public bool IsGeneric { get; }

        public IList<ITypeSymbol> GenericTypes { get; }

        public INamedTypeSymbol InterfaceType { get; }
    }
}