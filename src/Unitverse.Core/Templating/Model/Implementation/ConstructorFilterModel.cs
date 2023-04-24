namespace Unitverse.Core.Templating.Model.Implementation
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

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

        public bool ShouldGenerate => _source.ShouldGenerate;

        public const string Target = "Constructor";

        public string TemplateType => Target;

        string INameProvider.Name => "ctor(" + string.Join(",", Parameters.Select(x => x.Type?.Name ?? string.Empty)) + ")";

        public NamingContext CreateNamingContext(ITemplatingContext templatingContext)
        {
            return templatingContext.ModelGenerationContext.BaseNamingContext.WithMemberName(_source.Name);
        }
    }
}
