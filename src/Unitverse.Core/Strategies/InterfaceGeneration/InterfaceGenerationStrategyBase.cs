namespace Unitverse.Core.Strategies.InterfaceGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public abstract class InterfaceGenerationStrategyBase : IGenerationStrategy<ClassModel>
    {
        protected InterfaceGenerationStrategyBase(IFrameworkSet frameworkSet)
        {
            FrameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        public virtual bool IsExclusive => false;

        public virtual int Priority => 2;

        public abstract string SupportedInterfaceName { get; }

        protected IFrameworkSet FrameworkSet { get; }

        protected abstract NameResolver GeneratedMethodNamePattern { get; }

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

            return classModel.Interfaces.Any(y => y.InterfaceName == SupportedInterfaceName);
        }

        public abstract IEnumerable<MethodDeclarationSyntax> Create(ClassModel classModel, ClassModel model, NamingContext namingContext);

        protected IEnumerable<MethodDeclarationSyntax> GenerateMethods(ClassModel classModel, ClassModel model, NamingContext namingContext, Func<ClassModel, IInterfaceModel, IEnumerable<StatementSyntax>> generateBody)
        {
            if (classModel == null)
            {
                throw new ArgumentNullException(nameof(classModel));
            }

            foreach (var interfaceModel in classModel.Interfaces.Where(x => x.InterfaceName == SupportedInterfaceName))
            {
                var typeParameters = interfaceModel.GenericTypes.Select(x => x.Name).Aggregate(string.Empty, (current, next) => current + $"_{next}");
                namingContext = namingContext.WithInterfaceName(interfaceModel.InterfaceName).WithTypeParameters(typeParameters);

                var method = FrameworkSet.TestFramework.CreateTestMethod(GeneratedMethodNamePattern, namingContext, false, model != null && model.IsStatic);
                var body = generateBody(classModel, interfaceModel);
                yield return method.AddBodyStatements(body.ToArray());
            }
        }
    }
}