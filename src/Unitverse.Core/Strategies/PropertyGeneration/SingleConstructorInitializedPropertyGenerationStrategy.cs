namespace Unitverse.Core.Strategies.PropertyGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public class SingleConstructorInitializedPropertyGenerationStrategy : IGenerationStrategy<IPropertyModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public SingleConstructorInitializedPropertyGenerationStrategy(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        public bool IsExclusive => false;

        public int Priority => 3;

        public Func<IStrategyOptions, bool> IsEnabled => x => x.InitializedPropertyChecksAreEnabled;

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

            if (_frameworkSet.Options.GenerationOptions.UseAutoFixture && _frameworkSet.Options.GenerationOptions.UseAutoFixtureForMocking)
            {
                return false;
            }

            // if this is a record type without a primary constructor and this property has an init accessor
            if (property.HasInit && !model.Constructors.Any() && !model.IsStatic)
            {
                return true;
            }

            // there is only one constructor that references this parameter, and it's the one with most parameters
            return model.Constructors.Count(x => x.Parameters.Any(p => string.Equals(p.Name, property.Name, StringComparison.OrdinalIgnoreCase))) == 1 &&
                model.DefaultConstructor != null && model.DefaultConstructor.Parameters.Any(p => string.Equals(p.Name, property.Name, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<SectionedMethodHandler> Create(IPropertyModel property, ClassModel model, NamingContext namingContext)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var method = _frameworkSet.CreateTestMethod(_frameworkSet.NamingProvider.IsInitializedCorrectly, namingContext, false, model.IsStatic, "Checks that the " + property.Name + " property is initialized correctly by the constructor.");
            method.Emit(GetPropertyAssertionBodyStatements(property, model).ToArray());

            yield return method;
        }

        private IEnumerable<StatementSyntax> GetPropertyAssertionBodyStatements(IPropertyModel property, ClassModel model)
        {
            if (!model.Constructors.Any() && property.HasInit)
            {
                yield return _frameworkSet.AssertionFramework.AssertEqual(property.Access(model.TargetInstance), model.GetConstructorFieldReference(property, _frameworkSet), property.TypeInfo.Type.IsReferenceTypeAndNotString());
            }
            else
            {
                var parameter = model.Constructors.SelectMany(x => x.Parameters).First(x => string.Equals(x.Name, property.Name, StringComparison.OrdinalIgnoreCase));

                yield return _frameworkSet.AssertionFramework.AssertEqual(property.Access(model.TargetInstance), model.GetConstructorFieldReference(parameter, _frameworkSet), property.TypeInfo.Type.IsReferenceTypeAndNotString());
            }
        }
    }
}