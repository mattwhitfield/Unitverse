namespace SentryOne.UnitTestGenerator.Core.Strategies.PropertyGeneration
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

            // readonly property without a constructor initializer parameter
            return property.HasGet && !property.HasSet && !model.Constructors.Any(x => x.Parameters.Any(p => string.Equals(p.Name, property.Name, StringComparison.OrdinalIgnoreCase)));
        }

        public IEnumerable<MethodDeclarationSyntax> Create(IPropertyModel property, ClassModel model)
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

            var method = _frameworkSet.TestFramework.CreateTestMethod(string.Format(CultureInfo.InvariantCulture, "CanGet{0}", property.Name), false, model.IsStatic)
                .AddBodyStatements(_frameworkSet.AssertionFramework.AssertIsInstanceOf(property.Access(target), property.TypeInfo.ToTypeSyntax(_frameworkSet.Context), property.TypeInfo.Type.IsReferenceType))
                .AddBodyStatements(_frameworkSet.AssertionFramework.AssertFail(Strings.PlaceholderAssertionMessage));

            yield return method;
        }
    }
}