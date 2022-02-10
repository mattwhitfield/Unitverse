namespace Unitverse.Core.Strategies.PropertyGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using Unitverse.Core.Resources;

    public class ReadOnlyPropertyGenerationStrategy : IGenerationStrategy<IPropertyModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public ReadOnlyPropertyGenerationStrategy(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        public bool IsExclusive => false;

        public int Priority => 2;

        public bool CanHandle(IPropertyModel property, ClassModel model)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // if this is a record type without a primary constructor and this property has an init accessor
            if (property.HasInit && !model.Constructors.Any() && !model.IsStatic)
            {
                return false;
            }

            // readonly property without a constructor initializer parameter
            return property.HasGet && !property.HasSet && !model.Constructors.Any(x => x.Parameters.Any(p => string.Equals(p.Name, property.Name, StringComparison.OrdinalIgnoreCase)));
        }

        public IEnumerable<MethodDeclarationSyntax> Create(IPropertyModel property, ClassModel model, NamingContext namingContext)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var target = property.IsStatic ? model.TypeSyntax : model.TargetInstance;

            var interfaceMethodsImplemented = model.GetImplementedInterfaceSymbolsFor(property.Symbol);
            var testIsComplete = MockHelper.PrepareMockCalls(model, property.Node, property.Access(target), interfaceMethodsImplemented, Enumerable.Empty<string>(), _frameworkSet, out var mockSetupStatements, out var mockAssertionStatements);

            var method = _frameworkSet.TestFramework.CreateTestMethod(_frameworkSet.NamingProvider.CanGet, namingContext, false, model.IsStatic);

            method = MockHelper.EmitStatementListWithTrivia(method, mockSetupStatements, null, testIsComplete ? string.Empty : Environment.NewLine + Environment.NewLine);

            if (!testIsComplete)
            {
                var bodyStatement = _frameworkSet.AssertionFramework.AssertIsInstanceOf(property.Access(target), property.TypeInfo.ToTypeSyntax(_frameworkSet.Context), property.TypeInfo.Type.IsReferenceType);
                if (mockAssertionStatements.Count > 0)
                {
                    bodyStatement = bodyStatement.WithTrailingTrivia(SyntaxFactory.Comment(Environment.NewLine + Environment.NewLine));
                }

                method = method.AddBodyStatements(bodyStatement);
            }

            method = MockHelper.EmitStatementListWithTrivia(method, mockAssertionStatements, null, testIsComplete ? string.Empty : Environment.NewLine + Environment.NewLine);

            if (!testIsComplete)
            {
                method = method.AddBodyStatements(_frameworkSet.AssertionFramework.AssertFail(Strings.PlaceholderAssertionMessage));
            }

            yield return method;
        }
    }
}