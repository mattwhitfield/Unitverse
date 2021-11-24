namespace Unitverse.Core.Models
{
    using Microsoft.CodeAnalysis;

    public interface ITestableModel<out T>
        where T : SyntaxNode
    {
        string Name { get; }

        string OriginalName { get; }

        T Node { get; }

        void MutateName(string newName);

        bool ShouldGenerate { get; set; }

        void SetShouldGenerateForSingleItem(SyntaxNode syntaxNode);
    }
}
