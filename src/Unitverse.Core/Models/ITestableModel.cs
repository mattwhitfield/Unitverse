namespace Unitverse.Core.Models
{
    using Microsoft.CodeAnalysis;

    public interface ITestableModel
    {
        string Name { get; }

        string OriginalName { get; }

        void MutateName(string newName);

        bool ShouldGenerate { get; set; }

        bool MarkedForGeneration { get; set; }

        void SetShouldGenerateForSingleItem(SyntaxNode syntaxNode);
    }

    public interface ITestableModel<out T> : ITestableModel
        where T : SyntaxNode
    {
        T Node { get; }
    }
}
