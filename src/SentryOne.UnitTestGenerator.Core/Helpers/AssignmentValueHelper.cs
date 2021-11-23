namespace SentryOne.UnitTestGenerator.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Strategies.ValueGeneration;

    public static class AssignmentValueHelper
    {
        public static ExpressionSyntax GetDefaultAssignmentValue(TypeInfo propertyType, SemanticModel model, IFrameworkSet frameworkSet)
        {
            if (frameworkSet == null)
            {
                throw new ArgumentNullException(nameof(frameworkSet));
            }

            return GetDefaultAssignmentValue(propertyType.Type, model, new HashSet<string>(StringComparer.OrdinalIgnoreCase), frameworkSet);
        }

        public static ExpressionSyntax GetDefaultAssignmentValue(ITypeSymbol propertyType, SemanticModel model, IFrameworkSet frameworkSet)
        {
            return GetDefaultAssignmentValue(propertyType, model, new HashSet<string>(StringComparer.OrdinalIgnoreCase), frameworkSet);
        }

        public static ExpressionSyntax GetDefaultAssignmentValue(ITypeSymbol propertyType, SemanticModel model, HashSet<string> visitedTypes, IFrameworkSet frameworkSet)
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
                    return ValueGenerationStrategyFactory.GenerateFor("string", typeParameterSymbol, model, visitedTypes,  frameworkSet);
                }

                if (ValueGenerationStrategyFactory.IsSupported(propertyType))
                {
                    return ValueGenerationStrategyFactory.GenerateFor(propertyType, model, visitedTypes, frameworkSet);
                }

                if (propertyType.TypeKind == TypeKind.Interface)
                {
                    return frameworkSet.MockingFramework.GetThrowawayReference(propertyType.ToTypeSyntax(frameworkSet.Context));
                }

                if (propertyType is INamedTypeSymbol namedType && (propertyType.TypeKind == TypeKind.Class || propertyType.TypeKind == TypeKind.Struct))
                {
                    return GetClassDefaultAssignmentValue(model, visitedTypes, frameworkSet, namedType);
                }

                visitedTypes.Remove(fullName);
            }

            return SyntaxFactory.DefaultExpression(propertyType.ToTypeSyntax(frameworkSet.Context));
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
                    parameters.Add(GetDefaultAssignmentValue(parameter.Type, semanticModel, visitedTypesThisParameter, frameworkSet));
                }
            }

            return Generate.ObjectCreation(namedType.ToTypeSyntax(frameworkSet.Context), parameters.ToArray());
        }

        private static bool GetDerivedTypeInvocation(SemanticModel semanticModel, HashSet<string> visitedTypes, IFrameworkSet frameworkSet, INamedTypeSymbol namedType, out ExpressionSyntax expressionSyntax)
        {
            if (namedType.IsAbstract)
            {
                IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol @namespace)
                {
                    foreach (var type in @namespace.GetTypeMembers())
                    {
                        yield return type;
                    }

                    foreach (var nestedNamespace in @namespace.GetNamespaceMembers())
                    {
                        foreach (var type in GetAllTypes(nestedNamespace))
                        {
                            yield return type;
                        }
                    }
                }

                bool IsDerivedFrom(INamedTypeSymbol baseType, INamedTypeSymbol derivedType)
                {
                    var currentType = derivedType;
                    while (currentType != null)
                    {
                        if (currentType.Equals(baseType))
                        {
                            return true;
                        }

                        currentType = currentType.BaseType;
                    }

                    return false;
                }

                var children = GetAllTypes(namedType.ContainingAssembly.GlobalNamespace).Where(x => !x.IsAbstract && IsDerivedFrom(namedType, x)).ToList();
                if (children.Any())
                {
                    {
                        expressionSyntax = GetClassDefaultAssignmentValue(semanticModel, visitedTypes, frameworkSet, children.First());
                        return true;
                    }
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
                                return GetDefaultAssignmentValue(x.Type, semanticModel, visitedTypesThisMember, frameworkSet);
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
                            return Generate.Assignment(x.Name, GetDefaultAssignmentValue(x.Type, semanticModel, visitedTypesThisMember, frameworkSet));
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
