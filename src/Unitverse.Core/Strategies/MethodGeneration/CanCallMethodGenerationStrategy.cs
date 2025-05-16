namespace Unitverse.Core.Strategies.MethodGeneration
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

    public class CanCallMethodGenerationStrategy : IGenerationStrategy<IMethodModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public CanCallMethodGenerationStrategy(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        public bool IsExclusive => false;

        public int Priority => 1;

        public Func<IStrategyOptions, bool> IsEnabled => x => x.MethodCallChecksAreEnabled;

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

        public IEnumerable<SectionedMethodHandler> Create(IMethodModel method, ClassModel model, NamingContext namingContext)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var generatedMethod = _frameworkSet.CreateTestMethod(_frameworkSet.NamingProvider.CanCall, namingContext, method.IsAsync, model.IsStatic, "Checks that the " + method.Name + " method functions correctly.");

            if (method.Symbol == null)
            {
                yield break;
            }

            var interfaceMethodsImplemented = model.GetImplementedInterfaceSymbolsFor(method.Symbol);
            var testIsComplete = MockHelper.PrepareMockCalls(model, method.Node, null, interfaceMethodsImplemented, method.Parameters.Select(x => x.Name), _frameworkSet, out var mockSetupStatements, out var mockAssertionStatements);

            var paramExpressions = new List<CSharpSyntaxNode>();

            foreach (var parameter in method.Parameters)
            {
                if (parameter.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.OutKeyword)))
                {
                    paramExpressions.Add(SyntaxFactory.Argument(SyntaxFactory.DeclarationExpression(SyntaxFactory.IdentifierName("var"), SyntaxFactory.SingleVariableDesignation(parameter.Identifier))).WithRefKindKeyword(SyntaxFactory.Token(SyntaxKind.OutKeyword)));
                }
                else
                {
                    var defaultAssignmentValue = AssignmentValueHelper.GetDefaultAssignmentValue(parameter.TypeInfo, model.SemanticModel, _frameworkSet);

                    var paramName = parameter.Name;
                    var paramIdentifier = SyntaxFactory.IdentifierName(parameter.Identifier);
                    if (paramName == "fixture" && _frameworkSet.Options.GenerationOptions.UseAutoFixture)
                    {
                        paramName = "fixtureParam";
                        paramIdentifier = SyntaxFactory.IdentifierName(paramName);
                    }

                    if (parameter.TypeInfo.Type != null)
                    {
                        generatedMethod.Arrange(Generate.VariableDeclaration(parameter.TypeInfo.Type, _frameworkSet, paramName, defaultAssignmentValue));
                    }

                    if (parameter.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.RefKeyword)))
                    {
                        paramExpressions.Add(SyntaxFactory.Argument(paramIdentifier).WithRefKindKeyword(SyntaxFactory.Token(SyntaxKind.RefKeyword)));
                    }
                    else
                    {
                        paramExpressions.Add(paramIdentifier);
                    }
                }
            }

            generatedMethod.BlankLine();
            generatedMethod.Arrange(mockSetupStatements);

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
                bodyStatement = Generate.ImplicitlyTypedVariableDeclaration("result", methodCall);
            }
            else
            {
                bodyStatement = Generate.Statement(methodCall);
            }

            generatedMethod.Act(bodyStatement);

            generatedMethod.BlankLine();

            generatedMethod.Assert(mockAssertionStatements);

            if (!testIsComplete)
            {
                generatedMethod.BlankLine();
                generatedMethod.Assert(_frameworkSet.AssertionFramework.AssertFail(_frameworkSet.Options.GenerationOptions.DefaultFailureMessage));
            }

            yield return generatedMethod;
        }
    }
}