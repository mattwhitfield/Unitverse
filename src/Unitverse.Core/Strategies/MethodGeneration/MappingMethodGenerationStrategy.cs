namespace Unitverse.Core.Strategies.MethodGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using Unitverse.Core.Resources;

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
                if (returnTypeMembers.ContainsKey(methodParameter.Name))
                {
                    return true;
                }

                if (methodParameter.TypeInfo.Type.SpecialType == SpecialType.None && !Equals(methodParameter.TypeInfo.Type, returnTypeInfo))
                {
                    var properties = GetProperties(methodParameter.TypeInfo.Type);
                    if (properties.Any(x => returnTypeMembers.ContainsKey(x.Key)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static Dictionary<string, bool> GetProperties(ITypeSymbol returnTypeInfo)
        {
            var properties = returnTypeInfo.GetMembers().Where(x => x.Kind == SymbolKind.Property).OfType<IPropertySymbol>().Where(x => !x.IsWriteOnly && !x.IsIndexer);

            var dictionary = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            foreach (var property in properties)
            {
                dictionary[property.Name] = property.Type.IsReferenceType;
            }

            return dictionary;
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

            var generatedMethod = _frameworkSet.TestFramework.CreateTestMethod(_frameworkSet.NamingProvider.PerformsMapping, namingContext, method.IsAsync, model.IsStatic);

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
                        SyntaxFactory.VariableDeclaration(AssignmentValueHelper.GetTypeOrImplicitType(parameter.TypeInfo.Type, _frameworkSet))
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
                if (returnTypeMembers.ContainsKey(methodParameter.Name))
                {
                    var returnTypeMember = returnTypeMembers.FirstOrDefault(x => string.Equals(x.Key, methodParameter.Name, StringComparison.OrdinalIgnoreCase));
                    var resultProperty = Generate.PropertyAccess(SyntaxFactory.IdentifierName(Strings.CanCallMethodGenerationStrategy_Create_result), returnTypeMember.Key);
                    generatedMethod = generatedMethod.AddBodyStatements(_frameworkSet.AssertionFramework.AssertEqual(resultProperty, SyntaxFactory.IdentifierName(methodParameter.Name), returnTypeMembers[methodParameter.Name]));
                    continue;
                }

                if (methodParameter.TypeInfo.Type.SpecialType == SpecialType.None && !Equals(methodParameter.TypeInfo.Type, returnTypeInfo))
                {
                    var properties = GetProperties(methodParameter.TypeInfo.Type);
                    foreach (var matchedSourceProperty in properties.Where(x => returnTypeMembers.ContainsKey(x.Key)))
                    {
                        var returnTypeMember = returnTypeMembers.FirstOrDefault(x => string.Equals(x.Key, matchedSourceProperty.Key, StringComparison.OrdinalIgnoreCase));
                        var resultProperty = Generate.PropertyAccess(SyntaxFactory.IdentifierName(Strings.CanCallMethodGenerationStrategy_Create_result), returnTypeMember.Key);
                        generatedMethod = generatedMethod.AddBodyStatements(_frameworkSet.AssertionFramework.AssertEqual(resultProperty,  Generate.PropertyAccess(SyntaxFactory.IdentifierName(methodParameter.Name), matchedSourceProperty.Key), matchedSourceProperty.Value));
                    }
                }
            }

            yield return generatedMethod;
        }
    }
}