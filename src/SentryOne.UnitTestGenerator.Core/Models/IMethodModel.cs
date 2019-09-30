namespace SentryOne.UnitTestGenerator.Core.Models
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Frameworks;

    public interface IMethodModel : ITestableModel<MethodDeclarationSyntax>
    {
        bool IsAsync { get; }

        bool IsVoid { get; }

        IList<ParameterModel> Parameters { get; }

        ExpressionSyntax Invoke(ClassModel owner, bool suppressAwait, IFrameworkSet frameworkSet, params CSharpSyntaxNode[] arguments);
    }
}