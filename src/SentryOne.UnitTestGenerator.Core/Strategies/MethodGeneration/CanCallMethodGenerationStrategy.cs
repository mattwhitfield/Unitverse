namespace SentryOne.UnitTestGenerator.Core.Strategies.MethodGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Resources;

    public class CanCallMethodGenerationStrategy : IGenerationStrategy<IMethodModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public CanCallMethodGenerationStrategy(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        public bool IsExclusive => false;

        public int Priority => 1;

        public bool CanHandle(IMethodModel method, ClassModel model)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return !method.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.AbstractKeyword));
        }

        public IEnumerable<MethodDeclarationSyntax> Create(IMethodModel method, ClassModel model)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var methodName = string.Format(CultureInfo.InvariantCulture, "CanCall{0}", model.GetMethodUniqueName(method));

            var generatedMethod = _frameworkSet.TestFramework.CreateTestMethod(methodName, method.IsAsync, model.IsStatic);

            var paramExpressions = new List<CSharpSyntaxNode>();

            foreach (var parameter in method.Parameters)
            {
                if (parameter.Node.Modifiers.Any(x => x.Kind() == SyntaxKind.OutKeyword))
                {
                    paramExpressions.Add(SyntaxFactory.Argument(SyntaxFactory.DeclarationExpression(SyntaxFactory.IdentifierName(Strings.Create_var), SyntaxFactory.SingleVariableDesignation(parameter.Identifier))).WithRefKindKeyword(SyntaxFactory.Token(SyntaxKind.OutKeyword)));
                }
                else
                {
                    var defaultAssignmentValue = AssignmentValueHelper.GetDefaultAssignmentValue(parameter.TypeInfo, model.SemanticModel, _frameworkSet);

                    generatedMethod = generatedMethod.AddBodyStatements(SyntaxFactory.LocalDeclarationStatement(
                        SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName(Strings.Create_var))
                                     .WithVariables(SyntaxFactory.SingletonSeparatedList(
                                                       SyntaxFactory.VariableDeclarator(parameter.Identifier)
                                                                    .WithInitializer(SyntaxFactory.EqualsValueClause(defaultAssignmentValue))))));

                    if (parameter.Node.Modifiers.Any(x => x.Kind() == SyntaxKind.RefKeyword))
                    {
                        paramExpressions.Add(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parameter.Identifier)).WithRefKindKeyword(SyntaxFactory.Token(SyntaxKind.RefKeyword)));
                    }
                    else
                    {
                        paramExpressions.Add(SyntaxFactory.IdentifierName(parameter.Identifier));
                    }
                }
            }

            var methodCall = method.Invoke(model, false, _frameworkSet, paramExpressions.ToArray());

            bool requiresInstance = false;
            if (method.IsAsync)
            {
                if (model.SemanticModel.GetSymbolInfo(method.Node.ReturnType).Symbol is INamedTypeSymbol type)
                {
                    requiresInstance = type.TypeArguments.Any();
                }
            }
            else
            {
                requiresInstance = !method.IsVoid;
            }

            StatementSyntax bodyStatement;

            if (requiresInstance)
            {
                bodyStatement = SyntaxFactory.LocalDeclarationStatement(
                    SyntaxFactory.VariableDeclaration(
                            SyntaxFactory.IdentifierName(Strings.Create_var))
                        .WithVariables(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.VariableDeclarator(
                                        SyntaxFactory.Identifier(Strings.CanCallMethodGenerationStrategy_Create_result))
                                    .WithInitializer(
                                        SyntaxFactory.EqualsValueClause(methodCall)))));
            }
            else
            {
                bodyStatement = SyntaxFactory.ExpressionStatement(methodCall);
            }

            generatedMethod = generatedMethod.AddBodyStatements(bodyStatement);

            generatedMethod = generatedMethod.AddBodyStatements(_frameworkSet.TestFramework.AssertFail(Strings.PlaceholderAssertionMessage));

            yield return generatedMethod;
        }
    }
}