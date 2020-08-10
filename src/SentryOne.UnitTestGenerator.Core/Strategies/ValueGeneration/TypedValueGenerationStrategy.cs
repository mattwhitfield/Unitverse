namespace SentryOne.UnitTestGenerator.Core.Strategies.ValueGeneration
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Frameworks;

    public class TypedValueGenerationStrategy : IValueGenerationStrategy
    {
        private readonly Func<ITypeSymbol, SemanticModel, HashSet<string>, IFrameworkSet, ExpressionSyntax> _factory;

        public TypedValueGenerationStrategy(Func<ITypeSymbol, SemanticModel, HashSet<string>, IFrameworkSet, ExpressionSyntax> factory, params string[] typeNames)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            SupportedTypeNames = typeNames ?? throw new ArgumentNullException(nameof(typeNames));
        }

        public IEnumerable<string> SupportedTypeNames { get; }

        public ExpressionSyntax CreateValueExpression(ITypeSymbol symbol, SemanticModel model, HashSet<string> visitedTypes, IFrameworkSet frameworkSet)
        {
            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (frameworkSet is null)
            {
                throw new ArgumentNullException(nameof(frameworkSet));
            }

            return _factory(symbol, model, visitedTypes, frameworkSet);
        }
    }
}