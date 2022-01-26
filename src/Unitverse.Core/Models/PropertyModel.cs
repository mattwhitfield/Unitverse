namespace Unitverse.Core.Models
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;

    public class PropertyModel : TestableModel<PropertyDeclarationSyntax>, IPropertyModel
    {
        public PropertyModel(string name, PropertyDeclarationSyntax node, TypeInfo typeInfo, SemanticModel model)
            : base(name, node)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            TypeInfo = typeInfo;

            Symbol = ModelExtensions.GetDeclaredSymbol(model, node) as IPropertySymbol;
        }

        public TypeInfo TypeInfo { get; }

        public IPropertySymbol Symbol { get; }

        public bool HasGet => (Node.AccessorList?.Accessors != null && Node.AccessorList.Accessors.Any(x => x.IsKind(SyntaxKind.GetAccessorDeclaration) && !x.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword)))) || Node.ExpressionBody?.Expression != null;

        public bool HasSet => Node.AccessorList?.Accessors != null && Node.AccessorList.Accessors.Any(x => x.IsKind(SyntaxKind.SetAccessorDeclaration) && !x.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword)));

        public bool IsStatic => Node.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword));

        public ExpressionSyntax Access(ExpressionSyntax target)
        {
            if (Name == OriginalName)
            {
                return Generate.PropertyAccess(target, Node.Identifier.Text);
            }

            return Generate.PropertyAccess(target, Name);
        }
    }
}
