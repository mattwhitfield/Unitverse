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
        private InvocationExtractor(SemanticModel semanticModel, IEnumerable<string> targetFields)
        {
            _semanticModel = semanticModel;
            _targetFields = new HashSet<string>(targetFields);
        }

        public static DependencyAccessMap ExtractFrom(CSharpSyntaxNode node, SemanticModel semanticModel, IEnumerable<string> targetFields)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            if (targetFields is null)
            {
                throw new ArgumentNullException(nameof(targetFields));
            }

            var extractor = new InvocationExtractor(semanticModel, targetFields);
            node.Accept(extractor);

            return new DependencyAccessMap(extractor._methodCalls, extractor._propertyCalls, extractor._invocationCount, extractor._memberAccessCount);
        }

        private readonly List<Tuple<IMethodSymbol, string>> _methodCalls = new List<Tuple<IMethodSymbol, string>>();
        private readonly List<Tuple<IPropertySymbol, string>> _propertyCalls = new List<Tuple<IPropertySymbol, string>>();
        private readonly SemanticModel _semanticModel;
        private readonly HashSet<string> _targetFields;
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
            if (node.Expression is IdentifierNameSyntax identifierName)
            {
                if (_targetFields.Contains(identifierName.Identifier.Text))
                {
                    var symbolInfo = _semanticModel.GetSymbolInfo(node);
                    if (symbolInfo.Symbol is IPropertySymbol propertySymbol)
                    {
                        if (!(node.Parent is AssignmentExpressionSyntax assignment) || node != assignment.Left)
                        {
                            _propertyCalls.Add(Tuple.Create(propertySymbol, identifierName.Identifier.Text));
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
                if (memberAccessExpression.Expression is IdentifierNameSyntax identifierName)
                {
                    if (_targetFields.Contains(identifierName.Identifier.Text))
                    {
                        var symbolInfo = _semanticModel.GetSymbolInfo(node);
                        if (symbolInfo.Symbol is IMethodSymbol methodSymbol)
                        {
                            // if ReducedFrom is non-null, this is actually an extension method call
                            if (methodSymbol.ReducedFrom is null)
                            {
                                _methodCalls.Add(Tuple.Create(methodSymbol, identifierName.Identifier.Text));
                            }
                        }
                    }
                }
            }
        }
    }
}
