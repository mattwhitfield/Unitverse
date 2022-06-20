namespace Unitverse.Core.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class IndexerModel : TestableModel<IndexerDeclarationSyntax>, IIndexerModel
    {
        public IndexerModel(string name, List<ParameterModel> parameters, TypeInfo typeInfo, IndexerDeclarationSyntax node)
            : base(name, node)
        {
            TypeInfo = typeInfo;
            Parameters = parameters ?? new List<ParameterModel>();
        }

        public bool HasGet => Node.AccessorList?.Accessors.Any(x => x.IsKind(SyntaxKind.GetAccessorDeclaration) && !x.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword))) ?? false;

        public bool HasSet => Node.AccessorList?.Accessors.Any(x => x.IsKind(SyntaxKind.SetAccessorDeclaration) && !x.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword))) ?? false;

        public IList<ParameterModel> Parameters { get; }

        public TypeInfo TypeInfo { get; }
    }
}