namespace Unitverse.Core.Strategies.MethodGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public class StringParameterCheckMethodGenerationStrategy : IGenerationStrategy<IMethodModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public StringParameterCheckMethodGenerationStrategy(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        public bool IsExclusive => false;

        public int Priority => 1;

        public Func<IStrategyOptions, bool> IsEnabled => x => x.MethodParameterChecksAreEnabled;

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

            return !method.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.AbstractKeyword)) && method.Parameters.Any(x => x.TypeInfo.Type != null && x.TypeInfo.Type.SpecialType == SpecialType.System_String);
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

            var shouldUseSeparatedNullableChecks = _frameworkSet.Options.GenerationOptions.ShouldUseSeparateChecksForNullAndEmpty(method.Node);

            for (var i = 0; i < method.Parameters.Count; i++)
            {
                ParameterModel currentParam = method.Parameters[i];
                if (currentParam.TypeInfo.Type == null || currentParam.TypeInfo.Type.SpecialType != SpecialType.System_String)
                {
                    continue;
                }

                if (currentParam.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.OutKeyword)))
                {
                    continue;
                }

                namingContext = namingContext.WithParameterName(currentParam.Name.ToPascalCase());

                var stringKeyword = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword));
                var isAsync = method.IsAsync && _frameworkSet.AssertionFramework.AssertThrowsAsyncIsAwaitable;
                var isNonNullable = currentParam.IsNullableTypeSyntax || currentParam.HasNullDefaultValue;

                if (shouldUseSeparatedNullableChecks)
                {
                    if (!isNonNullable)
                    {
                        var nullDescription = "Checks that the " + method.Name + " method throws when the " + currentParam.Name + " parameter is null.";
                        var generatedNullMethod = _frameworkSet.CreateTestMethod(_frameworkSet.NamingProvider.CannotCallWithNull, namingContext, isAsync, model.IsStatic, nullDescription);
                        AddMethodBody(method, model, i, currentParam, generatedNullMethod, "ArgumentNullException", false);
                        yield return generatedNullMethod;
                    }

                    var description = "Checks that the " + method.Name + " method throws when the " + currentParam.Name + " parameter is empty or white space.";
                    var generatedMethod = _frameworkSet.CreateTestCaseMethod(_frameworkSet.NamingProvider.CannotCallWithInvalid, namingContext, isAsync, model.IsStatic, stringKeyword, new object?[] { string.Empty, "   " }, description);
                    AddMethodBody(method, model, i, currentParam, generatedMethod, "ArgumentException", true);
                    yield return generatedMethod;
                }
                else
                {
                    var description = isNonNullable ?
                        "Checks that the " + method.Name + " method throws when the " + currentParam.Name + " parameter is empty or white space." :
                        "Checks that the " + method.Name + " method throws when the " + currentParam.Name + " parameter is null, empty or white space.";

                    object?[] testValues = isNonNullable ?
                        new object?[] { string.Empty, "   " } :
                        new object?[] { null, string.Empty, "   " };

                    var generatedMethod = _frameworkSet.CreateTestCaseMethod(_frameworkSet.NamingProvider.CannotCallWithInvalid, namingContext, isAsync, model.IsStatic, stringKeyword, testValues, description);
                    AddMethodBody(method, model, i, currentParam, generatedMethod, "ArgumentNullException", true);
                    yield return generatedMethod;
                }
            }
        }

        private void AddMethodBody(IMethodModel method, ClassModel model, int parameterIndex, ParameterModel currentParam, SectionedMethodHandler generatedMethod, string exceptionType, bool useValue)
        {
            var paramList = new List<CSharpSyntaxNode>();

            for (var index = 0; index < method.Parameters.Count; index++)
            {
                var parameter = method.Parameters[index];
                if (parameter.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.RefKeyword)))
                {
                    var defaultAssignmentValue = AssignmentValueHelper.GetDefaultAssignmentValue(parameter.TypeInfo, model.SemanticModel, _frameworkSet);

                    if (index == parameterIndex)
                    {
                        defaultAssignmentValue = SyntaxFactory.DefaultExpression(currentParam.TypeInfo.ToTypeSyntax(_frameworkSet.Context));
                    }

                    if (parameter.TypeInfo.Type != null)
                    {
                        generatedMethod.Emit(Generate.VariableDeclaration(parameter.TypeInfo.Type, _frameworkSet, parameter.Name, defaultAssignmentValue));
                    }

                    paramList.Add(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parameter.Name)).WithRefKindKeyword(SyntaxFactory.Token(SyntaxKind.RefKeyword)));
                }
                else if (parameter.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.OutKeyword)))
                {
                    paramList.Add(SyntaxFactory.Argument(SyntaxFactory.IdentifierName("_")).WithRefKindKeyword(SyntaxFactory.Token(SyntaxKind.OutKeyword)));
                }
                else
                {
                    if (index == parameterIndex)
                    {
                        if (useValue)
                        {
                            paramList.Add(SyntaxFactory.IdentifierName("value"));
                        }
                        else
                        {
                            paramList.Add(SyntaxFactory.DefaultExpression(currentParam.TypeInfo.ToTypeSyntax(_frameworkSet.Context)));
                        }
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
                generatedMethod.Emit(_frameworkSet.AssertionFramework.AssertThrowsAsync(SyntaxFactory.IdentifierName(exceptionType), methodCall, currentParam.Name));
            }
            else
            {
                generatedMethod.Emit(_frameworkSet.AssertionFramework.AssertThrows(SyntaxFactory.IdentifierName(exceptionType), methodCall, currentParam.Name));
            }
        }
    }
}