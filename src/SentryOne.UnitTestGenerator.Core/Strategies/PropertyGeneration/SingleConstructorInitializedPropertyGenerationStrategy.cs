namespace SentryOne.UnitTestGenerator.Core.Strategies.PropertyGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Models;

    public class SingleConstructorInitializedPropertyGenerationStrategy : IGenerationStrategy<IPropertyModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public SingleConstructorInitializedPropertyGenerationStrategy(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        public bool IsExclusive => false;

        public int Priority => 3;

        public bool CanHandle(IPropertyModel property, ClassModel model)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (property.IsStatic)
            {
                return false;
            }

            // there is only one constructor that references this parameter, and it's the one with most parameters
            return model.Constructors.Count(x => x.Parameters.Any(p => string.Equals(p.Name, property.Name, StringComparison.OrdinalIgnoreCase))) == 1 &&
                model.DefaultConstructor != null && model.DefaultConstructor.Parameters.Any(p => string.Equals(p.Name, property.Name, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<MethodDeclarationSyntax> Create(IPropertyModel property, ClassModel model)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var methodName = string.Format(CultureInfo.InvariantCulture, "{0}IsInitializedCorrectly", property.Name);
            var method = _frameworkSet.TestFramework.CreateTestMethod(methodName, false, model.IsStatic)
                .AddBodyStatements(GetPropertyAssertionBodyStatements(property, model).ToArray());

            yield return method;
        }

        private IEnumerable<StatementSyntax> GetPropertyAssertionBodyStatements(IPropertyModel property, ClassModel model)
        {
            var parameter = model.Constructors.SelectMany(x => x.Parameters).First(x => string.Equals(x.Name, property.Name, StringComparison.OrdinalIgnoreCase));

            yield return _frameworkSet.AssertionFramework.AssertEqual(property.Access(model.TargetInstance), model.GetConstructorFieldReference(parameter, _frameworkSet), property.TypeInfo.Type.IsReferenceType);
        }
    }
}