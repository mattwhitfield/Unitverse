namespace Unitverse.Core.Strategies.OperatorGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using Unitverse.Core.Resources;

    public class CanCallOperatorGenerationStrategy : IGenerationStrategy<IOperatorModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public CanCallOperatorGenerationStrategy(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        public bool IsExclusive => false;

        public int Priority => 1;

        public bool CanHandle(IOperatorModel method, ClassModel model)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return true;
        }

        public IEnumerable<MethodDeclarationSyntax> Create(IOperatorModel method, ClassModel model, NamingContext namingContext)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var generatedMethod = _frameworkSet.TestFramework.CreateTestMethod(_frameworkSet.NamingProvider.CanCallOperator, namingContext, false, model.IsStatic);

            var paramExpressions = new List<CSharpSyntaxNode>();

            foreach (var parameter in method.Parameters)
            {
                var defaultAssignmentValue = AssignmentValueHelper.GetDefaultAssignmentValue(parameter.TypeInfo, model.SemanticModel, _frameworkSet, true);

                generatedMethod = generatedMethod.AddBodyStatements(SyntaxFactory.LocalDeclarationStatement(
                    SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName(Strings.Create_var))
                                 .WithVariables(SyntaxFactory.SingletonSeparatedList(
                                                   SyntaxFactory.VariableDeclarator(parameter.Identifier)
                                                                .WithInitializer(SyntaxFactory.EqualsValueClause(defaultAssignmentValue))))));

                paramExpressions.Add(SyntaxFactory.IdentifierName(parameter.Name));
            }

            var methodCall = method.Invoke(model, false, _frameworkSet, paramExpressions.ToArray());

            var bodyStatement = SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.IdentifierName(Strings.Create_var))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(
                                    SyntaxFactory.Identifier(Strings.CanCallMethodGenerationStrategy_Create_result))
                                .WithInitializer(
                                    SyntaxFactory.EqualsValueClause(methodCall)))));

            generatedMethod = generatedMethod.AddBodyStatements(bodyStatement);

            generatedMethod = generatedMethod.AddBodyStatements(_frameworkSet.AssertionFramework.AssertFail(Strings.PlaceholderAssertionMessage));

            yield return generatedMethod;
        }
    }
}