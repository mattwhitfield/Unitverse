namespace Unitverse.Core.Templating.Model.Implementation
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;

    public class ClassFilterModel : IClass
    {
        private readonly ClassModel _source;

        public ClassFilterModel(ClassModel source)
        {
            _source = source;
        }

        public IType Type => new TypeFilterModel(_source.TypeSymbol);

        public IEnumerable<IType> BaseTypes => _source.TypeSymbol.GetBaseTypes().Select(x => new TypeFilterModel(x));

        public IEnumerable<IType> Interfaces => _source.TypeSymbol.Interfaces.Select(x => new TypeFilterModel(x));

        public IEnumerable<IType> AllInterfaces => _source.TypeSymbol.AllInterfaces.Select(x => new TypeFilterModel(x));

        public IEnumerable<IAttribute> Attributes => _source.Declaration.GetAttributeModels(_source.SemanticModel);

        public IEnumerable<IMethod> Methods => _source.Methods.Select(x => new MethodFilterModel(x, _source.SemanticModel));

        public IEnumerable<IConstructor> Constructors => _source.Constructors.Select(x => new ConstructorFilterModel(x, _source.SemanticModel));

        public IEnumerable<IProperty> Properties => _source.Properties.Select(x => new PropertyFilterModel(x, _source.SemanticModel));
    }
}
