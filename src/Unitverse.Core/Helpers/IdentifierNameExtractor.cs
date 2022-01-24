namespace Unitverse.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class IdentifierNameExtractor : CSharpSyntaxWalker
    {
        private IdentifierNameExtractor()
        {
        }

        public static IEnumerable<string> ExtractFrom(CSharpSyntaxNode node)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var extractor = new IdentifierNameExtractor();
            node.Accept(extractor);
            return extractor._identifiers;
        }

        private readonly List<string> _identifiers = new List<string>();

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            base.VisitIdentifierName(node);

            _identifiers.Add(node.Identifier.Text);
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
        }
    }
}
