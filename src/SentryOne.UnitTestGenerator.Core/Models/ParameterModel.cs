namespace SentryOne.UnitTestGenerator.Core.Models
{
    using System;
    using Microsoft.CodeAnalysis;
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
        }

        public string Type { get; }

        public TypeInfo TypeInfo { get; }
    }
}