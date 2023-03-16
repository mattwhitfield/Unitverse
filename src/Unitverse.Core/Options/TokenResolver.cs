namespace Unitverse.Core.Options
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Unitverse.Core.Helpers;

    public abstract class TokenResolver<T>
    {
        private static readonly Dictionary<string, Func<string?, string>> Formatters = new Dictionary<string, Func<string?, string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "lower", x => x == null ? string.Empty : x.ToLowerInvariant() },
            { "upper", x => x == null ? string.Empty : x.ToUpperInvariant() },
            { "camel", x => x == null ? string.Empty : x.ToCamelCase() },
            { "pascal", x => x == null ? string.Empty : x.ToPascalCase() },
        };

        private readonly string _pattern;

        public TokenResolver(string pattern)
        {
            _pattern = pattern ?? string.Empty;
        }

        protected abstract bool GetTokenValue(T context, string token, out string? value);

        public string Resolve(T context)
        {
            var output = new StringBuilder();
            var token = new StringBuilder();
            var formatter = new StringBuilder();

            var inToken = false;
            var inFormatter = false;
            foreach (var c in _pattern)
            {
                if (c == '{')
                {
                    inToken = true;
                    inFormatter = false;
                }
                else if (c == ':' && inToken)
                {
                    inFormatter = true;
                }
                else if (c == '}' && inToken)
                {
                    // emit token
                    if (GetTokenValue(context, token.ToString(), out var value))
                    {
                        if (formatter.Length == 0 || !Formatters.TryGetValue(formatter.ToString(), out var formatterFunc))
                        {
                            formatterFunc = x => x == null ? string.Empty : x;
                        }

                        output.Append(formatterFunc(value));
                    }

                    token.Clear();
                    formatter.Clear();
                    inToken = false;
                    inFormatter = false;
                }
                else if (inToken)
                {
                    if (inFormatter)
                    {
                        formatter.Append(c);
                    }
                    else
                    {
                        token.Append(c);
                    }
                }
                else
                {
                    output.Append(c);
                }
            }

            return output.ToString();
        }
    }
}
