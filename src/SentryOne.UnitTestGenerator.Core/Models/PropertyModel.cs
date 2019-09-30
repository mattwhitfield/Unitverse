namespace SentryOne.UnitTestGenerator.Core.Models
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class PropertyModel : TestableModel<PropertyDeclarationSyntax>, IPropertyModel
    {
        public PropertyModel(string name, PropertyDeclarationSyntax node, TypeInfo typeInfo)
            : base(name, node)
        {
            TypeInfo = typeInfo;
        }

        public TypeInfo TypeInfo { get; }

        public bool HasGet => (Node.AccessorList?.Accessors != null && Node.AccessorList.Accessors.Any(x => x.IsKind(SyntaxKind.GetAccessorDeclaration) && !x.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword)))) || Node.ExpressionBody?.Expression != null;

        public bool HasSet => Node.AccessorList?.Accessors != null && Node.AccessorList.Accessors.Any(x => x.IsKind(SyntaxKind.SetAccessorDeclaration) && !x.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword)));

        public bool IsStatic => Node.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword));
    }
}
