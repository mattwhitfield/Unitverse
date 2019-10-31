namespace SentryOne.UnitTestGenerator.Core.Strategies.ClassGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Resources;

    public class AbstractClassGenerationStrategy : IClassGenerationStrategy
    {
        private readonly IFrameworkSet _frameworkSet;

        public AbstractClassGenerationStrategy(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        public int Priority => 1;

        public bool CanHandle(ClassModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return model.Declaration.Modifiers.Any(x => string.Equals(x.Text, "abstract", StringComparison.OrdinalIgnoreCase));
        }

        public ClassDeclarationSyntax Create(ClassModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var targetTypeName = _frameworkSet.GetTargetTypeName(model, true);
            var classDeclaration = SyntaxFactory.ClassDeclaration(targetTypeName);

            classDeclaration = classDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            if (!string.IsNullOrWhiteSpace(_frameworkSet.TestFramework.TestClassAttribute))
            {
                var testFixtureAtt = Generate.Attribute(_frameworkSet.TestFramework.TestClassAttribute);
                var list = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(testFixtureAtt));
                classDeclaration = classDeclaration.AddAttributeLists(list);
            }

            classDeclaration = classDeclaration.AddMembers(GenerateConcreteInheritor(model));

            var variableDeclaration = SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName("Test" + model.ClassName))
                .AddVariables(SyntaxFactory.VariableDeclarator("_testClass"));

            var fieldDeclaration = SyntaxFactory.FieldDeclaration(variableDeclaration)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
            classDeclaration = classDeclaration.AddMembers(fieldDeclaration);

            var setupMethod = Generate.SetupMethod(model, targetTypeName, _frameworkSet, ref classDeclaration);

            var identifierNameSyntax = SyntaxFactory.IdentifierName("Test" + model.ClassName);
            model.TypeSyntax = identifierNameSyntax;

            var creationExpression = model.GetObjectCreationExpression(_frameworkSet);

            var assignment = SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                model.TargetInstance,
                creationExpression);

            setupMethod = setupMethod.AddBodyStatements(SyntaxFactory.ExpressionStatement(assignment));

            classDeclaration = classDeclaration.AddMembers(setupMethod);

            return classDeclaration;
        }

        private static List<SyntaxNodeOrToken> ExtractTokenList(IList<ParameterModel> parameters)
        {
            var tokenList = new List<SyntaxNodeOrToken>();
            foreach (var parameter in parameters)
            {
                if (tokenList.Count > 0)
                {
                    tokenList.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
                }

                tokenList.Add(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parameter.Name)));
            }

            return tokenList;
        }

        private static SyntaxTokenList GetOverrideTokens(Accessibility accessibility)
        {
            var list = SyntaxFactory.TokenList();
            if (accessibility == Accessibility.Public)
            {
                list = list.Add(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            }

            if (accessibility == Accessibility.Protected)
            {
                list = list.Add(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));
            }

            if (accessibility == Accessibility.ProtectedAndInternal || accessibility == Accessibility.ProtectedAndFriend)
            {
                list = list.Add(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
                list = list.Add(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));
            }

            if (accessibility == Accessibility.ProtectedOrInternal || accessibility == Accessibility.ProtectedOrFriend)
            {
                list = list.Add(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));
                list = list.Add(SyntaxFactory.Token(SyntaxKind.InternalKeyword));
            }

            list = list.Add(SyntaxFactory.Token(SyntaxKind.OverrideKeyword));
            return list;
        }

        private static ClassDeclarationSyntax InheritConstructors(ClassModel model, string className, ClassDeclarationSyntax classDeclaration)
        {
            foreach (var modelConstructor in model.Constructors)
            {
                var tokenList = ExtractTokenList(modelConstructor.Parameters);

                var newConstructor = modelConstructor.Node
                    .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                    .WithIdentifier(SyntaxFactory.Identifier(className))
                    .WithBody(SyntaxFactory.Block())
                    .WithInitializer(
                        SyntaxFactory.ConstructorInitializer(
                            SyntaxKind.BaseConstructorInitializer,
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList<ArgumentSyntax>(tokenList))));

                classDeclaration = classDeclaration.AddMembers(newConstructor);
            }

            return classDeclaration;
        }

        private static ClassDeclarationSyntax InheritProtectedMethods(ClassModel model, ClassDeclarationSyntax classDeclaration)
        {
            foreach (var method in model.Methods)
            {
                if (!method.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.ProtectedKeyword)) || method.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.AbstractKeyword)))
                {
                    continue;
                }

                var tokenList = ExtractTokenList(method.Parameters);

                var baseInvocation = SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.BaseExpression(),
                            SyntaxFactory.IdentifierName(method.Name)))
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SeparatedList<ArgumentSyntax>(tokenList)));

                StatementSyntax methodStatement;
                if (method.IsVoid)
                {
                    methodStatement = SyntaxFactory.ExpressionStatement(baseInvocation);
                }
                else
                {
                    methodStatement = SyntaxFactory.ReturnStatement(baseInvocation);
                }

                var newConstructor = method.Node
                    .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                    .WithIdentifier(SyntaxFactory.Identifier("Public" + method.Name))
                    .WithBody(SyntaxFactory.Block(SyntaxFactory.SingletonList(methodStatement)));

                if (method.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.AbstractKeyword)))
                {
                    newConstructor = newConstructor.AddModifiers(SyntaxFactory.Token(SyntaxKind.OverrideKeyword));
                }

                method.MutateName("Public" + method.Name);

                classDeclaration = classDeclaration.AddMembers(newConstructor);
            }

            return classDeclaration;
        }

        private static bool IsAccessible(IMethodSymbol method)
        {
            if (method == null)
            {
                return false;
            }

            if (method.DeclaredAccessibility == Accessibility.Private)
            {
                return false;
            }

            return true;
        }

        private ClassDeclarationSyntax GenerateConcreteInheritor(ClassModel model)
        {
            var className = string.Format(CultureInfo.InvariantCulture, "Test{0}", model.ClassName);
            var classDeclaration = SyntaxFactory.ClassDeclaration(className)
                .WithBaseList(
                    SyntaxFactory.BaseList(
                        SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(
                            SyntaxFactory.SimpleBaseType(model.TypeSyntax))));

            classDeclaration = classDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));

            classDeclaration = InheritConstructors(model, className, classDeclaration);

            classDeclaration = InheritProtectedMethods(model, classDeclaration);

            classDeclaration = InheritProperties(model, classDeclaration);

            classDeclaration = ImplementAbstractMethods(model, classDeclaration);

            return classDeclaration;
        }

        private ClassDeclarationSyntax ImplementAbstractMethods(ClassModel model, ClassDeclarationSyntax classDeclaration)
        {
            var symbol = model.SemanticModel.GetDeclaredSymbol(model.Declaration);
            if (symbol == null)
            {
                return classDeclaration;
            }

            var methodCatalog = new Dictionary<string, IMethodSymbol>();
            var propertyCatalog = new Dictionary<string, IPropertySymbol>();

            var symbols = new Stack<INamedTypeSymbol>();
            while (symbol != null)
            {
                symbols.Push(symbol);
                symbol = symbol.BaseType;
            }

            bool IsInaccessible(ISymbol methodSymbol)
            {
                return methodSymbol.DeclaredAccessibility != Accessibility.Public && methodSymbol.DeclaredAccessibility != Accessibility.Protected && methodSymbol.DeclaredAccessibility != Accessibility.ProtectedAndInternal && methodSymbol.DeclaredAccessibility != Accessibility.ProtectedAndFriend && methodSymbol.DeclaredAccessibility != Accessibility.ProtectedOrFriend && methodSymbol.DeclaredAccessibility != Accessibility.ProtectedOrInternal;
            }

            while (symbols.Count > 0)
            {
                symbol = symbols.Pop();
                foreach (var method in symbol.GetMembers().OfType<IMethodSymbol>())
                {
                    if (method.MethodKind == MethodKind.PropertyGet || method.MethodKind == MethodKind.PropertySet)
                    {
                        continue;
                    }

                    if (IsInaccessible(method))
                    {
                        continue;
                    }

                    var methodSignature = method.Parameters.Select(x => x.Type.ToFullName()).Aggregate(method.Name, (x, y) => x + ":" + y) + method.TypeParameters.Select(x => x.Name).Aggregate(".", (x, y) => x + ":" + y);

                    if (method.IsAbstract)
                    {
                        methodCatalog[methodSignature] = method;
                    }
                    else if (method.IsOverride)
                    {
                        methodCatalog.Remove(methodSignature);
                    }
                }

                foreach (var property in symbol.GetMembers().OfType<IPropertySymbol>())
                {
                    if (IsInaccessible(property))
                    {
                        continue;
                    }

                    var propertySignature = property.Parameters.Select(x => x.Type.ToFullName()).Aggregate(property.Name, (x, y) => x + ":" + y);

                    if (property.IsAbstract)
                    {
                        propertyCatalog[propertySignature] = property;
                    }
                    else if (property.IsOverride)
                    {
                        propertyCatalog.Remove(propertySignature);
                    }
                }
            }

            foreach (var method in methodCatalog.Values)
            {
                var methodOverride = Generate.Method(method.Name, method.IsAsync, false)
                    .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(method.Parameters.Select(x => SyntaxFactory.Parameter(default(SyntaxList<AttributeListSyntax>), default(SyntaxTokenList), x.Type.ToTypeSyntax(_frameworkSet.Context), SyntaxFactory.Identifier(x.Name), null)))))
                    .WithModifiers(GetOverrideTokens(method.DeclaredAccessibility))
                    .WithIdentifier(SyntaxFactory.Identifier(method.Name))
                    .WithReturnType(method.ReturnType.ToTypeSyntax(_frameworkSet.Context));

                if (method.TypeParameters.Length > 0)
                {
                    methodOverride = methodOverride.WithTypeParameterList(SyntaxFactory.TypeParameterList(SyntaxFactory.SeparatedList(method.TypeParameters.Select(x => SyntaxFactory.TypeParameter(x.Name)))));
                }

                if (!string.Equals(method.ReturnType.ToFullName(), Strings.Generate_Method__void, StringComparison.OrdinalIgnoreCase))
                {
                    var methodStatement = SyntaxFactory.ReturnStatement(SyntaxFactory.DefaultExpression(method.ReturnType.ToTypeSyntax(_frameworkSet.Context)));
                    methodOverride = methodOverride.WithBody(SyntaxFactory.Block(methodStatement));
                }
                else
                {
                    methodOverride = methodOverride.WithBody(SyntaxFactory.Block());
                }

                classDeclaration = classDeclaration.AddMembers(methodOverride);
            }

            foreach (var property in propertyCatalog.Values)
            {
                var methodOverride = SyntaxFactory.PropertyDeclaration(property.Type.ToTypeSyntax(_frameworkSet.Context), property.Name)
                    .WithModifiers(GetOverrideTokens(property.DeclaredAccessibility));

                if (IsAccessible(property.GetMethod) && !IsAccessible(property.SetMethod))
                {
                    methodOverride = methodOverride.WithAccessorList(SyntaxFactory.AccessorList(new SyntaxList<AccessorDeclarationSyntax>(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))));
                }

                if (!IsAccessible(property.GetMethod) && IsAccessible(property.SetMethod))
                {
                    methodOverride = methodOverride.WithAccessorList(SyntaxFactory.AccessorList(new SyntaxList<AccessorDeclarationSyntax>(SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithBody(SyntaxFactory.Block()))));
                }

                if (IsAccessible(property.GetMethod) && IsAccessible(property.SetMethod))
                {
                    methodOverride = methodOverride.WithAccessorList(SyntaxFactory.AccessorList(new SyntaxList<AccessorDeclarationSyntax>(new[] { SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)), SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)) })));
                }

                classDeclaration = classDeclaration.AddMembers(methodOverride);
            }

            return classDeclaration;
        }

        private ClassDeclarationSyntax InheritProperties(ClassModel model, ClassDeclarationSyntax classDeclaration)
        {
            foreach (var property in model.Properties)
            {
                if (!property.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.ProtectedKeyword)))
                {
                    continue;
                }

                if (!property.HasGet && !property.HasSet)
                {
                    continue;
                }

                var propertyDeclaration = SyntaxFactory
                    .PropertyDeclaration(property.TypeInfo.ToTypeSyntax(_frameworkSet.Context), SyntaxFactory.Identifier("Public" + property.Name))
                    .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));

                ExpressionSyntax baseExpression;
                if (property.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.AbstractKeyword)))
                {
                    baseExpression = SyntaxFactory.ThisExpression();
                }
                else
                {
                    baseExpression = SyntaxFactory.BaseExpression();
                }

                var baseMember = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    baseExpression,
                    SyntaxFactory.IdentifierName(property.Name));

                if (!property.HasSet)
                {
                    propertyDeclaration = propertyDeclaration
                        .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(baseMember))
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
                }
                else if (!property.HasGet)
                {
                    var setAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithExpressionBody(
                            SyntaxFactory.ArrowExpressionClause(
                                SyntaxFactory.AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    baseMember,
                                    SyntaxFactory.IdentifierName(Strings.MsTestTestFramework_CreateTestCaseMethod_value))))
                        .WithSemicolonToken(
                            SyntaxFactory.Token(SyntaxKind.SemicolonToken));

                    propertyDeclaration = propertyDeclaration
                        .WithAccessorList(
                            SyntaxFactory.AccessorList(
                                SyntaxFactory.List(
                                    new[] { setAccessor })));
                }
                else
                {
                    var getAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(baseMember))
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

                    var setAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithExpressionBody(
                            SyntaxFactory.ArrowExpressionClause(
                                SyntaxFactory.AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    baseMember,
                                    SyntaxFactory.IdentifierName(Strings.MsTestTestFramework_CreateTestCaseMethod_value))))
                        .WithSemicolonToken(
                            SyntaxFactory.Token(SyntaxKind.SemicolonToken));

                    propertyDeclaration = propertyDeclaration
                        .WithAccessorList(
                            SyntaxFactory.AccessorList(
                                SyntaxFactory.List(
                                    new[] { getAccessor, setAccessor })));
                }

                property.MutateName("Public" + property.Name);

                classDeclaration = classDeclaration.AddMembers(propertyDeclaration);
            }

            return classDeclaration;
        }
    }
}