namespace SentryOne.UnitTestGenerator.Core.Strategies.ValueGeneration
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Frameworks;

    internal class SimpleValueGenerationStrategy : IValueGenerationStrategy
    {
        private readonly Func<ExpressionSyntax> _factory;

        public SimpleValueGenerationStrategy(Func<ExpressionSyntax> factory, params string[] typeNames)
        {
            _factory = factory;
            SupportedTypeNames = typeNames;
        }

        public IEnumerable<string> SupportedTypeNames { get; }

        public ExpressionSyntax CreateValueExpression(ITypeSymbol symbol, SemanticModel model, IFrameworkSet frameworkSet)
        {
            return _factory();
        }
    }
}