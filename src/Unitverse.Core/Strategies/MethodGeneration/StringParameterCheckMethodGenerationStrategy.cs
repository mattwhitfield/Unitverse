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

    public class StringParameterCheckMethodGenerationStrategy : IGenerationStrategy<IMethodModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public StringParameterCheckMethodGenerationStrategy(IFrameworkSet frameworkSet)
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

            return !method.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.AbstractKeyword)) && method.Parameters.Any(x => x.TypeInfo.Type.SpecialType == SpecialType.System_String);
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

            for (var i = 0; i < method.Parameters.Count; i++)
            {
                if (method.Parameters[i].TypeInfo.Type.SpecialType != SpecialType.System_String)
                {
                    continue;
                }

                if (method.Parameters[i].Node.Modifiers.Any(x => x.Kind() == SyntaxKind.OutKeyword))
                {
                    continue;
                }

                var paramList = new List<CSharpSyntaxNode>();

                namingContext = namingContext.WithParameterName(method.Parameters[i].Name.ToPascalCase());

                var isNonNullable = method.Parameters[i].Node.Type is NullableTypeSyntax;
                object[] testValues = isNonNullable ? new object[] { string.Empty, "   " } : new object[] { null, string.Empty, "   " };

                var generatedMethod = _frameworkSet.TestFramework.CreateTestCaseMethod(_frameworkSet.NamingProvider.CannotCallWithInvalid, namingContext, method.IsAsync && _frameworkSet.AssertionFramework.AssertThrowsAsyncIsAwaitable, model.IsStatic, SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)), testValues);

                for (var index = 0; index < method.Parameters.Count; index++)
                {
                    var parameter = method.Parameters[index];
                    if (parameter.Node.Modifiers.Any(x => x.Kind() == SyntaxKind.RefKeyword))
                    {
                        var defaultAssignmentValue = AssignmentValueHelper.GetDefaultAssignmentValue(parameter.TypeInfo, model.SemanticModel, _frameworkSet);

                        if (index == i)
                        {
                            defaultAssignmentValue = SyntaxFactory.DefaultExpression(method.Parameters[i].TypeInfo.ToTypeSyntax(_frameworkSet.Context));
                        }

                        generatedMethod = generatedMethod.AddBodyStatements(SyntaxFactory.LocalDeclarationStatement(
                            SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName(Strings.Create_var))
                                .WithVariables(SyntaxFactory.SingletonSeparatedList(
                                    SyntaxFactory.VariableDeclarator(parameter.Identifier)
                                        .WithInitializer(SyntaxFactory.EqualsValueClause(defaultAssignmentValue))))));

                        paramList.Add(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parameter.Name)).WithRefKindKeyword(SyntaxFactory.Token(SyntaxKind.RefKeyword)));
                    }
                    else if (parameter.Node.Modifiers.Any(x => x.Kind() == SyntaxKind.OutKeyword))
                    {
                        paramList.Add(SyntaxFactory.Argument(SyntaxFactory.IdentifierName("_")).WithRefKindKeyword(SyntaxFactory.Token(SyntaxKind.OutKeyword)));
                    }
                    else
                    {
                        if (index == i)
                        {
                            paramList.Add(SyntaxFactory.IdentifierName(Strings.MsTestTestFramework_CreateTestCaseMethod_value));
                        }
                        else
                        {
                            paramList.Add(AssignmentValueHelper.GetDefaultAssignmentValue(parameter.TypeInfo, model.SemanticModel, _frameworkSet));
                        }
                    }
                }

                var methodCall = method.Invoke(model, true, _frameworkSet, paramList.ToArray());

                if (method.IsAsync)
                {
                    generatedMethod = generatedMethod.AddBodyStatements(_frameworkSet.AssertionFramework.AssertThrowsAsync(SyntaxFactory.IdentifierName("ArgumentNullException"), methodCall));
                }
                else
                {
                    generatedMethod = generatedMethod.AddBodyStatements(_frameworkSet.AssertionFramework.AssertThrows(SyntaxFactory.IdentifierName("ArgumentNullException"), methodCall));
                }

                yield return generatedMethod;
            }
        }
    }
}