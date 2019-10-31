namespace SentryOne.UnitTestGenerator.Core.Strategies.IndexerGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Resources;

    public class ReadOnlyIndexerGenerationStrategy : IGenerationStrategy<IIndexerModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public ReadOnlyIndexerGenerationStrategy(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        public bool IsExclusive => false;

        public int Priority => 2;

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

        public IEnumerable<MethodDeclarationSyntax> Create(IIndexerModel indexer, ClassModel model)
        {
            if (indexer == null)
            {
                throw new ArgumentNullException(nameof(indexer));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var paramExpressions = indexer.Parameters.Select(param => AssignmentValueHelper.GetDefaultAssignmentValue(param.TypeInfo, model.SemanticModel, _frameworkSet)).ToArray();

            var method = _frameworkSet.TestFramework.CreateTestMethod(string.Format(CultureInfo.InvariantCulture, "CanGet{0}", model.GetIndexerName(indexer)), false, model.IsStatic)
                .AddBodyStatements(_frameworkSet.TestFramework.AssertIsInstanceOf(Generate.IndexerAccess(model.TargetInstance, paramExpressions), indexer.TypeInfo.ToTypeSyntax(_frameworkSet.Context)))
                .AddBodyStatements(_frameworkSet.TestFramework.AssertFail(Strings.PlaceholderAssertionMessage));

            yield return method;
        }
    }
}