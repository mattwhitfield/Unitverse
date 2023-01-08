namespace Unitverse.Core.Templating.Model.Implementation
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Unitverse.Core.Models;

    public class ConstructorFilterModel : IConstructor
    {
        private readonly IConstructorModel _source;
        private readonly SemanticModel _semanticModel;

        public ConstructorFilterModel(IConstructorModel source, SemanticModel semanticModel)
        {
            _source = source;
            _semanticModel = semanticModel;
            Parameters = new LazyEnumerable<IParameter>(() => _source.Parameters.Select(x => new ParameterFilterModel(x, _semanticModel)));
            Attributes = new LazyEnumerable<IAttribute>(() => _source.Node.GetAttributeModels(_semanticModel));
        }

        public IEnumerable<IParameter> Parameters { get; }

        public IEnumerable<IAttribute> Attributes { get; }
}
}
