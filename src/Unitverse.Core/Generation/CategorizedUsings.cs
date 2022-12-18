namespace Unitverse.Core.Generation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal class CategorizedUsings
    {
        public CategorizedUsings(IEnumerable<UsingDirectiveSyntax> usings, bool separateSystemUsings)
        {
            var usingsEmitted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var usingDirective in usings)
            {
                var fullString = usingDirective.NormalizeWhitespace().ToFullString();
                if (usingsEmitted.Add(fullString))
                {
                    if (usingDirective.Name is IdentifierNameSyntax ins && ins.Identifier.ValueText.StartsWith("<global", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword))
                    {
                        StaticUsings.Add(usingDirective);
                    }
                    else if (usingDirective.Alias != null)
                    {
                        AliasUsings.Add(usingDirective);
                    }
                    else if (separateSystemUsings && IsSystemUsing(usingDirective))
                    {
                        SystemUsings.Add(usingDirective);
                    }
                    else
                    {
                        NonSystemUsings.Add(usingDirective);
                    }
                }
            }
        }

        public List<UsingDirectiveSyntax> StaticUsings { get; } = new List<UsingDirectiveSyntax>();

        public List<UsingDirectiveSyntax> SystemUsings { get; } = new List<UsingDirectiveSyntax>();

        public List<UsingDirectiveSyntax> AliasUsings { get; } = new List<UsingDirectiveSyntax>();

        public List<UsingDirectiveSyntax> NonSystemUsings { get; } = new List<UsingDirectiveSyntax>();

        public List<UsingDirectiveSyntax> GetResolvedUsingDirectives()
        {
            var resolvedUsings = new List<UsingDirectiveSyntax>();

            resolvedUsings.AddRange(SystemUsings.OrderBy(x => x.Name.ToString()));
            resolvedUsings.AddRange(NonSystemUsings.OrderBy(x => x.Name.ToString()));
            resolvedUsings.AddRange(AliasUsings.OrderBy(x => x.Alias?.ToString()));
            resolvedUsings.AddRange(StaticUsings.OrderBy(x => x.Name.ToString()));

            return resolvedUsings;
        }

        private static bool IsSystemUsing(UsingDirectiveSyntax usingDirective)
        {
            var name = usingDirective.Name.ToString();
            return string.Equals(name, nameof(System), StringComparison.OrdinalIgnoreCase) ||
                   name.StartsWith(nameof(System) + ".", StringComparison.OrdinalIgnoreCase);
        }
    }
}
