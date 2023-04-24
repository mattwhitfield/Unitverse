namespace Unitverse.Core.Templating.Model.Implementation
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;

    public class OwningTypeFilterModel : IOwningType
    {
        private readonly ClassModel _source;

        public OwningTypeFilterModel(ClassModel source)
        {
            _source = source;

            BaseTypes = new LazyEnumerable<IType>(() => _source.TypeSymbol.GetBaseTypes().Select(x => new TypeFilterModel(x)));

            Interfaces = new LazyEnumerable<IType>(() => _source.TypeSymbol.Interfaces.Select(x => new TypeFilterModel(x)));

            AllInterfaces = new LazyEnumerable<IType>(() => _source.TypeSymbol.AllInterfaces.Select(x => new TypeFilterModel(x)));

            Attributes = new LazyEnumerable<IAttribute>(() => _source.Declaration.GetAttributeModels(_source.SemanticModel));

            Methods = new LazyEnumerable<IMethod>(() => _source.Methods.Select(x => new MethodFilterModel(x, _source.SemanticModel)));

            Constructors = new LazyEnumerable<IConstructor>(() => _source.Constructors.Select(x => new ConstructorFilterModel(x, _source.SemanticModel)));

            Properties = new LazyEnumerable<IProperty>(() => _source.Properties.Select(x => new PropertyFilterModel(x, _source.SemanticModel)));
        }

        public IType Type => new TypeFilterModel(_source.TypeSymbol);

        public bool IsStatic => _source.IsStatic;

        public bool IsGeneric => _source.TypeSymbol.IsGenericType;

        public IEnumerable<IType> BaseTypes { get; }

        public IEnumerable<IType> Interfaces { get; }

        public IEnumerable<IType> AllInterfaces { get; }

        public IEnumerable<IAttribute> Attributes { get; }

        public IEnumerable<IMethod> Methods { get; }

        public IEnumerable<IConstructor> Constructors { get; }

        public IEnumerable<IProperty> Properties { get; }

        string INameProvider.Name => "owningType";
    }
}
