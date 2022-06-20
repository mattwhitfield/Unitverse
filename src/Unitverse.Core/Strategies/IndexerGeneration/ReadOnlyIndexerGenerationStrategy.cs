namespace Unitverse.Core.Strategies.IndexerGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using Unitverse.Core.Resources;

    public class ReadOnlyIndexerGenerationStrategy : IGenerationStrategy<IIndexerModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public ReadOnlyIndexerGenerationStrategy(IFrameworkSet frameworkSet)
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

            // readonly property without a constructor initializer parameter
            return indexer.HasGet && !indexer.HasSet;
        }

        public IEnumerable<SectionedMethodHandler> Create(IIndexerModel indexer, ClassModel model, NamingContext namingContext)
        {
            if (indexer == null)
            {
                throw new ArgumentNullException(nameof(indexer));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var method = _frameworkSet.CreateTestMethod(_frameworkSet.NamingProvider.CanGet, namingContext, false, model.IsStatic, "Checks that the indexer functions correctly.");

            var paramExpressions = indexer.Parameters.Select(param => AssignmentValueHelper.GetDefaultAssignmentValue(param.TypeInfo, model.SemanticModel, _frameworkSet)).ToArray();

            method.Emit(_frameworkSet.AssertionFramework.AssertIsInstanceOf(Generate.IndexerAccess(model.TargetInstance, paramExpressions), indexer.TypeInfo.ToTypeSyntax(_frameworkSet.Context), indexer.TypeInfo.Type != null && indexer.TypeInfo.Type.IsReferenceType));
            method.Emit(_frameworkSet.AssertionFramework.AssertFail(Strings.PlaceholderAssertionMessage));

            yield return method;
        }
    }
}