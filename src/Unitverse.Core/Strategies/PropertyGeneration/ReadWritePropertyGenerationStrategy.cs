namespace Unitverse.Core.Strategies.PropertyGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public class ReadWritePropertyGenerationStrategy : IGenerationStrategy<IPropertyModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public ReadWritePropertyGenerationStrategy(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        public bool IsExclusive => false;

        public int Priority => 1;

        public Func<IStrategyOptions, bool> IsEnabled => x => x.PropertyChecksAreEnabled;

        public bool CanHandle(IPropertyModel property, ClassModel model)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return property.HasGet && property.HasSet;
        }

        public IEnumerable<SectionedMethodHandler> Create(IPropertyModel property, ClassModel model, NamingContext namingContext)
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
            MockHelper.PrepareMockCalls(model, property.Node, property.Access(target), interfaceMethodsImplemented, Enumerable.Empty<string>(), _frameworkSet, out var mockSetupStatements, out var mockAssertionStatements);

            var method = _frameworkSet.CreateTestMethod(_frameworkSet.NamingProvider.CanSetAndGet, namingContext, false, model.IsStatic, "Checks that the " + property.Name + " property can be read from and written to.");

            method.Arrange(mockSetupStatements);
            method.BlankLine();

            var defaultValue = AssignmentValueHelper.GetDefaultAssignmentValue(property.TypeInfo, model.SemanticModel, _frameworkSet);
            var declareTestValue = Generate.VariableDeclaration(property.TypeInfo.Type, _frameworkSet, "testValue", defaultValue);

            method.Arrange(declareTestValue);

            method.Act(SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, property.Access(target), SyntaxFactory.IdentifierName("testValue"))));

            var bodyStatement = _frameworkSet.AssertionFramework.AssertEqual(property.Access(target), SyntaxFactory.IdentifierName("testValue"), property.TypeInfo.Type.IsReferenceTypeAndNotString());

            method.Assert(bodyStatement);
            method.BlankLine();

            method.Assert(mockAssertionStatements);

            yield return method;
        }
    }
}