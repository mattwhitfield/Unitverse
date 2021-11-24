namespace Unitverse.Core.Strategies.ValueGeneration
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;

    public interface IValueGenerationStrategy
    {
        IEnumerable<string> SupportedTypeNames { get; }

        ExpressionSyntax CreateValueExpression(ITypeSymbol symbol, SemanticModel model, HashSet<string> visitedTypes, IFrameworkSet frameworkSet);
    }
}
