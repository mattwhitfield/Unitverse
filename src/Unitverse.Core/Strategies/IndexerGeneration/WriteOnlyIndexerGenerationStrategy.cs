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

    public class WriteOnlyIndexerGenerationStrategy : IGenerationStrategy<IIndexerModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public WriteOnlyIndexerGenerationStrategy(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        public bool IsExclusive => false;

        public int Priority => 2;

        public Func<IStrategyOptions, bool> IsEnabled => x => x.IndexerChecksAreEnabled;

        public bool CanHandle(IIndexerModel indexer, ClassModel model)
        {
            if (indexer == null)
            {
                throw new ArgumentNullException(nameof(indexer));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return !indexer.HasGet && indexer.HasSet;
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

            var method = _frameworkSet.TestFramework.CreateTestMethod(_frameworkSet.NamingProvider.CanSet, namingContext, false, model.IsStatic, "Checks that the indexer functions correctly.");
            method.Emit(GetPropertyAssertionBodyStatements(indexer, model).ToArray());

            yield return method.Method;
        }

        private IEnumerable<StatementSyntax> GetPropertyAssertionBodyStatements(IIndexerModel indexer, ClassModel sourceModel)
        {
            var paramExpressions = indexer.Parameters.Select(param => AssignmentValueHelper.GetDefaultAssignmentValue(param.TypeInfo, sourceModel.SemanticModel, _frameworkSet)).ToArray();

            var defaultAssignmentValue = AssignmentValueHelper.GetDefaultAssignmentValue(indexer.TypeInfo, sourceModel.SemanticModel, _frameworkSet);
            yield return SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, Generate.IndexerAccess(sourceModel.TargetInstance, paramExpressions), defaultAssignmentValue));
            yield return _frameworkSet.AssertionFramework.AssertFail(Strings.PlaceholderAssertionMessage);
        }
    }
}