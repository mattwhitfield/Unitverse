namespace Unitverse.Core.Generation
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class ICompilationUnitStrategyExtensions
    {
        public static void AddUsings(this ICompilationUnitStrategy strategy, IEnumerable<UsingDirectiveSyntax> usings)
        {
            foreach (var usingDirective in usings)
            {
                strategy.AddUsing(usingDirective);
            }
        }
    }
}
