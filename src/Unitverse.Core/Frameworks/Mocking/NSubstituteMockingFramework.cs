namespace Unitverse.Core.Frameworks.Mocking
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;

    public class NSubstituteMockingFramework : IMockingFramework
    {
        private readonly IGenerationContext _context;

        public NSubstituteMockingFramework(IGenerationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ExpressionSyntax GetFieldReference(ExpressionSyntax fieldReference)
        {
            return fieldReference;
        }

        public TypeSyntax GetFieldType(TypeSyntax type)
        {
            return type;
        }

        public IEnumerable<UsingDirectiveSyntax> GetUsings()
        {
            yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("NSubstitute"));
        }

        public ExpressionSyntax GetThrowawayReference(TypeSyntax type)
        {
            return GetFieldInitializer(type);
        }

        public ExpressionSyntax GetFieldInitializer(TypeSyntax type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _context.MocksUsed = true;
            return SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("Substitute"),
                    SyntaxFactory.GenericName(
                            SyntaxFactory.Identifier("For"))
                        .WithTypeArgumentList(
                            SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(type)))));
        }

        private ExpressionSyntax GetArgument(ITypeSymbol typeSymbol, IGenerationContext context)
        {
            return SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("Arg"),
                        SyntaxFactory.GenericName(SyntaxFactory.Identifier("Any"))
                        .WithTypeArgumentList(MockingHelper.TypeArgumentList(new[] { typeSymbol }, context))));
        }

        public ExpressionSyntax GetSetupFor(IMethodSymbol dependencyMethod, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, ExpressionSyntax expectedReturnValue, IEnumerable<string> parameters)
        {
            var methodCall = MockingHelper.GetMethodCall(dependencyMethod, mockFieldName, MockingHelper.TranslateArgumentFunc(GetArgument, parameters), _context);

            var methodReference = SyntaxFactory.IdentifierName("Returns");

            return SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        methodCall,
                        methodReference))
                    .WithArgumentList(Generate.Arguments(expectedReturnValue));
        }

        public ExpressionSyntax GetSetupFor(IPropertySymbol dependencyProperty, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, ExpressionSyntax expectedReturnValue)
        {
            var propertyAccess = SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.IdentifierName(mockFieldName),
                                    SyntaxFactory.IdentifierName(dependencyProperty.Name));

            var methodReference = SyntaxFactory.IdentifierName("Returns");

            return SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            propertyAccess,
                            methodReference))
                    .WithArgumentList(Generate.Arguments(expectedReturnValue));
        }

        public ExpressionSyntax GetAssertionFor(IMethodSymbol dependencyMethod, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, IEnumerable<string> parameters)
        {
            var received = SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName(mockFieldName),
                    SyntaxFactory.IdentifierName("Received")));

            return MockingHelper.GetMethodCall(dependencyMethod, received, MockingHelper.TranslateArgumentFunc(GetArgument, parameters), frameworkSet.Context);
        }

        public virtual void AddSetupMethodStatements(SectionedMethodHandler setupMethod)
        {
        }

        public ExpressionSyntax GetObjectCreationExpression(TypeSyntax type)
        {
            return null;
        }

        public bool AwaitAsyncAssertions => true;
    }
}
