namespace SentryOne.UnitTestGenerator.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Resources;

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
            IsVoid = string.Equals(node.ReturnType.ToFullString().Trim(), Strings.Generate_Method__void, StringComparison.OrdinalIgnoreCase);

            if (ModelExtensions.GetDeclaredSymbol(model, node) is IMethodSymbol methodSymbol)
            {
                IsAsync = methodSymbol.IsAwaitableNonDynamic();
            }
        }

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
                frameworkSet.Context.AddEmittedType(typeSymbol);
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