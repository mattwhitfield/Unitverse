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
        public PropertyModel(string name, PropertyDeclarationSyntax node, TypeInfo typeInfo, SemanticModel model, IPropertySymbol propertySymbol)
            : base(name, node)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            TypeInfo = typeInfo;

            Symbol = propertySymbol ?? ModelExtensions.GetDeclaredSymbol(model, node) as IPropertySymbol;

            _hasGet = new Lazy<bool>(() => (Node.AccessorList?.Accessors != null && Node.AccessorList.Accessors.Any(x => x.IsKind(SyntaxKind.GetAccessorDeclaration) && !x.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword)))) || Node.ExpressionBody?.Expression != null);
            _hasSet = new Lazy<bool>(() => Node.AccessorList?.Accessors != null && Node.AccessorList.Accessors.Any(x => x.IsKind(SyntaxKind.SetAccessorDeclaration) && !x.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword))));
            _hasInit = new Lazy<bool>(() => Node.AccessorList?.Accessors != null && Node.AccessorList.Accessors.Any(x => x.IsKind(SyntaxKind.InitAccessorDeclaration) && !x.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword))));
        }

        public TypeInfo TypeInfo { get; }

        public IPropertySymbol Symbol { get; }

        private Lazy<bool> _hasGet;
        private Lazy<bool> _hasSet;
        private Lazy<bool> _hasInit;

        public bool HasGet => _hasGet.Value;

        public bool HasSet => _hasSet.Value;

        public bool HasInit => _hasInit.Value;

        public bool IsStatic => Node.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword));

        public ExpressionSyntax Access(ExpressionSyntax target)
        {
            if (Name == OriginalName)
            {
                return Generate.MemberAccess(target, Node.Identifier.Text);
            }

            return Generate.MemberAccess(target, Name);
        }
    }
}
