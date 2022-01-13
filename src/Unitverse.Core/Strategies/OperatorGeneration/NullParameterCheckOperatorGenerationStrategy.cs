namespace Unitverse.Core.Strategies.OperatorGeneration
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

    public class NullParameterCheckOperatorGenerationStrategy : IGenerationStrategy<IOperatorModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public NullParameterCheckOperatorGenerationStrategy(IFrameworkSet frameworkSet)
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

            return !method.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.AbstractKeyword)) && method.Parameters.Any(x => x.TypeInfo.Type.IsReferenceType);
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

            for (var i = 0; i < method.Parameters.Count; i++)
            {
                if (!method.Parameters[i].TypeInfo.Type.IsReferenceType)
                {
                    continue;
                }

                var paramList = new List<CSharpSyntaxNode>();

                namingContext = namingContext.WithParameterName(method.Parameters[i].Name.ToPascalCase());
                var generatedMethod = _frameworkSet.TestFramework.CreateTestMethod(_frameworkSet.NamingProvider.CannotCallOperatorWithNull, namingContext, false, model.IsStatic);

                for (var index = 0; index < method.Parameters.Count; index++)
                {
                    var parameter = method.Parameters[index];

                    if (index == i)
                    {
                        paramList.Add(SyntaxFactory.DefaultExpression(method.Parameters[i].TypeInfo.ToTypeSyntax(_frameworkSet.Context)));
                    }
                    else
                    {
                        paramList.Add(AssignmentValueHelper.GetDefaultAssignmentValue(parameter.TypeInfo, model.SemanticModel, _frameworkSet, true));
                    }
                }

                var methodCall = method.Invoke(model, true, _frameworkSet, paramList.ToArray());

                if (methodCall == null)
                {
                    continue;
                }

                var assignment = SyntaxFactory.ParenthesizedLambdaExpression(
                    SyntaxFactory.Block(
                        SyntaxFactory.SingletonList<StatementSyntax>(
                            SyntaxFactory.LocalDeclarationStatement(
                                SyntaxFactory.VariableDeclaration(
                                        SyntaxFactory.IdentifierName(Strings.Create_var))
                                    .WithVariables(
                                        SyntaxFactory.SingletonSeparatedList(
                                            SyntaxFactory.VariableDeclarator(
                                                    SyntaxFactory.Identifier(Strings.CanCallMethodGenerationStrategy_Create_result))
                                                .WithInitializer(
                                                    SyntaxFactory.EqualsValueClause(
                                                        methodCall))))))));

                generatedMethod = generatedMethod.AddBodyStatements(_frameworkSet.AssertionFramework.AssertThrows(SyntaxFactory.IdentifierName("ArgumentNullException"), assignment));

                yield return generatedMethod;
            }
        }
    }
}