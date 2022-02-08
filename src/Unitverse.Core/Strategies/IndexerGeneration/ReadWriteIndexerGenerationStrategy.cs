namespace Unitverse.Core.Strategies.IndexerGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using Unitverse.Core.Resources;

    public class ReadWriteIndexerGenerationStrategy : IGenerationStrategy<IIndexerModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public ReadWriteIndexerGenerationStrategy(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        public bool IsExclusive => false;

        public int Priority => 1;

        public bool CanHandle(IIndexerModel property, ClassModel model)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return property.HasGet && property.HasSet;
        }

        public IEnumerable<MethodDeclarationSyntax> Create(IIndexerModel indexer, ClassModel model, NamingContext namingContext)
        {
            if (indexer == null)
            {
                throw new ArgumentNullException(nameof(indexer));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var method = _frameworkSet.TestFramework.CreateTestMethod(_frameworkSet.NamingProvider.CanSetAndGet, namingContext, false, model.IsStatic)
                .AddBodyStatements(GetPropertyAssertionBodyStatements(indexer, model).ToArray());

            yield return method;
        }

        private IEnumerable<StatementSyntax> GetPropertyAssertionBodyStatements(IIndexerModel indexer, ClassModel sourceModel)
        {
            var paramExpressions = indexer.Parameters.Select(param => AssignmentValueHelper.GetDefaultAssignmentValue(param.TypeInfo, sourceModel.SemanticModel, _frameworkSet)).ToArray();

            yield return SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(AssignmentValueHelper.GetTypeOrImplicitType(indexer.TypeInfo.Type, _frameworkSet))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(
                                    SyntaxFactory.Identifier(Strings.ReadWritePropertyGenerationStrategy_GetPropertyAssertionBodyStatements_testValue))
                                .WithInitializer(
                                    SyntaxFactory.EqualsValueClause(
                                        AssignmentValueHelper.GetDefaultAssignmentValue(indexer.TypeInfo, sourceModel.SemanticModel, _frameworkSet))))));

            yield return _frameworkSet.AssertionFramework.AssertIsInstanceOf(Generate.IndexerAccess(sourceModel.TargetInstance, paramExpressions), indexer.TypeInfo.ToTypeSyntax(_frameworkSet.Context), indexer.TypeInfo.Type.IsReferenceType);

            yield return SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, Generate.IndexerAccess(sourceModel.TargetInstance, paramExpressions), SyntaxFactory.IdentifierName(Strings.ReadWritePropertyGenerationStrategy_GetPropertyAssertionBodyStatements_testValue)));

            yield return _frameworkSet.AssertionFramework.AssertEqual(Generate.IndexerAccess(sourceModel.TargetInstance, paramExpressions), SyntaxFactory.IdentifierName(Strings.ReadWritePropertyGenerationStrategy_GetPropertyAssertionBodyStatements_testValue), indexer.TypeInfo.Type.IsReferenceType);
        }
    }
}