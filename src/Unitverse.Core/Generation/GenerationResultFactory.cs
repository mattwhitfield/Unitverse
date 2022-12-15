namespace Unitverse.Core.Generation
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;
    using Microsoft.CodeAnalysis.Options;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;

    internal static class GenerationResultFactory
    {
        public static GenerationResult CreateGenerationResult(CompilationUnitSyntax compilation, DocumentOptionSet? documentOptionSet, List<ClassModel> classModels, bool anyMethodsEmitted, IGenerationStatistics generationStatistics)
        {
            using (var workspace = new AdhocWorkspace())
            {
                compilation = (CompilationUnitSyntax)Formatter.Format(compilation, workspace, documentOptionSet);

                var generationResult = new GenerationResult(compilation.ToFullString(), anyMethodsEmitted, generationStatistics);
                foreach (var asset in classModels.SelectMany(x => x.RequiredAssets).Distinct())
                {
                    generationResult.RequiredAssets.Add(asset);
                }

                return generationResult;
            }
        }
    }
}
