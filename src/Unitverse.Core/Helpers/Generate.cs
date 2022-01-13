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
    using Unitverse.Core.Resources;

    public static class Generate
    {
        public static MethodDeclarationSyntax Method(string name, bool isAsync, bool isStatic)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(Strings.Generate_Method__void), name).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            if (isAsync)
            {
                method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(Strings.Generate_Method_Task), name).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

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

        public static LiteralExpressionSyntax Literal(object literal)
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

                methodReference = SyntaxFactory.GenericName(SyntaxFactory.Identifier(name)).WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList<TypeSyntax>(method.TypeParameterList.Parameters.Select(x => SyntaxFactory.IdentifierName(x.Identifier)))));
            }
            else
            {
                methodReference = SyntaxFactory.IdentifierName(name);
            }

            return SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        target,
                        methodReference))
                .WithArgumentList(Arguments(arguments));
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

        public static AttributeSyntax Attribute(string name, params object[] arguments)
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

        public static ExpressionSyntax PropertyAccess(ExpressionSyntax target, string propertyName)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            return SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                target,
                SyntaxFactory.IdentifierName(propertyName));
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

        public static ObjectCreationExpressionSyntax ObjectCreation(TypeSyntax type, IEnumerable<AssignmentExpressionSyntax> initializers)
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
                    nodes.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
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

        public static BaseMethodDeclarationSyntax SetupMethod(ClassModel model, string targetTypeName, IFrameworkSet frameworkSet, ref ClassDeclarationSyntax classDeclaration)
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

            var setupMethod = frameworkSet.TestFramework.CreateSetupMethod(targetTypeName);

            var parametersEmitted = new HashSet<string>();

            // generate fields for each constructor parameter
            foreach (var parameterModel in model.Constructors.SelectMany(x => x.Parameters))
            {
                if (!parametersEmitted.Add(parameterModel.Name))
                {
                    continue;
                }

                var fieldName = model.GetConstructorParameterFieldName(parameterModel, frameworkSet.NamingProvider);
                var typeSyntax = parameterModel.TypeInfo.ToTypeSyntax(frameworkSet.Context);

                if (parameterModel.TypeInfo.Type.TypeKind == TypeKind.Interface)
                {
                    typeSyntax = frameworkSet.MockingFramework.GetFieldType(typeSyntax);
                }

                var variable = SyntaxFactory.VariableDeclaration(typeSyntax)
                                        .AddVariables(SyntaxFactory.VariableDeclarator(fieldName));
                var field = SyntaxFactory.FieldDeclaration(variable)
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));

                classDeclaration = classDeclaration.AddMembers(field);

                ExpressionSyntax defaultExpression;
                if (parameterModel.TypeInfo.Type.TypeKind == TypeKind.Interface)
                {
                    frameworkSet.Context.InterfacesMocked++;
                    defaultExpression = frameworkSet.MockingFramework.GetFieldInitializer(parameterModel.TypeInfo.ToTypeSyntax(frameworkSet.Context));
                }
                else
                {
                    defaultExpression = AssignmentValueHelper.GetDefaultAssignmentValue(parameterModel.TypeInfo, model.SemanticModel, frameworkSet, false);
                }

                setupMethod = setupMethod.AddBodyStatements(SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        SyntaxFactory.IdentifierName(fieldName),
                        defaultExpression)));
            }

            return setupMethod;
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

        public static ParameterListSyntax ParameterList(IEnumerable<string> parameters)
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

                tokens.Add(SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameter)));
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
    }
}