namespace SentryOne.UnitTestGenerator.Core.Strategies.ValueGeneration
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Frameworks;

    public interface IValueGenerationStrategy
    {
        IEnumerable<string> SupportedTypeNames { get; }

        ExpressionSyntax CreateValueExpression(ITypeSymbol symbol, SemanticModel model, IFrameworkSet frameworkSet);
    }
}
