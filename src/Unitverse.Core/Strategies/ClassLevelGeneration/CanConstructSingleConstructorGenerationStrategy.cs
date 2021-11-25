namespace Unitverse.Core.Strategies.ClassLevelGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using Unitverse.Core.Resources;

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

            return model.Declaration.ChildNodes().OfType<ConstructorDeclarationSyntax>().Count() == 1 && model.DefaultConstructor != null && !model.IsStatic && model.DefaultConstructor.Node.Modifiers.Any(x => x.Kind() == SyntaxKind.PublicKeyword);
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

            var tokenList = model.DefaultConstructor.Parameters.Select(parameter => model.GetConstructorFieldReference(parameter, _frameworkSet)).Cast<ExpressionSyntax>().ToList();

            generatedMethod = generatedMethod.AddBodyStatements(SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.IdentifierName(Strings.Create_var))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(
                                    SyntaxFactory.Identifier(Strings.CanConstructMultiConstructorGenerationStrategy_Create_instance))
                                .WithInitializer(
                                    SyntaxFactory.EqualsValueClause(
                                        Generate.ObjectCreation(model.TypeSyntax, tokenList.ToArray())))))));

            generatedMethod = generatedMethod.AddBodyStatements(_frameworkSet.AssertionFramework.AssertNotNull(SyntaxFactory.IdentifierName(Strings.CanConstructMultiConstructorGenerationStrategy_Create_instance)));

            yield return generatedMethod;
        }
    }
}