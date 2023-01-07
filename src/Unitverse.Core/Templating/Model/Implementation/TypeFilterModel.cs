namespace Unitverse.Core.Templating.Model.Implementation
{
    using Microsoft.CodeAnalysis;
    using Unitverse.Core.Helpers;

    public class TypeFilterModel : IType
    {
        private INamedTypeSymbol _typeSymbol;
        private string _genericSuffix;

        public TypeFilterModel(INamedTypeSymbol typeSymbol)
        {
            _typeSymbol = typeSymbol;

            if (_typeSymbol.TypeParameters.Length > 1)
            {
                _genericSuffix = "<" + new string(',', _typeSymbol.TypeParameters.Length - 1) + ">";
            }
            else if (_typeSymbol.TypeParameters.Length > 0)
            {
                _genericSuffix = "<>";
            }
            else
            {
                _genericSuffix = string.Empty;
            }
        }

        public string Name => _typeSymbol.Name + _genericSuffix;

        public string FullName => _typeSymbol.ToFullName() + _genericSuffix;

        public string Namespace => _typeSymbol.ContainingNamespace.ToFullName();
    }
}
