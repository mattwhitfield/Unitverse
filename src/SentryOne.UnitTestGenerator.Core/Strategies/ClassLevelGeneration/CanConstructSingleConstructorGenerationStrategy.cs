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

    internal class CanConstructSingleConstructorGenerationStrategy : IGenerationStrategy<ClassModel>
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
            return model.Declaration.ChildNodes().OfType<ConstructorDeclarationSyntax>().Count() == 1 && model.DefaultConstructor != null && !model.IsStatic && model.DefaultConstructor.Node.Modifiers.Any(x => x.Kind() == SyntaxKind.PublicKeyword);
        }

        public IEnumerable<MethodDeclarationSyntax> Create(ClassModel method, ClassModel model)
        {
            var generatedMethod = _frameworkSet.TestFramework.CreateTestMethod("CanConstruct", false, false);

            var tokenList = model.DefaultConstructor.Parameters.Select(parameter => SyntaxFactory.IdentifierName(model.GetConstructorParameterFieldName(parameter))).Cast<ExpressionSyntax>().ToList();

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

            generatedMethod = generatedMethod.AddBodyStatements(_frameworkSet.TestFramework.AssertNotNull(SyntaxFactory.IdentifierName(Strings.CanConstructMultiConstructorGenerationStrategy_Create_instance)));

            yield return generatedMethod;
        }
    }
}