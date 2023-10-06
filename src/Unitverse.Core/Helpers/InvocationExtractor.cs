namespace Unitverse.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Models;

    public class InvocationExtractor : CSharpSyntaxWalker
    {
        private InvocationExtractor(INamedTypeSymbol containingTypeSymbol, SemanticModel semanticModel, IEnumerable<string> targetFields)
            : this(containingTypeSymbol, semanticModel, targetFields, new HashSet<MethodDeclarationSyntax>())
        {
        }

        private InvocationExtractor(INamedTypeSymbol containingTypeSymbol, SemanticModel semanticModel, IEnumerable<string> targetFields, ISet<MethodDeclarationSyntax> visitedMethods)
        {
            _semanticModel = semanticModel;
            _targetFields = new HashSet<string>(targetFields);
            _visitedMethods = visitedMethods;
            _containingTypeSymbol = containingTypeSymbol;
        }

        public static DependencyAccessMap ExtractFrom(ClassModel model, CSharpSyntaxNode node, IEnumerable<string> targetFields)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (targetFields is null)
            {
                throw new ArgumentNullException(nameof(targetFields));
            }

            var extractor = new InvocationExtractor(model.TypeSymbol, model.SemanticModel, targetFields);
            node.Accept(extractor);

            return new DependencyAccessMap(extractor._methodCalls, extractor._propertyCalls, extractor._invocationCount, extractor._memberAccessCount);
        }

        private readonly List<Tuple<IMethodSymbol, string>> _methodCalls = new List<Tuple<IMethodSymbol, string>>();
        private readonly List<Tuple<IPropertySymbol, string>> _propertyCalls = new List<Tuple<IPropertySymbol, string>>();
        private readonly SemanticModel _semanticModel;
        private readonly HashSet<string> _targetFields;
        private readonly ISet<MethodDeclarationSyntax> _visitedMethods;
        private readonly INamedTypeSymbol _containingTypeSymbol;
        private int _invocationCount;
        private int _memberAccessCount;

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            base.VisitMemberAccessExpression(node);

            if (node.Parent is InvocationExpressionSyntax)
            {
                return;
            }

            _memberAccessCount++;
            if (GetFieldTarget(node.Expression, out var fieldTarget))
            {
                if (_targetFields.Contains(fieldTarget))
                {
                    var symbolInfo = _semanticModel.GetSymbolInfoSafe(node);
                    if (symbolInfo?.Symbol is IPropertySymbol propertySymbol)
                    {
                        if (!(node.Parent is AssignmentExpressionSyntax assignment) || node != assignment.Left)
                        {
                            _propertyCalls.Add(Tuple.Create(propertySymbol, fieldTarget));
                        }
                    }
                }
            }
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            base.VisitInvocationExpression(node);
            _invocationCount++;

            if (node.Expression is MemberAccessExpressionSyntax memberAccessExpression)
            {
                if (GetFieldTarget(memberAccessExpression.Expression, out var fieldTarget))
                {
                    if (_targetFields.Contains(fieldTarget))
                    {
                        var symbolInfo = _semanticModel.GetSymbolInfoSafe(node);
                        if (symbolInfo?.Symbol is IMethodSymbol methodSymbol)
                        {
                            // if ReducedFrom is non-null, this is actually an extension method call
                            if (methodSymbol.ReducedFrom is null)
                            {
                                _methodCalls.Add(Tuple.Create(methodSymbol, fieldTarget));
                            }
                        }
                    }
                }
                else
                {
                    Descend(node.Expression);
                }
            }
            else if (node.Expression is IdentifierNameSyntax)
            {
                Descend(node);
            }
        }

        private void Descend(ExpressionSyntax node)
        {
            var methodDeclaration = GetInvokedMethodDeclaration(node);
            if (methodDeclaration is null || _visitedMethods.Contains(methodDeclaration))
            {
                return;
            }

            _visitedMethods.Add(methodDeclaration);
            var extractor = new InvocationExtractor(_containingTypeSymbol, _semanticModel, _targetFields, _visitedMethods);
            methodDeclaration?.Accept(extractor);
            _methodCalls.AddRange(extractor._methodCalls);
        }

        private bool GetFieldTarget(ExpressionSyntax expressionSyntax, out string fieldTarget)
        {
            if (expressionSyntax is IdentifierNameSyntax identifierName)
            {
                fieldTarget = identifierName.Identifier.Text;
                return true;
            }
            else if (expressionSyntax is MemberAccessExpressionSyntax memberAccess && (memberAccess.Expression is ThisExpressionSyntax || memberAccess.Expression is BaseExpressionSyntax))
            {
                fieldTarget = memberAccess.Name.ToString();
                return true;
            }
            else
            {
                fieldTarget = string.Empty;
                return false;
            }
        }

        private MethodDeclarationSyntax? GetInvokedMethodDeclaration(ExpressionSyntax invocationExpression)
        {
            var symbol = _semanticModel.GetSymbolInfoSafe(invocationExpression)?.Symbol;

            if (symbol == null
                || symbol.Kind != SymbolKind.Method
                || symbol.ContainingType != _containingTypeSymbol)
            {
                return null;
            }

            return symbol.DeclaringSyntaxReferences[0].GetSyntax() as MethodDeclarationSyntax;
        }
    }
}
