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
            if (propertyType == null)
            {
                throw new ArgumentNullException(nameof(propertyType));
            }

            if (frameworkSet == null)
            {
                throw new ArgumentNullException(nameof(frameworkSet));
            }

            return GetDefaultAssignmentValue(propertyType, model, new HashSet<string>(StringComparer.OrdinalIgnoreCase), frameworkSet);
        }

        private static ExpressionSyntax GetDefaultAssignmentValue(ITypeSymbol propertyType, SemanticModel model, HashSet<string> visitedTypes, IFrameworkSet frameworkSet)
        {
            frameworkSet.Context.AddEmittedType(propertyType);

            var fullName = propertyType.ToFullName();
            if (visitedTypes.Add(fullName))
            {
                if (propertyType is ITypeParameterSymbol typeParameterSymbol)
                {
                    return ValueGenerationStrategyFactory.GenerateFor("string", typeParameterSymbol, model, frameworkSet);
                }

                if (ValueGenerationStrategyFactory.IsSupported(propertyType))
                {
                    return ValueGenerationStrategyFactory.GenerateFor(propertyType, model, frameworkSet);
                }

                if (propertyType.TypeKind == TypeKind.Interface)
                {
                    return frameworkSet.MockingFramework.MockInterface(propertyType.ToTypeSyntax(frameworkSet.Context));
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

            if (constructor == null || constructor.IsImplicitlyDeclared || (!constructor.Parameters.Any() && constructor.DeclaredAccessibility == Accessibility.Public))
            {
                var initializableProperties = namedType.GetMembers().OfType<IPropertySymbol>().Where(x => x.DeclaredAccessibility == Accessibility.Public && !x.IsReadOnly && !x.IsStatic && x.SetMethod.DeclaredAccessibility != Accessibility.Private).ToList();
                var methods = namedType.GetMembers().OfType<IMethodSymbol>().Where(x => x.MethodKind == MethodKind.Ordinary);
                if (initializableProperties.Any() && !methods.Any())
                {
                    return Generate.ObjectCreation(namedType.ToTypeSyntax(frameworkSet.Context), initializableProperties.Select(x =>
                    {
                        var visitedTypesThisMember = new HashSet<string>(visitedTypes, StringComparer.OrdinalIgnoreCase);
                        return Generate.Assignment(x.Name, GetDefaultAssignmentValue(x.Type, semanticModel, visitedTypesThisMember, frameworkSet));
                    }));
                }

                return Generate.ObjectCreation(namedType.ToTypeSyntax(frameworkSet.Context));
            }

            if (constructor.DeclaredAccessibility != Accessibility.Public)
            {
                var factoryMethod = namedType.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x => x.ReturnType.Equals(namedType) && x.IsStatic && x.MethodKind == MethodKind.Ordinary);
                if (factoryMethod != null)
                {
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                namedType.ToTypeSyntax(frameworkSet.Context),
                                SyntaxFactory.IdentifierName(factoryMethod.Name)))
                        .WithArgumentList(Generate.Arguments(factoryMethod.Parameters.Select(x => GetDefaultAssignmentValue(x.Type, semanticModel, visitedTypes, frameworkSet)).ToArray()));
                }

                var instanceProperty = namedType.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(x => x.Type.Equals(namedType) && x.IsStatic);
                if (instanceProperty != null)
                {
                    return SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                namedType.ToTypeSyntax(frameworkSet.Context),
                                SyntaxFactory.IdentifierName(instanceProperty.Name));
                }
            }

            var parameters = new List<ExpressionSyntax>();

            foreach (var parameter in constructor.Parameters)
            {
                parameters.Add(GetDefaultAssignmentValue(parameter.Type, semanticModel, visitedTypes, frameworkSet));
            }

            return Generate.ObjectCreation(namedType.ToTypeSyntax(frameworkSet.Context), parameters.ToArray());
        }
    }
}
