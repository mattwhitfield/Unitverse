namespace Unitverse.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using Unitverse.Core.Resources;

    public static class Generate
    {
        public static MethodDeclarationSyntax Method(string name, bool isAsync, bool isStatic)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), name).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            if (isAsync)
            {
                method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("Task"), name).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

                if (isStatic)
                {
                    method = method.AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
                }

                method = method.AddModifiers(SyntaxFactory.Token(SyntaxKind.AsyncKeyword));
            }
            else if (isStatic)
            {
                method = method.AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
            }

            return method;
        }

        public static string CleanName(string sourceName)
        {
            if (string.IsNullOrWhiteSpace(sourceName))
            {
                return sourceName;
            }

            var first = true;
            var builder = new StringBuilder(sourceName.Length);
            foreach (var ch in sourceName)
            {
                bool valid;
                if (first)
                {
                    valid = char.IsLetter(ch) || ch == '_' || ch == '@';
                    first = false;
                }
                else
                {
                    valid = char.IsLetterOrDigit(ch) || ch == '_' || ch == '@';
                }

                if (valid)
                {
                    builder.Append(ch);
                }
                else
                {
                    builder.Append('_');
                }
            }

            return builder.ToString();
        }

        public static UsingDirectiveSyntax UsingDirective(string name)
        {
            return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(name));
        }

        public static SimpleNameSyntax GenericName(string identifier, ITypeSymbol typeSymbol, IGenerationContext context)
        {
            return SyntaxFactory.GenericName(SyntaxFactory.Identifier(identifier))
                                .WithTypeArgumentList(typeSymbol.ToTypeSyntax(context).AsList());
        }

        public static SimpleNameSyntax GenericName(string identifier, params TypeSyntax[] arguments)
        {
            return SyntaxFactory.GenericName(SyntaxFactory.Identifier(identifier))
                                .WithTypeArgumentList(arguments.AsList());
        }

        public static StatementSyntax Statement(ExpressionSyntax expression)
        {
            return SyntaxFactory.ExpressionStatement(expression);
        }

        public static SimpleNameSyntax GenericName(string identifier, params SyntaxKind[] arguments)
        {
            return GenericName(identifier, arguments.Select(x => (TypeSyntax)SyntaxFactory.PredefinedType(SyntaxFactory.Token(x))).ToArray());
        }

        public static InvocationExpressionSyntax MemberInvocation(ExpressionSyntax container, string member, params ExpressionSyntax[] arguments)
        {
            return MemberInvocation(container, SyntaxFactory.IdentifierName(member), arguments);
        }

        public static InvocationExpressionSyntax MemberInvocation(string container, string member, params ExpressionSyntax[] arguments)
        {
            return MemberInvocation(SyntaxFactory.IdentifierName(container), SyntaxFactory.IdentifierName(member), arguments);
        }

        public static InvocationExpressionSyntax MemberInvocation(string container, SimpleNameSyntax member, params ExpressionSyntax[] arguments)
        {
            return MemberInvocation(SyntaxFactory.IdentifierName(container), member, arguments);
        }

        public static InvocationExpressionSyntax MemberInvocation(ExpressionSyntax container, SimpleNameSyntax member, params ExpressionSyntax[] arguments)
        {
            return SyntaxFactory.InvocationExpression(MemberAccess(container, member)).WithArgs(arguments);
        }

        public static InvocationExpressionSyntax WithArgs(this InvocationExpressionSyntax syntax, params ExpressionSyntax[] arguments)
        {
            return syntax.WithArgumentList(Arguments(arguments));
        }

        public static MemberAccessExpressionSyntax MemberAccess(string container, string member)
        {
            return MemberAccess(SyntaxFactory.IdentifierName(container), SyntaxFactory.IdentifierName(member));
        }

        public static MemberAccessExpressionSyntax MemberAccess(ExpressionSyntax container, string member)
        {
            return MemberAccess(container, SyntaxFactory.IdentifierName(member));
        }

        public static MemberAccessExpressionSyntax MemberAccess(string container, SimpleNameSyntax member)
        {
            return MemberAccess(SyntaxFactory.IdentifierName(container), member);
        }

        public static MemberAccessExpressionSyntax MemberAccess(ExpressionSyntax container, SimpleNameSyntax member)
        {
            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, container, member);
        }

        public static LiteralExpressionSyntax Literal(object? literal)
        {
            if (literal is string s)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(s));
            }

            if (literal is bool boo)
            {
                return boo ? SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression) : SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression);
            }

            if (literal is short sh)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(sh));
            }

            if (literal is int i)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(i));
            }

            if (literal is long l)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(l));
            }

            if (literal is byte b)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(b));
            }

            if (literal is decimal d)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(d));
            }

            if (literal is double db)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(db));
            }

            if (literal is float f)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(f));
            }

            if (literal is char c)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.CharacterLiteralExpression, SyntaxFactory.Literal(c));
            }

            if (literal is null)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            }

            throw new InvalidOperationException(Strings.Generate_Literal_Can_only_emit_literals_for_strings_and_numeric_expressions);
        }

        public static ExpressionSyntax MethodCall(
            ExpressionSyntax target,
            MethodDeclarationSyntax method,
            string name,
            IFrameworkSet frameworkSet,
            params CSharpSyntaxNode[] arguments)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (frameworkSet == null)
            {
                throw new ArgumentNullException(nameof(frameworkSet));
            }

            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (method.ParameterList.Parameters.Count > 0 && method.ParameterList.Parameters[0].Modifiers.Any(x => x.Kind() == SyntaxKind.ThisKeyword) && arguments.Length > 0 && arguments[0] is ExpressionSyntax expression)
            {
                target = expression;
                arguments = arguments.Skip(1).ToArray();
            }

            SimpleNameSyntax methodReference;
            if (method.TypeParameterList?.Parameters.Any() ?? false)
            {
                foreach (var parameter in method.TypeParameterList.Parameters)
                {
                    frameworkSet.Context.AddVisitedGenericType(parameter.Identifier.ValueText);
                }

                methodReference = GenericName(name, method.TypeParameterList.Parameters.Select(x => SyntaxFactory.IdentifierName(x.Identifier)).ToArray());
            }
            else
            {
                methodReference = SyntaxFactory.IdentifierName(name);
            }

            return SyntaxFactory.InvocationExpression(MemberAccess(target, methodReference)).WithArgumentList(Arguments(arguments));
        }

        public static TypeArgumentListSyntax AsList(this IEnumerable<TypeSyntax> typeSyntaxEnumerable)
        {
            return SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(typeSyntaxEnumerable));
        }

        public static TypeArgumentListSyntax AsList(this TypeSyntax typeSyntax)
        {
            return SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(typeSyntax));
        }

        public static AttributeListSyntax AsList(this AttributeSyntax attribute)
        {
            return SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(attribute));
        }

        public static AttributeSyntax Attribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var nameSyntax = SyntaxFactory.ParseName(name);
            return SyntaxFactory.Attribute(nameSyntax);
        }

        public static AttributeSyntax Attribute(string name, params object?[] arguments)
        {
            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var nameSyntax = SyntaxFactory.ParseName(name);
            if (arguments.Any())
            {
                return SyntaxFactory.Attribute(nameSyntax, SyntaxFactory.AttributeArgumentList(SyntaxFactory.SeparatedList(arguments.Select(l => SyntaxFactory.AttributeArgument(Literal(l))))));
            }

            return SyntaxFactory.Attribute(nameSyntax);
        }

        public static AttributeSyntax Attribute(string name, params ExpressionSyntax[] arguments)
        {
            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var nameSyntax = SyntaxFactory.ParseName(name);
            if (arguments != null && arguments.Any())
            {
                return SyntaxFactory.Attribute(nameSyntax, SyntaxFactory.AttributeArgumentList(SyntaxFactory.SeparatedList(arguments.Select(SyntaxFactory.AttributeArgument))));
            }

            return SyntaxFactory.Attribute(nameSyntax);
        }

        public static ExpressionSyntax IndexerAccess(ExpressionSyntax target, params ExpressionSyntax[] arguments)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            return SyntaxFactory.ElementAccessExpression(target)
                .WithArgumentList(
                    SyntaxFactory.BracketedArgumentList(ArgumentList(arguments)));
        }

        public static ObjectCreationExpressionSyntax ObjectCreation(TypeSyntax type, params ExpressionSyntax[] arguments)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            return SyntaxFactory.ObjectCreationExpression(type).WithArgumentList(Arguments(arguments));
        }

        public static ObjectCreationExpressionSyntax ObjectCreation(IGenerationOptions generationOptions, TypeSyntax type, IEnumerable<AssignmentExpressionSyntax> initializers)
        {
            if (initializers == null)
            {
                throw new ArgumentNullException(nameof(initializers));
            }

            var nodes = new List<SyntaxNodeOrToken>();
            foreach (var initializer in initializers)
            {
                if (nodes.Count > 0)
                {
                    var commaToken = SyntaxFactory.Token(SyntaxKind.CommaToken);
                    if (generationOptions.EmitMultilinePocoInitializers)
                    {
                        commaToken = commaToken.WithTrailingTrivia(SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "\n"));
                    }

                    nodes.Add(commaToken);
                }

                nodes.Add(initializer);
            }

            return SyntaxFactory.ObjectCreationExpression(type)
                .WithInitializer(SyntaxFactory.InitializerExpression(SyntaxKind.ObjectInitializerExpression, SyntaxFactory.SeparatedList<ExpressionSyntax>(nodes)));
        }

        public static AssignmentExpressionSyntax Assignment(string identifier, ExpressionSyntax value)
        {
            return SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName(identifier), value);
        }

        public static SectionedMethodHandler SetupMethod(ClassModel model, string targetTypeName, IFrameworkSet frameworkSet, ref ClassDeclarationSyntax classDeclaration)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (frameworkSet == null)
            {
                throw new ArgumentNullException(nameof(frameworkSet));
            }

            if (classDeclaration == null)
            {
                throw new ArgumentNullException(nameof(classDeclaration));
            }

            if (string.IsNullOrWhiteSpace(targetTypeName))
            {
                throw new ArgumentNullException(nameof(targetTypeName));
            }

            var setupMethod = frameworkSet.CreateSetupMethod(targetTypeName, model.ClassName);

            var parametersEmitted = new HashSet<ParameterModel>(new ParameterModelComparer());

            if (frameworkSet.Options.GenerationOptions.UseFieldForAutoFixture)
            {
                var namingContext = new NamingContext(model.ClassName);
                var autoFixtureFieldName = frameworkSet.NamingProvider.AutoFixtureFieldName.Resolve(namingContext);

                var creationExpression = AutoFixtureHelper.GetCreationExpression(frameworkSet.Options.GenerationOptions);

                setupMethod.Emit(Statement(Assignment(autoFixtureFieldName, creationExpression)));
            }

            // generate fields for each constructor parameter
            foreach (var parameterModel in model.Constructors.SelectMany(x => x.Parameters))
            {
                if (!parametersEmitted.Add(parameterModel))
                {
                    continue;
                }

                var fieldName = model.GetConstructorParameterFieldName(parameterModel, frameworkSet);
                var typeInfo = parameterModel.TypeInfo;

                AddConstructorField(model, frameworkSet, ref classDeclaration, setupMethod, fieldName, typeInfo);
            }

            if (!model.Constructors.Any())
            {
                foreach (var propertyModel in model.Properties.Where(x => x.HasInit))
                {
                    var fieldName = model.GetConstructorParameterFieldName(propertyModel, frameworkSet);
                    var typeInfo = propertyModel.TypeInfo;

                    AddConstructorField(model, frameworkSet, ref classDeclaration, setupMethod, fieldName, typeInfo);
                }
            }

            return setupMethod;
        }

        private static void AddConstructorField(ClassModel model, IFrameworkSet frameworkSet, ref ClassDeclarationSyntax classDeclaration, SectionedMethodHandler setupMethod, string fieldName, TypeInfo typeInfo)
        {
            var typeSyntax = typeInfo.ToTypeSyntax(frameworkSet.Context);
            ExpressionSyntax defaultExpression;
            if (typeInfo.ShouldUseMock())
            {
                typeSyntax = frameworkSet.MockingFramework.GetFieldType(typeSyntax);
                frameworkSet.Context.InterfacesMocked++;
                defaultExpression = frameworkSet.MockingFramework.GetFieldInitializer(typeInfo.ToTypeSyntax(frameworkSet.Context));
            }
            else
            {
                defaultExpression = AssignmentValueHelper.GetDefaultAssignmentValue(typeInfo, model.SemanticModel, frameworkSet);
            }

            AddConstructorField(frameworkSet, ref classDeclaration, setupMethod, fieldName, typeSyntax, defaultExpression);
        }

        private static void AddConstructorField(IFrameworkSet frameworkSet, ref ClassDeclarationSyntax classDeclaration, SectionedMethodHandler setupMethod, string fieldName, TypeSyntax typeSyntax, ExpressionSyntax defaultExpression)
        {
            var variable = SyntaxFactory.VariableDeclaration(typeSyntax).AddVariables(SyntaxFactory.VariableDeclarator(fieldName));
            var field = SyntaxFactory.FieldDeclaration(variable).AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));

            classDeclaration = classDeclaration.AddMembers(field);

            setupMethod.Emit(Statement(
                SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    frameworkSet.QualifyFieldReference(SyntaxFactory.IdentifierName(fieldName)),
                    defaultExpression)));
        }

        public static VariableDeclaratorSyntax VariableDeclarator(string name, ExpressionSyntax defaultValue)
        {
            return SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(KeywordSafeName(name))).WithInitializer(SyntaxFactory.EqualsValueClause(defaultValue));
        }

        public static VariableDeclarationSyntax AsDeclaration(this VariableDeclaratorSyntax declaratorSyntax, TypeSyntax typeSyntax)
        {
            return SyntaxFactory.VariableDeclaration(typeSyntax).WithVariables(SyntaxFactory.SingletonSeparatedList(declaratorSyntax));
        }

        public static LocalDeclarationStatementSyntax AsLocal(this VariableDeclaratorSyntax declaratorSyntax, TypeSyntax typeSyntax)
        {
            return SyntaxFactory.LocalDeclarationStatement(declaratorSyntax.AsDeclaration(typeSyntax));
        }

        public static LocalDeclarationStatementSyntax VariableDeclaration(ITypeSymbol type, IFrameworkSet frameworkSet, string name, ExpressionSyntax defaultValue)
        {
            return VariableDeclarator(name, defaultValue).AsLocal(AssignmentValueHelper.GetTypeOrImplicitType(type, frameworkSet));
        }

        public static LocalDeclarationStatementSyntax ImplicitlyTypedVariableDeclaration(string name, ExpressionSyntax defaultValue)
        {
            return VariableDeclarator(name, defaultValue).AsLocal(SyntaxFactory.IdentifierName("var"));
        }

        public static ParenthesizedLambdaExpressionSyntax ParenthesizedLambdaExpression(ExpressionSyntax expression)
        {
            if (expression is ParenthesizedLambdaExpressionSyntax parenthesizedLambda)
            {
                return parenthesizedLambda;
            }

            return SyntaxFactory.ParenthesizedLambdaExpression(SyntaxFactory.ParameterList(), expression);
        }

        public static ArgumentListSyntax Arguments(params CSharpSyntaxNode[] expressions)
        {
            return SyntaxFactory.ArgumentList(ArgumentList(expressions));
        }

        public static ArgumentListSyntax Arguments(IEnumerable<CSharpSyntaxNode> expressions)
        {
            return SyntaxFactory.ArgumentList(ArgumentList(expressions));
        }

        public static string KeywordSafeName(string name)
        {
            bool isKeyword = CSharpKeywordIdentifier.IsCSharpKeyword(name) ||
                             SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None ||
                             SyntaxFacts.GetContextualKeywordKind(name) != SyntaxKind.None;

            if (isKeyword)
            {
                return "@" + name;
            }

            return name;
        }

        public static ParameterSyntax Parameter(string name)
        {
            return SyntaxFactory.Parameter(SyntaxFactory.Identifier(KeywordSafeName(name)));
        }

        public static ParameterListSyntax ParameterList(IEnumerable<ParameterSyntax> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var tokens = new List<SyntaxNodeOrToken>();
            foreach (var parameter in parameters)
            {
                if (tokens.Count > 0)
                {
                    tokens.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
                }

                tokens.Add(parameter);
            }

            return SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList<ParameterSyntax>(tokens));
        }

        private static SeparatedSyntaxList<ArgumentSyntax> ArgumentList(params CSharpSyntaxNode[] expressions)
        {
            if (expressions == null)
            {
                throw new ArgumentNullException(nameof(expressions));
            }

            return ArgumentList(expressions.AsEnumerable());
        }

        private static SeparatedSyntaxList<ArgumentSyntax> ArgumentList(IEnumerable<CSharpSyntaxNode> expressions)
        {
            if (expressions == null)
            {
                throw new ArgumentNullException(nameof(expressions));
            }

            var tokens = new List<SyntaxNodeOrToken>();
            foreach (var expression in expressions)
            {
                if (expression is ArgumentSyntax argumentSyntax)
                {
                    if (tokens.Count > 0)
                    {
                        tokens.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
                    }

                    tokens.Add(argumentSyntax);
                }
                else if (expression is ExpressionSyntax expressionSyntax)
                {
                    if (tokens.Count > 0)
                    {
                        tokens.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
                    }

                    tokens.Add(SyntaxFactory.Argument(expressionSyntax));
                }
            }

            var separatedSyntaxList = SyntaxFactory.SeparatedList<ArgumentSyntax>(tokens);
            return separatedSyntaxList;
        }

        internal static BaseListSyntax? BaseList(params string[] testTypeBaseClasses)
        {
            var baseTypes = testTypeBaseClasses.Select(typeName => SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(typeName)));
            return SyntaxFactory.BaseList(SyntaxFactory.SeparatedList<BaseTypeSyntax>(baseTypes));
        }
    }
}