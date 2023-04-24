namespace Unitverse.Core.Templating.Model.Implementation
{
    using Microsoft.CodeAnalysis;
    using System;

    public class AttributeFilterModel : IAttribute
    {
        private INamedTypeSymbol _typeSymbol;

        public AttributeFilterModel(INamedTypeSymbol typeSymbol)
        {
            _typeSymbol = typeSymbol;
        }

        public IType Type => new TypeFilterModel(_typeSymbol);

        string INameProvider.Name
        {
            get
            {
                if (Type.Name.EndsWith("Attribute", StringComparison.OrdinalIgnoreCase) && Type.Name.Length > 0)
                {
                    return Type.Name.Substring(0, Type.Name.Length - 9);
                }

                return Type.Name;
            }
        }
    }
}
