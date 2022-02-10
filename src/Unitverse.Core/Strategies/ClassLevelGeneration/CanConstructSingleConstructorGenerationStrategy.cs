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

    public class CanConstructSingleConstructorGenerationStrategy : IGenerationStrategy<ClassModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public CanConstructSingleConstructorGenerationStrategy(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        public bool IsExclusive => false;

        public int Priority => 1;

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

            if (model.DefaultConstructor == null || model.IsStatic)
            {
                return false;
            }

            if (model.Declaration.ChildNodes().OfType<ConstructorDeclarationSyntax>().Count() == 1 && model.DefaultConstructor.Node.Modifiers.Any(x => x.Kind() == SyntaxKind.PublicKeyword))
            {
                return true;
            }

            return model.Declaration is RecordDeclarationSyntax;
        }

        public IEnumerable<MethodDeclarationSyntax> Create(ClassModel method, ClassModel model, NamingContext namingContext)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var generatedMethod = _frameworkSet.TestFramework.CreateTestMethod(_frameworkSet.NamingProvider.CanConstruct, namingContext, false, false);

            generatedMethod = generatedMethod.AddBodyStatements(Generate.ImplicitlyTypedVariableDeclaration("instance", model.GetObjectCreationExpression(_frameworkSet)));

            generatedMethod = generatedMethod.AddBodyStatements(_frameworkSet.AssertionFramework.AssertNotNull(SyntaxFactory.IdentifierName("instance")));

            yield return generatedMethod;
        }
    }
}