namespace Unitverse.Core.Strategies.ClassDecoration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Models;

    public class ClassDecorationStrategyFactory
    {
        private readonly IList<IClassDecorationStrategy> _strategies;

        public ClassDecorationStrategyFactory(IFrameworkSet frameworkSet)
        {
            var frameworkSet1 = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));

            _strategies = new List<IClassDecorationStrategy>
            {
                new RequiresStaGenerationStrategy(frameworkSet1),
            };
        }

        public TypeDeclarationSyntax Apply(TypeDeclarationSyntax syntax, ClassModel model)
        {
            var strategies = _strategies.OrderByDescending(x => x.Priority);

            foreach (var strategy in strategies)
            {
                if (strategy.CanHandle(syntax, model))
                {
                    syntax = strategy.Apply(syntax, model);
                }

                if (strategy.IsExclusive)
                {
                    break;
                }
            }

            return syntax;
        }
    }
}