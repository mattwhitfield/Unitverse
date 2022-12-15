namespace Unitverse.Core.Generation
{
    using System;
    using Microsoft.CodeAnalysis;
    using Unitverse.Core.Options;

    public interface IGenerationItem
    {
        SyntaxNode? SourceNode { get; }

        IUnitTestGeneratorOptions Options { get; }

        Func<string, string> NamespaceTransform { get; }
    }
}
