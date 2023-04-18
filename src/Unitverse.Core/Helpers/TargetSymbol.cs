using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Unitverse.Core.Helpers
{
    public class TargetSymbol
    {
        public TargetSymbol(SyntaxNode node, ISymbol symbol, TypeInfo type)
        {
            Node = node;
            Symbol = symbol;
            Type = type;
        }

        public SyntaxNode Node { get; }

        public ISymbol Symbol { get; }

        public TypeInfo Type { get; }
    }
}
