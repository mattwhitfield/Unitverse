namespace Unitverse.Core.Frameworks.Mocking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Resources;

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
            yield return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(Strings.NSubstituteMockingFramework_GetUsings_NSubstitute));
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
                            SyntaxFactory.Identifier(Strings.NSubstituteMockingFramework_MockInterface_For))
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

        public ExpressionSyntax GetSetupFor(IMethodSymbol dependencyMethod, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet)
        {
            var methodCall = MockingHelper.GetMethodCall(dependencyMethod, mockFieldName, GetArgument, _context);

            var methodReference = SyntaxFactory.IdentifierName("Returns");

            return SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        methodCall,
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
                            propertyAccess,
                            methodReference))
                    .WithArgumentList(Generate.Arguments(AssignmentValueHelper.GetDefaultAssignmentValue(dependencyProperty.Type, model, frameworkSet)));
        }

        public ExpressionSyntax GetAssertionFor(IMethodSymbol dependencyMethod, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet)
        {
            var received = SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName(mockFieldName),
                    SyntaxFactory.IdentifierName("Received")));

            return MockingHelper.GetMethodCall(dependencyMethod, received, GetArgument, frameworkSet.Context);
        }

        public bool AwaitAsyncAssertions => true;
    }
}
