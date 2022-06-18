namespace Unitverse.Core.Frameworks.Mocking
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;

    internal class AutoFixtureMockingAdaptor : IMockingFramework
    {
        private readonly IMockingFramework _targetFramework;
        private readonly IGenerationContext _context;

        public AutoFixtureMockingAdaptor(IMockingFramework targetFramework, IGenerationContext context)
        {
            _targetFramework = targetFramework ?? throw new ArgumentNullException(nameof(targetFramework));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public bool AwaitAsyncAssertions => _targetFramework.AwaitAsyncAssertions;

        public ExpressionSyntax GetAssertionFor(IMethodSymbol dependencyMethod, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, IEnumerable<string> parameters)
        {
            return _targetFramework.GetAssertionFor(dependencyMethod, mockFieldName, model, frameworkSet, parameters);
        }

        public ExpressionSyntax GetFieldInitializer(TypeSyntax type)
        {
            var mockFieldType = GetFieldType(type);
            if (mockFieldType != type)
            {
                _context.MocksUsed = true;
            }

            return AutoFixtureHelper.Freeze(mockFieldType, _context);
        }

        public ExpressionSyntax GetFieldReference(ExpressionSyntax fieldReference)
        {
            return _targetFramework.GetFieldReference(fieldReference);
        }

        public TypeSyntax GetFieldType(TypeSyntax type)
        {
            return _targetFramework.GetFieldType(type);
        }

        public ExpressionSyntax GetObjectCreationExpression(TypeSyntax typeSyntax)
        {
            return AutoFixtureHelper.Create(typeSyntax, _context);
        }

        public ExpressionSyntax GetSetupFor(IMethodSymbol dependencyMethod, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, ExpressionSyntax expectedReturnValue, IEnumerable<string> parameters)
        {
            _context.MocksUsed = true;
            return _targetFramework.GetSetupFor(dependencyMethod, mockFieldName, model, frameworkSet, expectedReturnValue, parameters);
        }

        public ExpressionSyntax GetSetupFor(IPropertySymbol dependencyProperty, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, ExpressionSyntax expectedReturnValue)
        {
            _context.MocksUsed = true;
            return _targetFramework.GetSetupFor(dependencyProperty, mockFieldName, model, frameworkSet, expectedReturnValue);
        }

        public ExpressionSyntax GetThrowawayReference(TypeSyntax type)
        {
            return AutoFixtureHelper.Create(type, _context);
        }

        public IEnumerable<UsingDirectiveSyntax> GetUsings()
        {
            return _targetFramework.GetUsings();
        }
    }
}
