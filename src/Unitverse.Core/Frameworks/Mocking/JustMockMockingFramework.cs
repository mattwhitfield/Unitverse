namespace Unitverse.Core.Frameworks.Mocking
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;

    public class JustMockMockingFramework : IMockingFramework
    {
        private readonly IGenerationContext _context;

        public JustMockMockingFramework(IGenerationContext context)
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
            yield return Generate.UsingDirective("Telerik.JustMock");
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
            return Generate.MemberInvocation("Mock", Generate.GenericName("Create", type));
        }

        private ExpressionSyntax GetArgument(ITypeSymbol typeSymbol, IGenerationContext context)
        {
            return Generate.MemberInvocation("Arg", Generate.GenericName("IsAny", typeSymbol, context));
        }

        public ExpressionSyntax GetSetupFor(IMethodSymbol dependencyMethod, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, ExpressionSyntax expectedReturnValue, IEnumerable<string> parameters)
        {
            var methodCall = MockingHelper.GetMethodCall(dependencyMethod, mockFieldName, MockingHelper.TranslateArgumentFunc(GetArgument, parameters), _context);

            if (dependencyMethod.IsAsyncCallable())
            {
                expectedReturnValue = Generate.MemberInvocation("Task", "FromResult", expectedReturnValue);
            }

            return Generate.MemberInvocation(Mock("Arrange", methodCall), "Returns", expectedReturnValue);
        }

        public ExpressionSyntax GetSetupFor(IPropertySymbol dependencyProperty, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, ExpressionSyntax expectedReturnValue)
        {
            var propertyAccess = Generate.MemberAccess(mockFieldName, dependencyProperty.Name);

            return Generate.MemberInvocation(Mock("Arrange", propertyAccess), "Returns", expectedReturnValue);
        }

        public ExpressionSyntax GetAssertionFor(IMethodSymbol dependencyMethod, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, IEnumerable<string> parameters)
        {
            var methodCall = MockingHelper.GetMethodCall(dependencyMethod, mockFieldName, MockingHelper.TranslateArgumentFunc(GetArgument, parameters), frameworkSet.Context);

            return Mock("Assert", methodCall);
        }

        private static ExpressionSyntax Mock(string methodName, ExpressionSyntax methodCall)
        {
            return Generate.MemberInvocation("Mock", methodName, Generate.ParenthesizedLambdaExpression(methodCall));
        }

        public ExpressionSyntax? GetObjectCreationExpression(TypeSyntax type)
        {
            return null;
        }

        public bool AwaitAsyncAssertions => false;
    }
}