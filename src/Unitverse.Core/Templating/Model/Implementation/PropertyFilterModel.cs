namespace Unitverse.Core.Templating.Model.Implementation
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Unitverse.Core.Models;

    public class PropertyFilterModel : IProperty
    {
        private readonly IPropertyModel _source;
        private readonly SemanticModel _semanticModel;

        public PropertyFilterModel(IPropertyModel source, SemanticModel semanticModel)
        {
            _source = source;
            _semanticModel = semanticModel;
            Attributes = new LazyEnumerable<IAttribute>(() => _source.Node.GetAttributeModels(_semanticModel));
        }

        public string Name => _source.Name;

        public bool HasGet => _source.HasGet;

        public bool HasSet => _source.HasSet;

        public bool HasInit => _source.HasInit;

        public bool IsStatic => _source.IsStatic;

        public IType? Type => _source.Node.Type.GetTypeModel(_semanticModel);

        public IEnumerable<IAttribute> Attributes { get; }

        public string TemplateType => "Property";
    }
}
