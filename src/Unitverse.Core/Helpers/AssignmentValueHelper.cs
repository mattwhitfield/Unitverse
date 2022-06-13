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
        public static TypeSyntax GetTypeOrImplicitType(ITypeSymbol type, IFrameworkSet frameworkSet)
        {
            if (type.TypeKind == TypeKind.Delegate)
            {
                return type.ToTypeSyntax(frameworkSet.Context);
            }

            return SyntaxFactory.IdentifierName("var");
        }

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

            if (frameworkSet.Options.GenerationOptions.UseAutoFixture)
            {
                // with AutoFixture, we do interfaces and delegates and let AutoFixture do the rest
                var expression =
                    GetInterfaceOrNull(propertyType, frameworkSet) ??
                    GetDelegateOrNull(propertyType, model, visitedTypes, frameworkSet);

                if (expression != null)
                {
                    return expression;
                }

                frameworkSet.Context.CurrentMethod?.AddRequirement(Requirements.AutoFixture);
                return SyntaxFactory.InvocationExpression(Generate.MemberAccess(
                    AutoFixtureHelper.VariableReference,
                    SyntaxFactory.GenericName(SyntaxFactory.Identifier("Create"))
                                                           .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(propertyType.ToTypeSyntax(frameworkSet.Context))))));
            }

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
                    return ValueGenerationStrategyFactory.GenerateFor("string", typeParameterSymbol, model, visitedTypes, frameworkSet);
                }

                var expression =
                    GetStrategyGeneratedValueOrNull(propertyType, model, visitedTypes, frameworkSet) ??
                    GetInterfaceOrNull(propertyType, frameworkSet) ??
                    GetClassStructOrNull(propertyType, model, visitedTypes, frameworkSet) ??
                    GetDelegateOrNull(propertyType, model, visitedTypes, frameworkSet);

                if (expression != null)
                {
                    return expression;
                }

                visitedTypes.Remove(fullName);
            }

            return SyntaxFactory.DefaultExpression(propertyType.ToTypeSyntax(frameworkSet.Context));
        }

        private static ExpressionSyntax GetStrategyGeneratedValueOrNull(ITypeSymbol propertyType, SemanticModel model, HashSet<string> visitedTypes, IFrameworkSet frameworkSet)
        {
            if (ValueGenerationStrategyFactory.IsSupported(propertyType))
            {
                frameworkSet.Context.ValuesGenerated++;
                return ValueGenerationStrategyFactory.GenerateFor(propertyType, model, visitedTypes, frameworkSet);
            }

            return null;
        }

        private static ExpressionSyntax GetDelegateOrNull(ITypeSymbol propertyType, SemanticModel model, HashSet<string> visitedTypes, IFrameworkSet frameworkSet)
        {
            if (propertyType is INamedTypeSymbol delegateType && (propertyType.TypeKind == TypeKind.Delegate))
            {
                var invokeMethod = delegateType.DelegateInvokeMethod;
                if (invokeMethod != null)
                {
                    return GetDefaultDelegateValue(model, visitedTypes, frameworkSet, invokeMethod);
                }
            }

            return null;
        }

        private static ExpressionSyntax GetClassStructOrNull(ITypeSymbol propertyType, SemanticModel model, HashSet<string> visitedTypes, IFrameworkSet frameworkSet)
        {
            // TODO - can we do record types here?
            if (propertyType is INamedTypeSymbol namedType && (propertyType.TypeKind == TypeKind.Class || propertyType.TypeKind == TypeKind.Struct))
            {
                frameworkSet.Context.TypesConstructed++;
                return GetClassDefaultAssignmentValue(model, visitedTypes, frameworkSet, namedType);
            }

            return null;
        }

        private static ExpressionSyntax GetInterfaceOrNull(ITypeSymbol propertyType, IFrameworkSet frameworkSet)
        {
            if (propertyType.TypeKind == TypeKind.Interface)
            {
                frameworkSet.Context.InterfacesMocked++;
                return frameworkSet.MockingFramework.GetThrowawayReference(propertyType.ToTypeSyntax(frameworkSet.Context));
            }

            return null;
        }

        private static ExpressionSyntax GetDefaultDelegateValue(SemanticModel model, HashSet<string> visitedTypes, IFrameworkSet frameworkSet, IMethodSymbol invokeMethod)
        {
            if (invokeMethod.Parameters.Length > 0 && invokeMethod.Parameters.Any(p => p.RefKind != RefKind.None))
            {
                return GetExplicitDelegateValue(model, visitedTypes, frameworkSet, invokeMethod);
            }

            var isVoid = invokeMethod.ReturnType.SpecialType == SpecialType.System_Void;
            IEnumerable<ParameterSyntax> GetParameters(int count)
            {
                var start = count < 4 ? 'x' : 'a';
                for (int i = 0; i < count; i++)
                {
                    var name = ((char)(start + i)).ToString();
                    yield return Generate.Parameter(name);
                }
            }

            var parameterCount = invokeMethod.Parameters.Length;
            LambdaExpressionSyntax lambda;
            if (parameterCount == 0)
            {
                lambda = SyntaxFactory.ParenthesizedLambdaExpression();
            }
            else if (parameterCount == 1)
            {
                lambda = SyntaxFactory.SimpleLambdaExpression(Generate.Parameter("x"));
            }
            else
            {
                var parameterList = Generate.ParameterList(GetParameters(parameterCount));
                lambda = SyntaxFactory.ParenthesizedLambdaExpression().WithParameterList(parameterList);
            }

            if (isVoid)
            {
                return lambda.WithBlock(SyntaxFactory.Block());
            }
            else
            {
                return lambda.WithExpressionBody(GetDefaultAssignmentValue(invokeMethod.ReturnType, model, visitedTypes, frameworkSet));
            }
        }

        private static ExpressionSyntax GetExplicitDelegateValue(SemanticModel model, HashSet<string> visitedTypes, IFrameworkSet frameworkSet, IMethodSymbol invokeMethod)
        {
            var isVoid = invokeMethod.ReturnType.SpecialType == SpecialType.System_Void;
            var setStatements = new List<StatementSyntax>();

            ParameterSyntax GetParameter(IParameterSymbol parameter, string name)
            {
                var syntax = Generate.Parameter(name);
                if (parameter.RefKind == RefKind.Out)
                {
                    syntax = syntax.WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.OutKeyword)));
                    setStatements.Add(SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName(name), GetDefaultAssignmentValue(parameter.Type, model, visitedTypes, frameworkSet))));
                }
                else if (parameter.RefKind == RefKind.Ref)
                {
                    syntax = syntax.WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.RefKeyword)));
                }
                else if (parameter.RefKind == RefKind.In)
                {
                    syntax = syntax.WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.InKeyword)));
                }

                syntax = syntax.WithType(parameter.Type.ToTypeSyntax(frameworkSet.Context));

                return syntax;
            }

            IEnumerable<ParameterSyntax> GetParameters(IList<IParameterSymbol> parameters)
            {
                var start = parameters.Count < 4 ? 'x' : 'a';
                for (int i = 0; i < parameters.Count; i++)
                {
                    var name = ((char)(start + i)).ToString();
                    yield return GetParameter(parameters[i], name);
                }
            }

            var parameterCount = invokeMethod.Parameters.Length;
            LambdaExpressionSyntax lambda;
            if (parameterCount == 0)
            {
                lambda = SyntaxFactory.ParenthesizedLambdaExpression();
            }
            else
            {
                var parameterList = Generate.ParameterList(GetParameters(invokeMethod.Parameters));
                lambda = SyntaxFactory.ParenthesizedLambdaExpression().WithParameterList(parameterList);
            }

            if (!isVoid && setStatements.Count == 0)
            {
                return lambda.WithExpressionBody(GetDefaultAssignmentValue(invokeMethod.ReturnType, model, visitedTypes, frameworkSet));
            }
            else
            {
                if (!isVoid)
                {
                    setStatements.Add(SyntaxFactory.ReturnStatement(GetDefaultAssignmentValue(invokeMethod.ReturnType, model, visitedTypes, frameworkSet)));
                }

                return lambda.WithBlock(SyntaxFactory.Block(setStatements.ToArray()));
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
                    parameters.Add(GetDefaultAssignmentValue(parameter.Type, semanticModel, visitedTypesThisParameter, frameworkSet));
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
