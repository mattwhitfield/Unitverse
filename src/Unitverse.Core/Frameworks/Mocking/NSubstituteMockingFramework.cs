namespace Unitverse.Core.Frameworks.Mocking
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;

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
            yield return Generate.UsingDirective("NSubstitute");
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
            return Generate.MemberInvocation("Substitute", Generate.GenericName("For", type));
        }

        private ExpressionSyntax GetArgument(ITypeSymbol typeSymbol, IGenerationContext context)
        {
            return Generate.MemberInvocation("Arg", Generate.GenericName("Any", typeSymbol, context));
        }

        public ExpressionSyntax GetSetupFor(IMethodSymbol dependencyMethod, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, ExpressionSyntax expectedReturnValue, IEnumerable<string> parameters)
        {
            var methodCall = MockingHelper.GetMethodCall(dependencyMethod, mockFieldName, MockingHelper.TranslateArgumentFunc(GetArgument, parameters), _context);

            return Generate.MemberInvocation(methodCall, "Returns", expectedReturnValue);
        }

        public ExpressionSyntax GetSetupFor(IPropertySymbol dependencyProperty, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, ExpressionSyntax expectedReturnValue)
        {
            var propertyAccess = Generate.MemberAccess(mockFieldName, dependencyProperty.Name);

            return Generate.MemberInvocation(propertyAccess, "Returns", expectedReturnValue);
        }

        public ExpressionSyntax GetAssertionFor(IMethodSymbol dependencyMethod, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, IEnumerable<string> parameters)
        {
            var received = Generate.MemberInvocation(mockFieldName, "Received");

            return MockingHelper.GetMethodCall(dependencyMethod, received, MockingHelper.TranslateArgumentFunc(GetArgument, parameters), frameworkSet.Context);
        }

        public ExpressionSyntax? GetObjectCreationExpression(TypeSyntax type)
        {
            return null;
        }

        public bool AwaitAsyncAssertions => true;
    }
}
