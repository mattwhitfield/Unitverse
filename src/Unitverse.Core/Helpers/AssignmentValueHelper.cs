namespace Unitverse.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Strategies.ValueGeneration;

    public static class AssignmentValueHelper
    {
        public static ExpressionSyntax GetDefaultAssignmentValue(TypeInfo propertyType, SemanticModel model, IFrameworkSet frameworkSet, bool useExplicitTyping)
        {
            if (frameworkSet == null)
            {
                throw new ArgumentNullException(nameof(frameworkSet));
            }

            return GetDefaultAssignmentValue(propertyType.Type, model, new HashSet<string>(StringComparer.OrdinalIgnoreCase), frameworkSet, useExplicitTyping);
        }

        public static ExpressionSyntax GetDefaultAssignmentValue(ITypeSymbol propertyType, SemanticModel model, IFrameworkSet frameworkSet, bool useExplicitTyping)
        {
            return GetDefaultAssignmentValue(propertyType, model, new HashSet<string>(StringComparer.OrdinalIgnoreCase), frameworkSet, useExplicitTyping);
        }

        public static ExpressionSyntax GetDefaultAssignmentValue(ITypeSymbol propertyType, SemanticModel model, HashSet<string> visitedTypes, IFrameworkSet frameworkSet, bool useExplicitTyping)
        {
            if (propertyType == null)
            {
                throw new ArgumentNullException(nameof(propertyType));
            }

            if (visitedTypes == null)
            {
                throw new ArgumentNullException(nameof(visitedTypes));
            }

            if (frameworkSet == null)
            {
                throw new ArgumentNullException(nameof(frameworkSet));
            }

            frameworkSet.Context.AddEmittedType(propertyType);

            var fullName = propertyType.ToFullName();
            if (visitedTypes.Add(fullName))
            {
                if (propertyType is ITypeParameterSymbol typeParameterSymbol)
                {
                    frameworkSet.Context.GenericTypes.TryGetValue(typeParameterSymbol.Name, out var derivedType);
                    if (derivedType is INamedTypeSymbol namedTypeSymbol)
                    {
                        frameworkSet.Context.TypesConstructed++;
                        return GetClassDefaultAssignmentValue(model, visitedTypes, frameworkSet, namedTypeSymbol);
                    }

                    frameworkSet.Context.ValuesGenerated++;
                    return ValueGenerationStrategyFactory.GenerateFor("string", typeParameterSymbol, model, visitedTypes,  frameworkSet);
                }

                if (ValueGenerationStrategyFactory.IsSupported(propertyType))
                {
                    frameworkSet.Context.ValuesGenerated++;
                    return ValueGenerationStrategyFactory.GenerateFor(propertyType, model, visitedTypes, frameworkSet);
                }

                if (propertyType.TypeKind == TypeKind.Interface)
                {
                    frameworkSet.Context.InterfacesMocked++;
                    return frameworkSet.MockingFramework.GetThrowawayReference(propertyType.ToTypeSyntax(frameworkSet.Context));
                }

                if (propertyType is INamedTypeSymbol namedType && (propertyType.TypeKind == TypeKind.Class || propertyType.TypeKind == TypeKind.Struct))
                {
                    frameworkSet.Context.TypesConstructed++;
                    return GetClassDefaultAssignmentValue(model, visitedTypes, frameworkSet, namedType);
                }

                if (propertyType is INamedTypeSymbol delegateType && (propertyType.TypeKind == TypeKind.Delegate))
                {
                    var invokeMethod = delegateType.DelegateInvokeMethod;
                    if (invokeMethod != null)
                    {
                        var delegateSyntax = GetDefaultDelegateValue(model, visitedTypes, frameworkSet, invokeMethod);
                        if (useExplicitTyping)
                        {
                            return SyntaxFactory.CastExpression(delegateType.ToTypeSyntax(frameworkSet.Context), SyntaxFactory.ParenthesizedExpression(delegateSyntax));
                        }

                        return delegateSyntax;
                    }
                }

                visitedTypes.Remove(fullName);
            }

            return SyntaxFactory.DefaultExpression(propertyType.ToTypeSyntax(frameworkSet.Context));
        }

        private static ExpressionSyntax GetDefaultDelegateValue(SemanticModel model, HashSet<string> visitedTypes, IFrameworkSet frameworkSet, IMethodSymbol invokeMethod)
        {
            var isVoid = invokeMethod.ReturnType.SpecialType == SpecialType.System_Void;

            var parameterCount = invokeMethod.Parameters.Length;
            LambdaExpressionSyntax lambda;
            if (parameterCount == 0)
            {
                lambda = SyntaxFactory.ParenthesizedLambdaExpression();
            }
            else if (parameterCount == 1)
            {
                lambda = SyntaxFactory.SimpleLambdaExpression(SyntaxFactory.Parameter(SyntaxFactory.Identifier("x")));
            }
            else
            {
                var start = parameterCount < 4 ? 'x' : 'a';
                var parameterList = Generate.ParameterList(Enumerable.Range(0, parameterCount).Select(x => ((char)(start + x)).ToString()));
                lambda = SyntaxFactory.ParenthesizedLambdaExpression().WithParameterList(parameterList);
            }

            if (isVoid)
            {
                return lambda.WithBlock(SyntaxFactory.Block());
            }
            else
            {
                return lambda.WithExpressionBody(GetDefaultAssignmentValue(invokeMethod.ReturnType, model, visitedTypes, frameworkSet, false));
            }

        }

        private static ExpressionSyntax GetClassDefaultAssignmentValue(SemanticModel semanticModel, HashSet<string> visitedTypes, IFrameworkSet frameworkSet, INamedTypeSymbol namedType)
        {
            var constructor = namedType.Constructors.Where(x => !x.IsStatic && x.DeclaredAccessibility == Accessibility.Public).OrderBy(x => x.Parameters.Length).FirstOrDefault() ??
                              namedType.Constructors.Where(x => !x.IsStatic).OrderBy(x => x.Parameters.Length).FirstOrDefault();

            if (GetImplicitConstructorInvocation(semanticModel, visitedTypes, frameworkSet, namedType, constructor, out var implicitConstructorInvocation))
            {
                return implicitConstructorInvocation;
            }

            if (GetFactoryMethodInvocation(semanticModel, visitedTypes, frameworkSet, namedType, constructor, out var factoryMethodInvocation))
            {
                return factoryMethodInvocation;
            }

            if (GetDerivedTypeInvocation(semanticModel, visitedTypes, frameworkSet, namedType, out var derivedTypeInvocation))
            {
                return derivedTypeInvocation;
            }

            return GetStandardConstructorInvocation(semanticModel, visitedTypes, frameworkSet, namedType, constructor);
        }

        private static ExpressionSyntax GetStandardConstructorInvocation(SemanticModel semanticModel, HashSet<string> visitedTypes, IFrameworkSet frameworkSet, INamedTypeSymbol namedType, IMethodSymbol constructor)
        {
            var parameters = new List<ExpressionSyntax>();
            if (constructor != null)
            {
                foreach (var parameter in constructor.Parameters)
                {
                    var visitedTypesThisParameter = new HashSet<string>(visitedTypes, StringComparer.OrdinalIgnoreCase);
                    parameters.Add(GetDefaultAssignmentValue(parameter.Type, semanticModel, visitedTypesThisParameter, frameworkSet, false));
                }
            }

            return Generate.ObjectCreation(namedType.ToTypeSyntax(frameworkSet.Context), parameters.ToArray());
        }

        private static bool GetDerivedTypeInvocation(SemanticModel semanticModel, HashSet<string> visitedTypes, IFrameworkSet frameworkSet, INamedTypeSymbol namedType, out ExpressionSyntax expressionSyntax)
        {
            if (namedType.IsAbstract)
            {
                var derivedType = TypeHelper.FindDerivedNonAbstractType(namedType);
                if (derivedType != null)
                {
                    expressionSyntax = GetClassDefaultAssignmentValue(semanticModel, visitedTypes, frameworkSet, derivedType);
                    return true;
                }
            }

            expressionSyntax = null;
            return false;
        }

        private static bool GetFactoryMethodInvocation(SemanticModel semanticModel, HashSet<string> visitedTypes, IFrameworkSet frameworkSet, INamedTypeSymbol namedType, IMethodSymbol constructor, out ExpressionSyntax memberAccessExpression)
        {
            if (constructor.DeclaredAccessibility != Accessibility.Public)
            {
                var factoryMethod = namedType.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x => x.ReturnType.Equals(namedType) && x.IsStatic && x.MethodKind == MethodKind.Ordinary);
                if (factoryMethod != null)
                {
                    {
                        memberAccessExpression = SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    namedType.ToTypeSyntax(frameworkSet.Context),
                                    SyntaxFactory.IdentifierName(factoryMethod.Name)))
                            .WithArgumentList(Generate.Arguments(factoryMethod.Parameters.Select(x =>
                            {
                                var visitedTypesThisMember = new HashSet<string>(visitedTypes, StringComparer.OrdinalIgnoreCase);
                                return GetDefaultAssignmentValue(x.Type, semanticModel, visitedTypesThisMember, frameworkSet, false);
                            }).OfType<CSharpSyntaxNode>().ToArray()));
                        return true;
                    }
                }

                var instanceProperty = namedType.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x => x.Type.Equals(namedType) && x.IsStatic);
                if (instanceProperty != null)
                {
                    {
                        memberAccessExpression = SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            namedType.ToTypeSyntax(frameworkSet.Context),
                            SyntaxFactory.IdentifierName(instanceProperty.Name));
                        return true;
                    }
                }
            }

            memberAccessExpression = null;
            return false;
        }

        private static bool GetImplicitConstructorInvocation(SemanticModel semanticModel, HashSet<string> visitedTypes, IFrameworkSet frameworkSet, INamedTypeSymbol namedType, IMethodSymbol constructor, out ExpressionSyntax expressionSyntax)
        {
            if (constructor == null || constructor.IsImplicitlyDeclared || (!constructor.Parameters.Any() && constructor.DeclaredAccessibility == Accessibility.Public))
            {
                var initializableProperties = namedType.GetMembers().OfType<IPropertySymbol>().Where(x => x.DeclaredAccessibility == Accessibility.Public && !x.IsReadOnly && !x.IsStatic && x.SetMethod.DeclaredAccessibility != Accessibility.Private).ToList();
                var methods = namedType.GetMembers().OfType<IMethodSymbol>().Where(x => x.MethodKind == MethodKind.Ordinary);
                if (initializableProperties.Any() && !methods.Any())
                {
                    {
                        expressionSyntax = Generate.ObjectCreation(namedType.ToTypeSyntax(frameworkSet.Context), initializableProperties.Select(x =>
                        {
                            var visitedTypesThisMember = new HashSet<string>(visitedTypes, StringComparer.OrdinalIgnoreCase);
                            return Generate.Assignment(x.Name, GetDefaultAssignmentValue(x.Type, semanticModel, visitedTypesThisMember, frameworkSet, false));
                        }));
                        return true;
                    }
                }

                expressionSyntax = Generate.ObjectCreation(namedType.ToTypeSyntax(frameworkSet.Context));
                return true;
            }

            expressionSyntax = null;
            return false;
        }
    }
}
