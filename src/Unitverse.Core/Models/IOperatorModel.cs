﻿namespace Unitverse.Core.Models
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;

    public interface IOperatorModel : ITestableModel<OperatorDeclarationSyntax>
    {
        string OperatorText { get; }

        IList<ParameterModel> Parameters { get; }

        ExpressionSyntax? Invoke(ClassModel owner, bool suppressAwait, IFrameworkSet frameworkSet, params CSharpSyntaxNode[] arguments);
    }
}