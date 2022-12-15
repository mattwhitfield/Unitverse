namespace Unitverse.Tests.Common
{
    using System;
    using Microsoft.CodeAnalysis;
    using Unitverse.Core.Generation;
    using Unitverse.Core.Options;

    public class TestGenerationItem : IGenerationItem
    {
        public TestGenerationItem(SyntaxNode? sourceNode, IUnitTestGeneratorOptions options, Func<string, string> namespaceTransform)
        {
            SourceNode = sourceNode;
            Options = options;
            NamespaceTransform = namespaceTransform;
        }

        public SyntaxNode? SourceNode { get; }

        public IUnitTestGeneratorOptions Options { get; }

        public Func<string, string> NamespaceTransform { get; }
    }
}
