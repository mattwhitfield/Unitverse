namespace Unitverse.Core.Generation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class CategorizedUsings
    {
        public CategorizedUsings(IEnumerable<UsingDirectiveSyntax> usings, bool separateSystemUsings)
        {
            var usingsEmitted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var usingDirective in usings)
            {
                var fullString = usingDirective.NormalizeWhitespace(string.Empty, string.Empty, false).ToFullString().Replace(" ", string.Empty);
                if (usingsEmitted.Add(fullString))
                {
                    if (fullString.IndexOf("<global", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        continue;
                    }

                    if (usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword))
                    {
                        if (separateSystemUsings && IsSystemUsing(usingDirective))
                        {
                            _staticSystemUsings.Add(usingDirective);
                        }
                        else
                        {
                            _staticNonSystemUsings.Add(usingDirective);
                        }
                    }
                    else if (usingDirective.Alias != null)
                    {
                        _aliasUsings.Add(usingDirective);
                    }
                    else if (separateSystemUsings && IsSystemUsing(usingDirective))
                    {
                        _systemUsings.Add(usingDirective);
                    }
                    else
                    {
                        _nonSystemUsings.Add(usingDirective);
                    }
                }
            }
        }

        private List<UsingDirectiveSyntax> _staticSystemUsings = new List<UsingDirectiveSyntax>();

        private List<UsingDirectiveSyntax> _staticNonSystemUsings = new List<UsingDirectiveSyntax>();

        private List<UsingDirectiveSyntax> _systemUsings = new List<UsingDirectiveSyntax>();

        private List<UsingDirectiveSyntax> _aliasUsings = new List<UsingDirectiveSyntax>();

        private List<UsingDirectiveSyntax> _nonSystemUsings = new List<UsingDirectiveSyntax>();

        public List<UsingDirectiveSyntax> GetResolvedUsingDirectives()
        {
            var resolvedUsings = new List<UsingDirectiveSyntax>();

            resolvedUsings.AddRange(_systemUsings.OrderBy(x => x.Name.ToString()));
            resolvedUsings.AddRange(_nonSystemUsings.OrderBy(x => x.Name.ToString()));
            resolvedUsings.AddRange(_aliasUsings.OrderBy(x => x.Alias?.ToString()));
            resolvedUsings.AddRange(_staticSystemUsings.OrderBy(x => x.Name.ToString()));
            resolvedUsings.AddRange(_staticNonSystemUsings.OrderBy(x => x.Name.ToString()));

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
