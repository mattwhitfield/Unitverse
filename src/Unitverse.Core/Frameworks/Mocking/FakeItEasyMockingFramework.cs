namespace Unitverse.Core.Frameworks.Mocking
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Resources;

    public class FakeItEasyMockingFramework : IMockingFramework
    {
        private readonly IGenerationContext _context;

        public FakeItEasyMockingFramework(IGenerationContext context)
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
            yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(Strings.FakeItEasyMockingFramework_GetUsings_FakeItEasy));
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
                    SyntaxFactory.IdentifierName("A"),
                    SyntaxFactory.GenericName(
                            SyntaxFactory.Identifier(Strings.FakeItEasyMockingFramework_MockInterface_Fake))
                        .WithTypeArgumentList(
                            SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(type)))));
        }

        private ExpressionSyntax GetArgument(ITypeSymbol typeSymbol, IGenerationContext context)
        {
            return SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.GenericName(SyntaxFactory.Identifier("A")).WithTypeArgumentList(MockingHelper.TypeArgumentList(new[] { typeSymbol }, context)),
                        SyntaxFactory.IdentifierName("_"));
        }

        public ExpressionSyntax GetSetupFor(IMethodSymbol dependencyMethod, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet)
        {
            var methodCall = MockingHelper.GetMethodCall(dependencyMethod, mockFieldName, GetArgument, _context);

            var methodReference = SyntaxFactory.IdentifierName("Returns");

            return SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        ACallTo(methodCall),
                        methodReference))
                    .WithArgumentList(Generate.Arguments(AssignmentValueHelper.GetDefaultAssignmentValue(MockingHelper.ReduceAsyncReturnType(dependencyMethod.ReturnType), model, frameworkSet)));
        }

        public ExpressionSyntax GetSetupFor(IPropertySymbol dependencyProperty, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet)
        {
            var propertyAccess = SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.IdentifierName(mockFieldName),
                                    SyntaxFactory.IdentifierName(dependencyProperty.Name));

            var methodReference = SyntaxFactory.IdentifierName("Returns");

            return SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            ACallTo(propertyAccess),
                            methodReference))
                    .WithArgumentList(Generate.Arguments(AssignmentValueHelper.GetDefaultAssignmentValue(dependencyProperty.Type, model, frameworkSet)));
        }

        public ExpressionSyntax GetAssertionFor(IMethodSymbol dependencyMethod, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet)
        {
            var methodCall = MockingHelper.GetMethodCall(dependencyMethod, mockFieldName, GetArgument, frameworkSet.Context);

            return SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        ACallTo(methodCall),
                        SyntaxFactory.IdentifierName("MustHaveHappened")));
        }

        private ExpressionSyntax ACallTo(ExpressionSyntax methodCall)
        {
            var aCallTo = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("A"), SyntaxFactory.IdentifierName("CallTo"));
            return SyntaxFactory.InvocationExpression(aCallTo).WithArgumentList(Generate.Arguments(SyntaxFactory.ParenthesizedLambdaExpression().WithExpressionBody(methodCall)));
        }

        public bool AwaitAsyncAssertions => false;
    }
}