namespace SentryOne.UnitTestGenerator.Core.Models
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Frameworks;

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
    }
}