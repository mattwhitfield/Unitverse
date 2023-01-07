namespace Unitverse.Core.Templating.Model.Implementation
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Unitverse.Core.Models;

    public class ParameterFilterModel : IParameter
    {
        private readonly ParameterModel _source;
        private readonly SemanticModel _semanticModel;

        public ParameterFilterModel(ParameterModel source, SemanticModel semanticModel)
        {
            _source = source;
            _semanticModel = semanticModel;
        }

        public string Name => _source.Name;

        public IType? Type => _source.Node.Type.GetTypeModel(_semanticModel);

        public IEnumerable<IAttribute> Attributes => _source.Node.GetAttributeModels(_semanticModel);
    }
}
