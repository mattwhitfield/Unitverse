namespace Unitverse.Core.Strategies.MethodGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public class NullParameterCheckMethodGenerationStrategy : IGenerationStrategy<IMethodModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public NullParameterCheckMethodGenerationStrategy(IFrameworkSet frameworkSet)
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

            return !method.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.AbstractKeyword)) && method.Parameters.Any(x => x.TypeInfo.Type != null && x.TypeInfo.Type.IsReferenceType && x.TypeInfo.Type.SpecialType != SpecialType.System_String);
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

            for (var i = 0; i < method.Parameters.Count; i++)
            {
                ParameterModel currentParam = method.Parameters[i];
                if (currentParam.TypeInfo.Type == null || !currentParam.TypeInfo.Type.IsReferenceType)
                {
                    continue;
                }

                if (currentParam.TypeInfo.Type.SpecialType == SpecialType.System_String)
                {
                    continue;
                }

                if (currentParam.Node.Modifiers.Any(x => x.Kind() == SyntaxKind.OutKeyword))
                {
                    continue;
                }

                if (currentParam.IsNullableTypeSyntax || currentParam.HasNullDefaultValue)
                {
                    continue;
                }

                var paramList = new List<CSharpSyntaxNode>();

                namingContext = namingContext.WithParameterName(currentParam.Name.ToPascalCase());
                var generatedMethod = _frameworkSet.CreateTestMethod(_frameworkSet.NamingProvider.CannotCallWithNull, namingContext, method.IsAsync && _frameworkSet.AssertionFramework.AssertThrowsAsyncIsAwaitable, model.IsStatic, "Checks that the " + method.Name + " method throws when the " + currentParam.Name + " parameter is null.");

                for (var index = 0; index < method.Parameters.Count; index++)
                {
                    var parameter = method.Parameters[index];
                    if (parameter.Node.Modifiers.Any(x => x.Kind() == SyntaxKind.RefKeyword))
                    {
                        var defaultAssignmentValue = AssignmentValueHelper.GetDefaultAssignmentValue(parameter.TypeInfo, model.SemanticModel, _frameworkSet);

                        if (index == i)
                        {
                            defaultAssignmentValue = SyntaxFactory.DefaultExpression(currentParam.TypeInfo.ToTypeSyntax(_frameworkSet.Context));
                        }

                        if (parameter.TypeInfo.Type != null)
                        {
                            generatedMethod.Emit(Generate.VariableDeclaration(parameter.TypeInfo.Type, _frameworkSet, parameter.Name, defaultAssignmentValue));
                        }

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
                            paramList.Add(SyntaxFactory.DefaultExpression(currentParam.TypeInfo.ToTypeSyntax(_frameworkSet.Context)));
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
                    generatedMethod.Emit(_frameworkSet.AssertionFramework.AssertThrowsAsync(SyntaxFactory.IdentifierName("ArgumentNullException"), methodCall));
                }
                else
                {
                    generatedMethod.Emit(_frameworkSet.AssertionFramework.AssertThrows(SyntaxFactory.IdentifierName("ArgumentNullException"), methodCall));
                }

                yield return generatedMethod;
            }
        }
    }
}