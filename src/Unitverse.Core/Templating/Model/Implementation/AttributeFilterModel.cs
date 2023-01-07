namespace Unitverse.Core.Templating.Model.Implementation
{
    using Microsoft.CodeAnalysis;

    public class AttributeFilterModel : IAttribute
    {
        private INamedTypeSymbol _typeSymbol;

        public AttributeFilterModel(INamedTypeSymbol typeSymbol)
        {
            _typeSymbol = typeSymbol;
        }

        public IType Type => new TypeFilterModel(_typeSymbol);
    }
}
