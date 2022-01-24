namespace Unitverse.Core.Strategies.MethodGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using Unitverse.Core.Resources;

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

        public IEnumerable<MethodDeclarationSyntax> Create(IMethodModel method, ClassModel model, NamingContext namingContext)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var generatedMethod = _frameworkSet.TestFramework.CreateTestMethod(_frameworkSet.NamingProvider.CanCall, namingContext, method.IsAsync, model.IsStatic);

            var mockSetupStatements = new List<StatementSyntax>();
            var mockAssertionStatements = new List<StatementSyntax>();

            if (_frameworkSet.Options.GenerationOptions.AutomaticallyConfigureMocks)
            {
                PrepareMockCalls(model, method, mockSetupStatements, mockAssertionStatements);
            }

            var paramExpressions = new List<CSharpSyntaxNode>();

            generatedMethod = EmitStatementListWithTrivia(generatedMethod, mockSetupStatements, "// Set up mocks" + Environment.NewLine, Environment.NewLine + Environment.NewLine);

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
                        SyntaxFactory.VariableDeclaration(AssignmentValueHelper.GetTypeOrImplicitType(parameter.TypeInfo.Type, _frameworkSet))
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

            if (mockAssertionStatements.Count > 0)
            {
                bodyStatement = bodyStatement.WithTrailingTrivia(SyntaxFactory.Comment(Environment.NewLine + Environment.NewLine));
            }

            generatedMethod = generatedMethod.AddBodyStatements(bodyStatement);

            generatedMethod = EmitStatementListWithTrivia(generatedMethod, mockAssertionStatements, "// Verify mocks" + Environment.NewLine, Environment.NewLine + Environment.NewLine);

            generatedMethod = generatedMethod.AddBodyStatements(_frameworkSet.AssertionFramework.AssertFail(Strings.PlaceholderAssertionMessage));

            yield return generatedMethod;
        }

        private static MethodDeclarationSyntax EmitStatementListWithTrivia(MethodDeclarationSyntax method, List<StatementSyntax> statements, string leadingComment, string trailingComment)
        {
            if (statements.Any())
            {
                for (int i = 0; i < statements.Count; i++)
                {
                    var statement = statements[i];
                    if (i == 0)
                    {
                        statement = statement.WithLeadingTrivia(SyntaxFactory.Comment(leadingComment));
                    }

                    if (i == statements.Count - 1)
                    {
                        statement = statement.WithTrailingTrivia(SyntaxFactory.Comment(trailingComment));
                    }

                    method = method.AddBodyStatements(statement);
                }
            }

            return method;
        }

        private void PrepareMockCalls(ClassModel model, IMethodModel method, List<StatementSyntax> mockSetupStatements, List<StatementSyntax> mockAssertionStatements)
        {
            var mappedInterfaceFields = model.DependencyMap.MappedInterfaceFields.ToList();
            var dependencyMap = InvocationExtractor.ExtractFrom(method.Node, model.SemanticModel, mappedInterfaceFields);

            foreach (var field in mappedInterfaceFields)
            {
                var fieldType = model.DependencyMap.GetTypeSymbolFor(field);
                if (fieldType != null)
                {
                    var constructorParameters = model.DependencyMap.GetConstructorParametersFor(field);
                    foreach (var constructorParameter in constructorParameters)
                    {
                        var mockFieldName = model.GetConstructorParameterFieldName(constructorParameter, _frameworkSet.NamingProvider);
                        foreach (var dependencyMethod in dependencyMap.GetAccessedMethodSymbolsFor(field))
                        {
                            if (dependencyMethod.ContainingType == fieldType)
                            {
                                var isAsync = dependencyMethod.ReturnType is INamedTypeSymbol namedType && namedType.Name == "Task" && namedType.ContainingNamespace.ToDisplayString() == "System.Threading.Tasks";
                                var isPlainTaskReturnType = isAsync && ((INamedTypeSymbol)dependencyMethod.ReturnType).TypeArguments.Length == 0;

                                if (!dependencyMethod.ReturnsVoid && !isPlainTaskReturnType)
                                {
                                    mockSetupStatements.Add(SyntaxFactory.ExpressionStatement(_frameworkSet.MockingFramework.GetSetupFor(dependencyMethod, mockFieldName, model.SemanticModel, _frameworkSet)));
                                }

                                var assertion = _frameworkSet.MockingFramework.GetAssertionFor(dependencyMethod, mockFieldName, model.SemanticModel, _frameworkSet);
                                if (isAsync && _frameworkSet.MockingFramework.AwaitAsyncAssertions)
                                {
                                    assertion = SyntaxFactory.AwaitExpression(assertion);
                                }

                                mockAssertionStatements.Add(SyntaxFactory.ExpressionStatement(assertion));
                            }
                        }

                        foreach (var dependencyProperty in dependencyMap.GetAccessedPropertySymbolsFor(field))
                        {
                            if (dependencyProperty.ContainingType == fieldType)
                            {
                                mockSetupStatements.Add(SyntaxFactory.ExpressionStatement(_frameworkSet.MockingFramework.GetSetupFor(dependencyProperty, mockFieldName, model.SemanticModel, _frameworkSet)));
                            }
                        }
                    }
                }
            }
        }
    }
}