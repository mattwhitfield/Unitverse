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
    using Unitverse.Core.Resources;

    public class CanConstructMultiConstructorGenerationStrategy : IGenerationStrategy<ClassModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public CanConstructMultiConstructorGenerationStrategy(IFrameworkSet frameworkSet)
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

            return model.Declaration.ChildNodes().OfType<ConstructorDeclarationSyntax>().Count(x => x.Modifiers.All(m => !m.IsKind(SyntaxKind.StaticKeyword))) > 1 && !model.IsStatic;
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

            bool isFirst = true;
            foreach (var constructor in model.Constructors)
            {
                var tokenList = constructor.Parameters.Select(parameter => model.GetConstructorFieldReference(parameter, _frameworkSet)).Cast<ExpressionSyntax>().ToList();

                var creationExpression = Generate.ObjectCreation(model.TypeSyntax, tokenList.ToArray());

                if (isFirst)
                {
                    var variables = SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(Strings.CanConstructMultiConstructorGenerationStrategy_Create_instance)).WithInitializer(SyntaxFactory.EqualsValueClause(creationExpression)));
                    generatedMethod = generatedMethod.AddBodyStatements(SyntaxFactory.LocalDeclarationStatement(SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName(Strings.Create_var)).WithVariables(variables)));
                    isFirst = false;
                }
                else
                {
                    generatedMethod = generatedMethod.AddBodyStatements(SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName(Strings.CanConstructMultiConstructorGenerationStrategy_Create_instance), creationExpression)));
                }

                generatedMethod = generatedMethod.AddBodyStatements(_frameworkSet.AssertionFramework.AssertNotNull(SyntaxFactory.IdentifierName(Strings.CanConstructMultiConstructorGenerationStrategy_Create_instance)));
            }

            yield return generatedMethod;
        }
    }
}