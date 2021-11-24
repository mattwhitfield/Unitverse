namespace Unitverse.Core.Models
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public interface IIndexerModel : ITestableModel<IndexerDeclarationSyntax>
    {
        IList<ParameterModel> Parameters { get; }

        TypeInfo TypeInfo { get; }

        bool HasGet { get; }

        bool HasSet { get; }
    }
}