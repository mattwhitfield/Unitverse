namespace Unitverse.Core.Helpers
{
    using Microsoft.CodeAnalysis;

    public interface ISemanticModelLoader
    {
        public SemanticModel GetSemanticModel(SyntaxNode node);
    }
}
