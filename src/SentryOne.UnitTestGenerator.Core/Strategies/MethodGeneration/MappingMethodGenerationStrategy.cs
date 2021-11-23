namespace SentryOne.UnitTestGenerator.Core.Strategies.MethodGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Resources;

    public class MappingMethodGenerationStrategy : IGenerationStrategy<IMethodModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public MappingMethodGenerationStrategy(IFrameworkSet frameworkSet)
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

            if (method.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.AbstractKeyword)))
            {
                return false;
            }

            var returnTypeInfo = model.SemanticModel.GetTypeInfo(method.Node.ReturnType).Type;
            if (returnTypeInfo == null || returnTypeInfo.SpecialType != SpecialType.None || method.Node.IsKind(SyntaxKind.IndexerDeclaration) || (returnTypeInfo.ToFullName() == typeof(Task).FullName && !(returnTypeInfo is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.IsGenericType)))
            {
                return false;
            }

            if (returnTypeInfo is INamedTypeSymbol namedType && namedType.IsGenericType && returnTypeInfo.ToFullName() == typeof(Task).FullName)
            {
                returnTypeInfo = namedType.TypeArguments[0];
            }

            var returnTypeMembers = GetProperties(returnTypeInfo);

            foreach (var methodParameter in method.Parameters)
            {
                if (returnTypeMembers.Contains(methodParameter.Name))
                {
                    return true;
                }

                if (methodParameter.TypeInfo.Type.SpecialType == SpecialType.None && !Equals(methodParameter.TypeInfo.Type, returnTypeInfo))
                {
                    var properties = GetProperties(methodParameter.TypeInfo.Type);
                    if (properties.Any(x => returnTypeMembers.Contains(x)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static HashSet<string> GetProperties(ITypeSymbol returnTypeInfo)
        {
            return new HashSet<string>(returnTypeInfo.GetMembers().Where(x => x.Kind == SymbolKind.Property).OfType<IPropertySymbol>().Where(x => !x.IsWriteOnly && !x.IsIndexer).Select(x => x.Name), StringComparer.OrdinalIgnoreCase);
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

            var methodName = string.Format(CultureInfo.InvariantCulture, "{0}PerformsMapping", model.GetMethodUniqueName(method));

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
                        paramExpressions.Add(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parameter.Name)).WithRefKindKeyword(SyntaxFactory.Token(SyntaxKind.RefKeyword)));
                    }
                    else
                    {
                        paramExpressions.Add(SyntaxFactory.IdentifierName(parameter.Name));
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

            var returnTypeInfo = model.SemanticModel.GetTypeInfo(method.Node.ReturnType).Type;
            if (returnTypeInfo == null || returnTypeInfo.SpecialType != SpecialType.None || method.Node.IsKind(SyntaxKind.IndexerDeclaration) || (returnTypeInfo.ToFullName() == typeof(Task).FullName && !(returnTypeInfo is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.IsGenericType)))
            {
                yield break;
            }

            if (returnTypeInfo is INamedTypeSymbol namedType && namedType.IsGenericType && returnTypeInfo.ToFullName() == typeof(Task).FullName)
            {
                returnTypeInfo = namedType.TypeArguments[0];
            }

            var returnTypeMembers = GetProperties(returnTypeInfo);

            foreach (var methodParameter in method.Parameters)
            {
                if (returnTypeMembers.Contains(methodParameter.Name))
                {
                    var returnTypeMember = returnTypeMembers.FirstOrDefault(x => string.Equals(x, methodParameter.Name, StringComparison.OrdinalIgnoreCase));
                    var resultProperty = Generate.PropertyAccess(SyntaxFactory.IdentifierName(Strings.CanCallMethodGenerationStrategy_Create_result), returnTypeMember);
                    generatedMethod = generatedMethod.AddBodyStatements(_frameworkSet.TestFramework.AssertEqual(resultProperty, SyntaxFactory.IdentifierName(methodParameter.Name)));
                    continue;
                }

                if (methodParameter.TypeInfo.Type.SpecialType == SpecialType.None && !Equals(methodParameter.TypeInfo.Type, returnTypeInfo))
                {
                    var properties = GetProperties(methodParameter.TypeInfo.Type);
                    foreach (var matchedSourceProperty in properties.Where(x => returnTypeMembers.Contains(x)))
                    {
                        var returnTypeMember = returnTypeMembers.FirstOrDefault(x => string.Equals(x, matchedSourceProperty, StringComparison.OrdinalIgnoreCase));
                        var resultProperty = Generate.PropertyAccess(SyntaxFactory.IdentifierName(Strings.CanCallMethodGenerationStrategy_Create_result), returnTypeMember);
                        generatedMethod = generatedMethod.AddBodyStatements(_frameworkSet.TestFramework.AssertEqual(resultProperty,  Generate.PropertyAccess(SyntaxFactory.IdentifierName(methodParameter.Name), matchedSourceProperty)));
                    }
                }
            }

            yield return generatedMethod;
        }
    }
}