namespace Unitverse.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;

    public class MethodModel : TestableModel<MethodDeclarationSyntax>, IMethodModel
    {
        public MethodModel(string name, List<ParameterModel> parameters, MethodDeclarationSyntax node, SemanticModel model)
            : base(name, node)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            Parameters = parameters ?? new List<ParameterModel>();
            IsVoid = string.Equals(node.ReturnType.ToFullString().Trim(), "void", StringComparison.OrdinalIgnoreCase);

            Symbol = ModelExtensions.GetDeclaredSymbol(model, node) as IMethodSymbol;
            if (Symbol != null)
            {
                IsAsync = Symbol.IsAwaitableNonDynamic();
            }
        }

        public IMethodSymbol? Symbol { get; }

        public bool IsAsync { get; }

        public bool IsVoid { get; }

        public IList<ParameterModel> Parameters { get; }

        public ExpressionSyntax Invoke(ClassModel owner, bool suppressAwait, IFrameworkSet frameworkSet, params CSharpSyntaxNode[] arguments)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (frameworkSet == null)
            {
                throw new ArgumentNullException(nameof(frameworkSet));
            }

            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            var ownerTargetInstance = Node.Modifiers.Any(x => x.IsKind(SyntaxKind.StaticKeyword)) ? owner.TypeSyntax : owner.TargetInstance;

            if (Node.ExplicitInterfaceSpecifier != null)
            {
                ownerTargetInstance = SyntaxFactory.ParenthesizedExpression(SyntaxFactory.CastExpression(Node.ExplicitInterfaceSpecifier.Name, ownerTargetInstance));
                var typeSymbol = owner.SemanticModel.GetTypeInfo(Node.ExplicitInterfaceSpecifier.Name).Type;
                if (typeSymbol != null)
                {
                    frameworkSet.Context.AddEmittedType(typeSymbol);
                }
            }

            var name = Name == OriginalName ? Node.Identifier.Text : Name;
            var expressionSyntax = Generate.MethodCall(ownerTargetInstance, Node, name, frameworkSet, arguments);
            if (IsAsync && !suppressAwait)
            {
                return SyntaxFactory.AwaitExpression(expressionSyntax);
            }

            return expressionSyntax;
        }
    }
}