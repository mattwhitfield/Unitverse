namespace SentryOne.UnitTestGenerator.Core.Strategies.ClassGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Models;

    public class ClassGenerationStrategyFactory
    {
        private readonly IList<IClassGenerationStrategy> _strategies;

        public ClassGenerationStrategyFactory(IFrameworkSet frameworkSet)
        {
            var frameworkSet1 = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));

            _strategies = new List<IClassGenerationStrategy>
            {
                new StandardClassGenerationStrategy(frameworkSet1),
                new AbstractClassGenerationStrategy(frameworkSet1),
                new StaticClassGenerationStrategy(frameworkSet1),
            };
        }

        public TypeDeclarationSyntax CreateFor(ClassModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var strategy = _strategies.Where(x => x.CanHandle(model)).OrderByDescending(x => x.Priority).FirstOrDefault();
            if (strategy == null)
            {
                throw new InvalidOperationException("Cannot find a strategy for generation for the type " + model.ClassName);
            }

            return strategy.Create(model);
        }
    }
}