namespace Unitverse.Core.Strategies.ClassGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;

    public class ClassGenerationStrategyFactory
    {
        private readonly IList<IClassGenerationStrategy> _strategies;
        private readonly IFrameworkSet _frameworkSet;

        public ClassGenerationStrategyFactory(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));

            _strategies = new List<IClassGenerationStrategy>
            {
                new StandardClassGenerationStrategy(_frameworkSet),
                new AbstractClassGenerationStrategy(_frameworkSet),
                new StaticClassGenerationStrategy(_frameworkSet),
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

            var classSyntax = strategy.Create(model);

            if (_frameworkSet.Options.GenerationOptions.EmitXmlDocumentation)
            {
                var documentation = XmlCommentHelper.DocumentationComment(
                    XmlCommentHelper.Summary(
                        XmlCommentHelper.TextLiteral("Unit tests for the type "),
                        XmlCommentHelper.See(model.ClassName),
                        XmlCommentHelper.TextLiteral(".")));
                classSyntax = classSyntax.WithXmlDocumentation(documentation);
            }

            if (!string.IsNullOrWhiteSpace(_frameworkSet.Options.GenerationOptions.TestTypeBaseClass))
            {
                classSyntax = classSyntax.WithBaseList(Generate.BaseList(_frameworkSet.Options.GenerationOptions.TestTypeBaseClass));
            }

            return classSyntax;
        }
    }
}