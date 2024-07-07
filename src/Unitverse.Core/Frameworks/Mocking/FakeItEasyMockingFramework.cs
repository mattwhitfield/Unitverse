namespace Unitverse.Core.Frameworks.Mocking
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;

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
            yield return Generate.UsingDirective("FakeItEasy");
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
            return Generate.MemberInvocation("A", Generate.GenericName("Fake", type));
        }

        private ExpressionSyntax GetArgument(ITypeSymbol typeSymbol, IGenerationContext context)
        {
            return Generate.MemberAccess(Generate.GenericName("A", typeSymbol, context), "_");
        }

        public ExpressionSyntax GetSetupFor(IMethodSymbol dependencyMethod, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, ExpressionSyntax expectedReturnValue, IEnumerable<string> parameters)
        {
            var methodCall = MockingHelper.GetMethodCall(dependencyMethod, mockFieldName, MockingHelper.TranslateArgumentFunc(GetArgument, parameters), _context);

            return Generate.MemberInvocation(ACallTo(methodCall), "Returns", expectedReturnValue);
        }

        public ExpressionSyntax GetSetupFor(IPropertySymbol dependencyProperty, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, ExpressionSyntax expectedReturnValue)
        {
            var propertyAccess = Generate.MemberAccess(mockFieldName, dependencyProperty.Name);

            return Generate.MemberInvocation(ACallTo(propertyAccess), "Returns", expectedReturnValue);
        }

        public ExpressionSyntax GetAssertionFor(IMethodSymbol dependencyMethod, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, IEnumerable<string> parameters)
        {
            var methodCall = MockingHelper.GetMethodCall(dependencyMethod, mockFieldName, MockingHelper.TranslateArgumentFunc(GetArgument, parameters), frameworkSet.Context);

            return Generate.MemberInvocation(ACallTo(methodCall), "MustHaveHappened");
        }

        private static ExpressionSyntax ACallTo(ExpressionSyntax methodCall)
        {
            return Generate.MemberInvocation("A", "CallTo", Generate.ParenthesizedLambdaExpression(methodCall));
        }

        public ExpressionSyntax? GetObjectCreationExpression(TypeSyntax type, bool isReferenceType)
        {
            return null;
        }

        public ExpressionSyntax? GetVoidSetupFor(IMethodSymbol dependencyMethod, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, IEnumerable<string> parameters)
        {
            return null;
        }

        public bool AwaitAsyncAssertions => false;
    }
}