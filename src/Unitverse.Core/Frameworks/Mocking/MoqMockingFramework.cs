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
            yield return Generate.UsingDirective("Moq");
        }

        public TypeSyntax GetFieldType(TypeSyntax type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return Generate.GenericName("Mock", type);
        }

        public ExpressionSyntax GetFieldReference(ExpressionSyntax fieldReference)
        {
            if (fieldReference is null)
            {
                throw new ArgumentNullException(nameof(fieldReference));
            }

            _context.MocksUsed = true;
            return Generate.MemberAccess(fieldReference, "Object");
        }

        public ExpressionSyntax GetThrowawayReference(TypeSyntax type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _context.MocksUsed = true;
            return Generate.MemberAccess(GetMoqFieldInitializer(type), "Object");
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
            return Generate.ObjectCreation(Generate.GenericName("Mock", type));
        }

        private ExpressionSyntax GetArgument(ITypeSymbol typeSymbol, IGenerationContext context)
        {
            return Generate.MemberInvocation("It", Generate.GenericName("IsAny", typeSymbol, context));
        }

        public ExpressionSyntax GetSetupFor(IMethodSymbol dependencyMethod, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, ExpressionSyntax expectedReturnValue, IEnumerable<string> parameters)
        {
            var methodCall = MockingHelper.GetMethodCall(dependencyMethod, "mock", MockingHelper.TranslateArgumentFunc(GetArgument, parameters), _context);

            var isAsync = dependencyMethod.ReturnType is INamedTypeSymbol namedType && namedType.Name == "Task" && namedType.ContainingNamespace.ToDisplayString() == "System.Threading.Tasks";
            var methodName = isAsync ? "ReturnsAsync" : "Returns";

            return Generate.MemberInvocation(Mock("Setup", mockFieldName, methodCall), methodName, expectedReturnValue);
        }

        public ExpressionSyntax GetSetupFor(IPropertySymbol dependencyProperty, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, ExpressionSyntax expectedReturnValue)
        {
            var propertyAccess = Generate.MemberAccess("mock", dependencyProperty.Name);

            return Generate.MemberInvocation(Mock("Setup", mockFieldName, propertyAccess), "Returns", expectedReturnValue);
        }

        public ExpressionSyntax GetAssertionFor(IMethodSymbol dependencyMethod, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, IEnumerable<string> parameters)
        {
            var methodCall = MockingHelper.GetMethodCall(dependencyMethod, "mock", MockingHelper.TranslateArgumentFunc(GetArgument, parameters), _context);

            return Mock("Verify", mockFieldName, methodCall);
        }

        private static ExpressionSyntax Mock(string actionName, string mockFieldName, ExpressionSyntax methodCall)
        {
            return Generate.MemberInvocation(mockFieldName, actionName, SyntaxFactory.SimpleLambdaExpression(Generate.Parameter("mock"), methodCall));
        }

        public virtual ExpressionSyntax? GetObjectCreationExpression(TypeSyntax type)
        {
            return null;
        }

        public bool AwaitAsyncAssertions => false;
    }
}