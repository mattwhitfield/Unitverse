namespace SentryOne.UnitTestGenerator.Core.Strategies.ClassLevelGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Resources;

    public class CanConstructNoConstructorGenerationStrategy : IGenerationStrategy<ClassModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public CanConstructNoConstructorGenerationStrategy(IFrameworkSet frameworkSet)
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

            return !model.Declaration.ChildNodes().OfType<ConstructorDeclarationSyntax>().Any() && !model.IsStatic;
        }

        public IEnumerable<MethodDeclarationSyntax> Create(ClassModel method, ClassModel model)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var generatedMethod = _frameworkSet.TestFramework.CreateTestMethod("CanConstruct", false, false);

            generatedMethod = generatedMethod.AddBodyStatements(SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.IdentifierName(Strings.Create_var))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(
                                    SyntaxFactory.Identifier(Strings.CanConstructMultiConstructorGenerationStrategy_Create_instance))
                                .WithInitializer(
                                    SyntaxFactory.EqualsValueClause(
                                        Generate.ObjectCreation(model.TypeSyntax)))))));

            generatedMethod = generatedMethod.AddBodyStatements(_frameworkSet.TestFramework.AssertNotNull(SyntaxFactory.IdentifierName(Strings.CanConstructMultiConstructorGenerationStrategy_Create_instance)));

            yield return generatedMethod;
        }
    }
}