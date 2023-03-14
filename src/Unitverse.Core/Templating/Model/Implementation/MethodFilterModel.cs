namespace Unitverse.Core.Templating.Model.Implementation
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Unitverse.Core.Models;

    public class MethodFilterModel : IMethod
    {
        private readonly IMethodModel _source;
        private readonly SemanticModel _semanticModel;

        public MethodFilterModel(IMethodModel source, SemanticModel semanticModel)
        {
            _source = source;
            _semanticModel = semanticModel;
            Parameters = new LazyEnumerable<IParameter>(() => _source.Parameters.Select(x => new ParameterFilterModel(x, _semanticModel)));
            Attributes = new LazyEnumerable<IAttribute>(() => _source.Node.GetAttributeModels(_semanticModel));
        }

        public string Name => _source.Name;

        public bool IsAsync => _source.IsAsync;

        public bool IsVoid => _source.IsVoid;

        public bool IsStatic => _source.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.StaticKeyword));

        public IType? ReturnType => _source.Node.ReturnType.GetTypeModel(_semanticModel);

        public IEnumerable<IParameter> Parameters { get; }

        public IEnumerable<IAttribute> Attributes { get; }

        public string TemplateType => "Method";
    }
}
