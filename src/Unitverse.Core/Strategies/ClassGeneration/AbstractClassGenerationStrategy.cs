namespace Unitverse.Core.Strategies.ClassGeneration
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

    public class AbstractClassGenerationStrategy : IClassGenerationStrategy
    {
        private IFrameworkSet _frameworkSet;

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

            if (model.Declaration.Modifiers.Any(x => string.Equals(x.Text, "abstract", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            if (_frameworkSet.Options.GenerationOptions.EmitSubclassForProtectedMethods)
            {
                return !model.Declaration.Modifiers.Any(m => m.IsKind(SyntaxKind.SealedKeyword)) && (model.Methods.Any(x => x.MarkedForGeneration && x.Node.Modifiers.Any(m => m.IsKind(SyntaxKind.ProtectedKeyword))) || model.Properties.Any(x => x.MarkedForGeneration && x.Node.Modifiers.Any(m => m.IsKind(SyntaxKind.ProtectedKeyword))));
            }

            return false;
        }

        public ClassDeclarationSyntax Create(ClassModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var targetTypeName = _frameworkSet.GetTargetTypeName(model);
            var classDeclaration = SyntaxFactory.ClassDeclaration(targetTypeName);

            classDeclaration = classDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            if (_frameworkSet.TestFramework.TestClassAttributes != null)
            {
                classDeclaration = classDeclaration.AddAttributeLists(_frameworkSet.TestFramework.TestClassAttributes.AsList());
            }

            classDeclaration = classDeclaration.AddMembers(GenerateConcreteInheritor(model));

            var variableDeclaration = SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName("Test" + model.ClassName))
                .AddVariables(SyntaxFactory.VariableDeclarator(model.TargetFieldName));

            var fieldDeclaration = SyntaxFactory.FieldDeclaration(variableDeclaration)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
            classDeclaration = classDeclaration.AddMembers(fieldDeclaration);

            if (_frameworkSet.Options.GenerationOptions.UseFieldForAutoFixture)
            {
                var autoFixtureFieldName = _frameworkSet.NamingProvider.AutoFixtureFieldName.Resolve(new NamingContext(model.ClassName));
                var variable = SyntaxFactory.VariableDeclaration(AutoFixtureHelper.TypeSyntax)
                            .AddVariables(SyntaxFactory.VariableDeclarator(autoFixtureFieldName));
                var field = SyntaxFactory.FieldDeclaration(variable)
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
                classDeclaration = classDeclaration.AddMembers(field);
            }

            var setupMethod = Generate.SetupMethod(model, targetTypeName, _frameworkSet, ref classDeclaration);

            var identifierNameSyntax = SyntaxFactory.IdentifierName("Test" + model.ClassName);
            model.TypeSyntax = identifierNameSyntax;

            var creationExpression = model.GetObjectCreationExpression(_frameworkSet, true);

            var assignment = SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                model.TargetInstance,
                creationExpression);

            setupMethod.Emit(Generate.Statement(assignment));

            classDeclaration = classDeclaration.AddMembers(setupMethod.Method);

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

                var argument = SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parameter.Node.Identifier));

                if (parameter.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.OutKeyword)))
                {
                    argument = argument.WithRefKindKeyword(SyntaxFactory.Token(SyntaxKind.OutKeyword));
                }
                else if (parameter.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.RefKeyword)))
                {
                    argument = argument.WithRefKindKeyword(SyntaxFactory.Token(SyntaxKind.RefKeyword));
                }

                tokenList.Add(argument);
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
                    .WithExpressionBody(null)
                    .WithInitializer(
                        SyntaxFactory.ConstructorInitializer(
                            SyntaxKind.BaseConstructorInitializer,
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList<ArgumentSyntax>(tokenList))));

                classDeclaration = classDeclaration.AddMembers(newConstructor);
            }

            return classDeclaration;
        }

        private ClassDeclarationSyntax InheritProtectedMethods(ClassModel model, ClassDeclarationSyntax classDeclaration)
        {
            foreach (var method in model.Methods)
            {
                if (!method.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.ProtectedKeyword)) || method.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.AbstractKeyword)))
                {
                    continue;
                }

                var tokenList = ExtractTokenList(method.Parameters);

                SimpleNameSyntax identifier = SyntaxFactory.IdentifierName(method.Name);

                if (method.Symbol != null && method.Symbol.TypeParameters.Length > 0)
                {
                    identifier = Generate.GenericName(method.Name, method.Symbol.TypeParameters.Select(x => SyntaxFactory.IdentifierName(x.Name)).ToArray());
                }

                InvocationExpressionSyntax baseInvocation;
                if (method.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.StaticKeyword)))
                {
                    baseInvocation = SyntaxFactory.InvocationExpression(identifier)
                                                  .WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList<ArgumentSyntax>(tokenList)));
                }
                else
                {
                    baseInvocation = Generate.MemberInvocation(SyntaxFactory.BaseExpression(), identifier)
                                             .WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList<ArgumentSyntax>(tokenList)));
                }

                StatementSyntax methodStatement;
                if (method.IsVoid)
                {
                    methodStatement = Generate.Statement(baseInvocation);
                }
                else
                {
                    methodStatement = SyntaxFactory.ReturnStatement(baseInvocation);
                }

                SyntaxTokenList modifiers;
                if (method.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.StaticKeyword)))
                {
                    modifiers = SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword));
                }
                else
                {
                    modifiers = SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                }

                var methodSymbol = method.Symbol;

                if (methodSymbol != null)
                {
                    var initialMethod = Generate.Method(method.Name, method.IsAsync, false)
                                                .WithIdentifier(SyntaxFactory.Identifier("Public" + method.Name))
                                                .WithModifiers(modifiers)
                                                .WithReturnType(methodSymbol.ReturnType.ToTypeSyntax(_frameworkSet.Context));

                    var methodHandler = new SectionedMethodHandler(initialMethod, _frameworkSet.Options.GenerationOptions);
                    _frameworkSet.Context.CurrentMethod = methodHandler;

                    var parameters = new List<ParameterSyntax>();

                    foreach (var param in methodSymbol.Parameters)
                    {
                        parameters.Add(GetParameter(param, model.SemanticModel, methodHandler));
                    }

                    methodHandler = new SectionedMethodHandler(initialMethod, _frameworkSet.Options.GenerationOptions);
                    methodHandler.Emit(methodStatement);

                    var newMethod = (MethodDeclarationSyntax)methodHandler.Method.WithParameterList(Generate.ParameterList(parameters));

                    if (methodSymbol.TypeParameters.Length > 0)
                    {
                        newMethod = newMethod.WithTypeParameterList(SyntaxFactory.TypeParameterList(SyntaxFactory.SeparatedList(methodSymbol.TypeParameters.Select(x => SyntaxFactory.TypeParameter(x.Name)))));
                    }

                    if (method.Node.Modifiers.Any(x => x.IsKind(SyntaxKind.AbstractKeyword)))
                    {
                        newMethod = newMethod.AddModifiers(SyntaxFactory.Token(SyntaxKind.OverrideKeyword));
                    }

                    method.MutateName("Public" + method.Name);

                    classDeclaration = classDeclaration.AddMembers(newMethod);
                }
            }

            return classDeclaration;
        }

        private static bool IsAccessible(IMethodSymbol? method)
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
            var originalFrameworkSet = _frameworkSet;

            // turn off using field for auto fixture while generating for abstracts
            var newOptions = _frameworkSet.Options.OverrideGenerationOption(x => x.UseFieldForAutoFixture = false);
            _frameworkSet = FrameworkSetFactory.Create(newOptions);

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

            _frameworkSet = originalFrameworkSet;

            return classDeclaration;
        }

        private ClassDeclarationSyntax ImplementAbstractMethods(ClassModel model, ClassDeclarationSyntax classDeclaration)
        {
            var symbol = model.TypeSymbol;
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
                var initialMethod = Generate.Method(method.Name, method.IsAsync, false)
                                            .WithModifiers(GetOverrideTokens(method.DeclaredAccessibility))
                                            .WithIdentifier(SyntaxFactory.Identifier(method.Name))
                                            .WithReturnType(method.ReturnType.ToTypeSyntax(_frameworkSet.Context));

                var methodHandler = new SectionedMethodHandler(initialMethod, _frameworkSet.Options.GenerationOptions);
                _frameworkSet.Context.CurrentMethod = methodHandler;

                var parameters = new List<ParameterSyntax>();

                foreach (var param in method.Parameters)
                {
                    parameters.Add(GetParameter(param, model.SemanticModel, methodHandler));
                }

                if (!string.Equals(method.ReturnType.ToFullName(), "void", StringComparison.OrdinalIgnoreCase))
                {
                    methodHandler.Emit(SyntaxFactory.ReturnStatement(SyntaxFactory.DefaultExpression(method.ReturnType.ToTypeSyntax(_frameworkSet.Context))));
                }

                var methodOverride = (MethodDeclarationSyntax)methodHandler.Method.WithParameterList(Generate.ParameterList(parameters));

                if (method.TypeParameters.Length > 0)
                {
                    methodOverride = methodOverride.WithTypeParameterList(SyntaxFactory.TypeParameterList(SyntaxFactory.SeparatedList(method.TypeParameters.Select(x => SyntaxFactory.TypeParameter(x.Name)))));
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

        private ParameterSyntax GetParameter(IParameterSymbol parameter, SemanticModel model, SectionedMethodHandler statements)
        {
            var syntax = Generate.Parameter(parameter.Name);
            if (parameter.RefKind == RefKind.Out)
            {
                syntax = syntax.WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.OutKeyword)));
                var defaultExpression = AssignmentValueHelper.GetDefaultAssignmentValue(parameter.Type, model, _frameworkSet);
                statements.Emit(Generate.Statement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName(parameter.Name), defaultExpression)));
            }
            else if (parameter.RefKind == RefKind.Ref)
            {
                syntax = syntax.WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.RefKeyword)));
            }
            else if (parameter.RefKind == RefKind.In)
            {
                syntax = syntax.WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.InKeyword)));
            }

            syntax = syntax.WithType(parameter.Type.ToTypeSyntax(_frameworkSet.Context));

            return syntax;
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

                var baseMember = Generate.MemberAccess(baseExpression, property.Name);

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
                                    SyntaxFactory.IdentifierName("value"))))
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
                                    SyntaxFactory.IdentifierName("value"))))
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