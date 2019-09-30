namespace SentryOne.UnitTestGenerator.Core.Strategies.MethodGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Resources;

    internal class CanCallOperatorGenerationStrategy : IGenerationStrategy<IOperatorModel>
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
            return true;
        }

        public IEnumerable<MethodDeclarationSyntax> Create(IOperatorModel method, ClassModel model)
        {
            var methodName = string.Format(CultureInfo.InvariantCulture, "CanCall{0}Operator", method.Name);

            var generatedMethod = _frameworkSet.TestFramework.CreateTestMethod(methodName, false, model.IsStatic);

            var paramExpressions = new List<CSharpSyntaxNode>();

            foreach (var parameter in method.Parameters)
            {
                var defaultAssignmentValue = AssignmentValueHelper.GetDefaultAssignmentValue(parameter.TypeInfo, model.SemanticModel, _frameworkSet);

                generatedMethod = generatedMethod.AddBodyStatements(SyntaxFactory.LocalDeclarationStatement(
                    SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName(Strings.Create_var))
                                 .WithVariables(SyntaxFactory.SingletonSeparatedList(
                                                   SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(parameter.Name))
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

            generatedMethod = generatedMethod.AddBodyStatements(_frameworkSet.TestFramework.AssertFail(Strings.PlaceholderAssertionMessage));

            yield return generatedMethod;
        }
    }
}