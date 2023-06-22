namespace Unitverse.Core.Strategies.ClassLevelGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public abstract class ParameterCheckConstructorGenerationStrategyBase : IGenerationStrategy<ClassModel>
    {
        protected IFrameworkSet FrameworkSet { get; }

        public bool IsExclusive => false;

        public int Priority => 1;

        public Func<IStrategyOptions, bool> IsEnabled => x => x.ConstructorParameterChecksAreEnabled;

        public ParameterCheckConstructorGenerationStrategyBase(IFrameworkSet frameworkSet)
        {
            FrameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        public bool CanHandle(ClassModel method, ClassModel model)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Declaration is RecordDeclarationSyntax)
            {
                return false;
            }

            return model.Constructors.SelectMany(x => x.Parameters).Any(CanHandleParameter) && !model.IsStatic;
        }

        protected abstract bool CanHandleParameter(ParameterModel parameter);

        public abstract IEnumerable<SectionedMethodHandler> Create(ClassModel member, ClassModel model, NamingContext namingContext);

        protected ExpressionSyntax GetFieldReferenceOrNewObjectFor(ClassModel model, ParameterModel parameter)
        {
            if (FrameworkSet.Options.GenerationOptions.UseFieldsForConstructorParameterTests)
            {
                return model.GetConstructorFieldReference(parameter, FrameworkSet);
            }

            return AssignmentValueHelper.GetDefaultAssignmentValue(parameter.TypeInfo, model.SemanticModel, FrameworkSet);
        }
    }
}