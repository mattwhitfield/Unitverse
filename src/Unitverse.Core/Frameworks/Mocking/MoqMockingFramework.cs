namespace Unitverse.Core.Frameworks.Mocking
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;

    public class MoqMockingFramework : IMockingFramework
    {
        private readonly IGenerationContext _context;

        public MoqMockingFramework(IGenerationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public virtual IEnumerable<UsingDirectiveSyntax> GetUsings()
        {
            yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Moq"));
        }

        public TypeSyntax GetFieldType(TypeSyntax type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return SyntaxFactory.GenericName(SyntaxFactory.Identifier("Mock"))
                                .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(type)));
        }

        public ExpressionSyntax GetFieldReference(ExpressionSyntax fieldReference)
        {
            if (fieldReference is null)
            {
                throw new ArgumentNullException(nameof(fieldReference));
            }

            _context.MocksUsed = true;
            return SyntaxFactory.MemberAccessExpression(
                     SyntaxKind.SimpleMemberAccessExpression,
                     fieldReference,
                     SyntaxFactory.IdentifierName("Object"));
        }

        public ExpressionSyntax GetThrowawayReference(TypeSyntax type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _context.MocksUsed = true;
            return SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    GetMoqFieldInitializer(type),
                    SyntaxFactory.IdentifierName("Object"));
        }

        public virtual ExpressionSyntax GetFieldInitializer(TypeSyntax type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return GetMoqFieldInitializer(type);
        }

        private ExpressionSyntax GetMoqFieldInitializer(TypeSyntax type)
        {
            _context.MocksUsed = true;
            return SyntaxFactory.ObjectCreationExpression(SyntaxFactory.GenericName(SyntaxFactory.Identifier("Mock"))
                                                                       .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(type))))
                                .WithArgumentList(SyntaxFactory.ArgumentList());
        }

        private ExpressionSyntax GetArgument(ITypeSymbol typeSymbol, IGenerationContext context)
        {
            return SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("It"),
                        SyntaxFactory.GenericName(SyntaxFactory.Identifier("IsAny"))
                        .WithTypeArgumentList(MockingHelper.TypeArgumentList(new[] { typeSymbol }, context))));
        }

        public ExpressionSyntax GetSetupFor(IMethodSymbol dependencyMethod, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, ExpressionSyntax expectedReturnValue, IEnumerable<string> parameters)
        {
            var methodCall = MockingHelper.GetMethodCall(dependencyMethod, "mock", MockingHelper.TranslateArgumentFunc(GetArgument, parameters), _context);

            var isAsync = dependencyMethod.ReturnType is INamedTypeSymbol namedType && namedType.Name == "Task" && namedType.ContainingNamespace.ToDisplayString() == "System.Threading.Tasks";
            var methodReference = SyntaxFactory.IdentifierName(isAsync ? "ReturnsAsync" : "Returns");

            return SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        Mock("Setup", mockFieldName, methodCall),
                        methodReference))
                    .WithArgumentList(Generate.Arguments(expectedReturnValue));
        }

        public ExpressionSyntax GetSetupFor(IPropertySymbol dependencyProperty, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, ExpressionSyntax expectedReturnValue)
        {
            var propertyAccess = SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.IdentifierName("mock"),
                                    SyntaxFactory.IdentifierName(dependencyProperty.Name));

            var methodReference = SyntaxFactory.IdentifierName("Returns");

            return SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            Mock("Setup", mockFieldName, propertyAccess),
                            methodReference))
                    .WithArgumentList(Generate.Arguments(expectedReturnValue));
        }

        public ExpressionSyntax GetAssertionFor(IMethodSymbol dependencyMethod, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, IEnumerable<string> parameters)
        {
            var methodCall = MockingHelper.GetMethodCall(dependencyMethod, "mock", MockingHelper.TranslateArgumentFunc(GetArgument, parameters), _context);

            return Mock("Verify", mockFieldName, methodCall);
        }

        private static ExpressionSyntax Mock(string actionName, string mockFieldName, ExpressionSyntax methodCall)
        {
            var mockSetup = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName(mockFieldName), SyntaxFactory.IdentifierName(actionName));
            return SyntaxFactory.InvocationExpression(mockSetup).WithArgumentList(Generate.Arguments(SyntaxFactory.SimpleLambdaExpression(Generate.Parameter("mock"), methodCall)));
        }

        public virtual void AddSetupMethodStatements(SectionedMethodHandler setupMethod)
        {
        }

        public virtual ExpressionSyntax GetObjectCreationExpression(TypeSyntax type)
        {
            return null;
        }

        public bool AwaitAsyncAssertions => false;
    }
}