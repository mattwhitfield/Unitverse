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
        }

        public IEnumerable<IParameter> Parameters => _source.Parameters.Select(x => new ParameterFilterModel(x, _semanticModel));

        public IEnumerable<IAttribute> Attributes => _source.Node.GetAttributeModels(_semanticModel);
    }
}
