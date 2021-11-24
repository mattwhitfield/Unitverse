namespace Unitverse.Core.Strategies
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Models;

    public interface IGenerationStrategy<in T>
    {
        bool IsExclusive { get; }

        int Priority { get; }

        bool CanHandle(T member, ClassModel model);

        IEnumerable<MethodDeclarationSyntax> Create(T method, ClassModel model);
    }
}