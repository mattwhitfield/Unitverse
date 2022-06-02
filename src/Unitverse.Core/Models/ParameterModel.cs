﻿namespace Unitverse.Core.Models
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class ParameterModel : TestableModel<ParameterSyntax>
    {
        public ParameterModel(string name, ParameterSyntax node, string type, TypeInfo typeInfo)
            : base(name, node)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentNullException(nameof(type));
            }

            Type = type;

            TypeInfo = typeInfo;

            Identifier = node.Identifier;
        }

        public SyntaxToken Identifier { get; }

        public string Type { get; }

        public TypeInfo TypeInfo { get; }

        public bool IsNullableTypeSyntax => Node.Type is NullableTypeSyntax;

        public bool HasNullDefaultValue
        {
            get
            {
                if (Node.Default == null)
                {
                    return false;
                }

                if (Node.Default.Value is DefaultExpressionSyntax)
                {
                    return true;
                }

                if (Node.Default.Value is LiteralExpressionSyntax literal)
                {
                    return literal.Kind() == SyntaxKind.DefaultLiteralExpression || literal.Kind() == SyntaxKind.NullLiteralExpression;
                }

                return false;
            }
        }
    }
}