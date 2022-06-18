namespace Unitverse.Core.Frameworks
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public interface IMockingFramework
    {
        bool AwaitAsyncAssertions { get; }

        IEnumerable<UsingDirectiveSyntax> GetUsings();

        TypeSyntax GetFieldType(TypeSyntax type);

        ExpressionSyntax GetFieldReference(ExpressionSyntax fieldReference);

        ExpressionSyntax GetFieldInitializer(TypeSyntax type);

        ExpressionSyntax GetThrowawayReference(TypeSyntax type);

        ExpressionSyntax GetSetupFor(IMethodSymbol dependencyMethod, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, ExpressionSyntax expectedReturnValue, IEnumerable<string> parameters);

        ExpressionSyntax GetSetupFor(IPropertySymbol dependencyProperty, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, ExpressionSyntax expectedReturnValue);

        ExpressionSyntax GetAssertionFor(IMethodSymbol dependencyMethod, string mockFieldName, SemanticModel model, IFrameworkSet frameworkSet, IEnumerable<string> parameters);

        ExpressionSyntax GetObjectCreationExpression(TypeSyntax typeSyntax);
    }
}