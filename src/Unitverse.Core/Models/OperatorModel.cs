namespace Unitverse.Core.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;

    public class OperatorModel : TestableModel<OperatorDeclarationSyntax>, IOperatorModel
    {
        private static readonly Dictionary<string, SyntaxKind> BinaryExpressionKinds = new Dictionary<string, SyntaxKind>(StringComparer.OrdinalIgnoreCase)
        {
            { "+", SyntaxKind.AddExpression },
            { "-", SyntaxKind.SubtractExpression },
            { "*", SyntaxKind.MultiplyExpression },
            { "/", SyntaxKind.DivideExpression },
            { "%", SyntaxKind.ModuloExpression },
            { "&", SyntaxKind.BitwiseAndExpression },
            { "|", SyntaxKind.BitwiseOrExpression },
            { "^", SyntaxKind.ExclusiveOrExpression },
            { "<<", SyntaxKind.LeftShiftExpression },
            { ">>", SyntaxKind.RightShiftExpression },
            { "==", SyntaxKind.EqualsExpression },
            { "!=", SyntaxKind.NotEqualsExpression },
            { "<", SyntaxKind.LessThanExpression },
            { ">", SyntaxKind.GreaterThanExpression },
            { "<=", SyntaxKind.LessThanOrEqualExpression },
            { ">=", SyntaxKind.GreaterThanOrEqualExpression },
        };

        private static readonly Dictionary<string, string> OperatorNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "+", "Plus" },
            { "++", "Increment" },
            { "--", "Decrement" },
            { "-", "Minus" },
            { "*", "Multiplication" },
            { "/", "Division" },
            { "!", "Negation" },
            { "~", "BitwiseComplement" },
            { "%", "Modulo" },
            { "&", "And" },
            { "|", "Or" },
            { "^", "ExclusiveOr" },
            { "<<", "LeftShift" },
            { ">>", "RightShift" },
            { "==", "Equality" },
            { "!=", "Inequality" },
            { "<", "LessThan" },
            { ">", "GreaterThan" },
            { "<=", "LessThanEqualTo" },
            { ">=", "GreaterThanEqualTo" },
        };

        private static readonly Dictionary<string, SyntaxKind> UnaryExpressionKinds = new Dictionary<string, SyntaxKind>(StringComparer.OrdinalIgnoreCase)
        {
            { "+", SyntaxKind.UnaryPlusExpression },
            { "++", SyntaxKind.PreIncrementExpression },
            { "--", SyntaxKind.PreDecrementExpression },
            { "-", SyntaxKind.UnaryMinusExpression },
            { "!", SyntaxKind.LogicalNotExpression },
            { "~", SyntaxKind.BitwiseNotExpression },
        };

        public OperatorModel(string name, List<ParameterModel> parameters, OperatorDeclarationSyntax node, SemanticModel model)
            : base(GetName(name, parameters), node)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            Parameters = parameters ?? new List<ParameterModel>();
            OperatorText = name;
        }

        public string OperatorText { get; }

        public IList<ParameterModel> Parameters { get; }

        public ExpressionSyntax? Invoke(ClassModel owner, bool suppressAwait, IFrameworkSet frameworkSet, params CSharpSyntaxNode[] arguments)
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

            if (arguments.Length == 1 && arguments[0] is ExpressionSyntax expression && UnaryExpressionKinds.TryGetValue(OperatorText, out var kind))
            {
                return SyntaxFactory.PrefixUnaryExpression(kind, expression);
            }

            if (arguments.Length == 2 && arguments[0] is ExpressionSyntax left && arguments[1] is ExpressionSyntax right && BinaryExpressionKinds.TryGetValue(OperatorText, out kind))
            {
                return SyntaxFactory.BinaryExpression(kind, left, right);
            }

            return null;
        }

        private static string GetName(string name, List<ParameterModel> parameters)
        {
            var prefix = parameters != null && parameters.Count == 1 ? "Unary" : string.Empty;

            if (OperatorNames.TryGetValue(name, out var resolvedName))
            {
                return prefix + resolvedName;
            }

            return "Unknown";
        }
    }
}