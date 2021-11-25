namespace Unitverse.Core.Strategies.PropertyGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using Unitverse.Core.Resources;

    public class ReadWritePropertyGenerationStrategy : IGenerationStrategy<IPropertyModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public ReadWritePropertyGenerationStrategy(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        public bool IsExclusive => false;

        public int Priority => 1;

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

            var method = _frameworkSet.TestFramework.CreateTestMethod(_frameworkSet.NamingProvider.CanSetAndGet, namingContext, false, model.IsStatic)
                .AddBodyStatements(GetPropertyAssertionBodyStatements(property, model).ToArray());

            yield return method;
        }

        private IEnumerable<StatementSyntax> GetPropertyAssertionBodyStatements(IPropertyModel property, ClassModel sourceModel)
        {
            yield return SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.IdentifierName(Strings.Create_var))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(
                                    SyntaxFactory.Identifier(Strings.ReadWritePropertyGenerationStrategy_GetPropertyAssertionBodyStatements_testValue))
                                .WithInitializer(
                                    SyntaxFactory.EqualsValueClause(
                                        AssignmentValueHelper.GetDefaultAssignmentValue(property.TypeInfo, sourceModel.SemanticModel, _frameworkSet))))));

            var target = property.IsStatic ? sourceModel.TypeSyntax : sourceModel.TargetInstance;

            yield return SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, property.Access(target), SyntaxFactory.IdentifierName(Strings.ReadWritePropertyGenerationStrategy_GetPropertyAssertionBodyStatements_testValue)));

            yield return _frameworkSet.AssertionFramework.AssertEqual(property.Access(target), SyntaxFactory.IdentifierName(Strings.ReadWritePropertyGenerationStrategy_GetPropertyAssertionBodyStatements_testValue), property.TypeInfo.Type.IsReferenceType);
        }
    }
}