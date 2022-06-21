namespace Unitverse.Core.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class ConstructorModel : TestableModel<ConstructorDeclarationSyntax>, IConstructorModel
    {
        public ConstructorModel(string name, IList<ParameterModel> parameters, ConstructorDeclarationSyntax node)
            : base(name, node)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Parameters = parameters ?? new List<ParameterModel>();
        }

        public IList<ParameterModel> Parameters { get; }

        public bool IsFromRelatedPartial { get; set; }

        public override void SetShouldGenerateForSingleItem(SyntaxNode syntaxNode)
        {
            ShouldGenerate = (syntaxNode is ConstructorDeclarationSyntax && !IsFromRelatedPartial) || syntaxNode == Node.Parent;
        }
    }
}