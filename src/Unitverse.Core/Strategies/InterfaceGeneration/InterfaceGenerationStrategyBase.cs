﻿namespace Unitverse.Core.Strategies.InterfaceGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public abstract class InterfaceGenerationStrategyBase : IGenerationStrategy<ClassModel>
    {
        protected InterfaceGenerationStrategyBase(IFrameworkSet frameworkSet, string supportedInterfaceName)
        {
            FrameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
            SupportedInterfaceName = supportedInterfaceName;
        }

        public virtual bool IsExclusive => false;

        public virtual int Priority => 2;

        private string SupportedInterfaceName { get; }

        public virtual int MinimumRequiredGenericParameterCount => 0;

        public virtual int MaximumRequiredGenericParameterCount => 1;

        protected IFrameworkSet FrameworkSet { get; }

        protected abstract NameResolver GeneratedMethodNamePattern { get; }

        public Func<IStrategyOptions, bool> IsEnabled => x => x.InterfaceImplementationChecksAreEnabled;

        public virtual bool CanHandle(ClassModel classModel, ClassModel model)
        {
            if (classModel == null)
            {
                throw new ArgumentNullException(nameof(classModel));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return classModel.Interfaces.Any(y => y.InterfaceName == SupportedInterfaceName && y.GenericTypes.Count >= MinimumRequiredGenericParameterCount && y.GenericTypes.Count <= MaximumRequiredGenericParameterCount);
        }

        public IEnumerable<SectionedMethodHandler> Create(ClassModel classModel, ClassModel model, NamingContext namingContext)
        {
            if (classModel is null)
            {
                throw new ArgumentNullException(nameof(classModel));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            foreach (var interfaceModel in classModel.Interfaces.Where(x => x.InterfaceName == SupportedInterfaceName))
            {
                var typeParameters = interfaceModel.GenericTypes.Select(x => x.Name).Aggregate(string.Empty, (current, next) => current + $"_{next}");
                namingContext = namingContext.WithInterfaceName(interfaceModel.InterfaceName).WithTypeParameters(typeParameters);

                var method = FrameworkSet.CreateTestMethod(GeneratedMethodNamePattern, namingContext, false, model != null && model.IsStatic, "Checks that the " + SupportedInterfaceName + " interface is implemented correctly.");
                AddBodyStatements(classModel, interfaceModel, method);
                yield return method;
            }
        }

        protected abstract void AddBodyStatements(ClassModel sourceModel, IInterfaceModel interfaceModel, SectionedMethodHandler method);
    }
}