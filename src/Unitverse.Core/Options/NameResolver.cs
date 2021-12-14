namespace Unitverse.Core.Options
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Unitverse.Core.Helpers;

    public class NameResolver
    {
        private static readonly Dictionary<string, Func<NamingContext, string>> TokenResolvers = new Dictionary<string, Func<NamingContext, string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "typeName", x => x.TypeName },
            { "interfaceName", x => x.InterfaceName },
            { "memberName", x => x.MemberName },
            { "memberBareName", x => x.MemberBareName },
            { "parameterName", x => x.ParameterName },
            { "typeParameters", x => x.TypeParameters },
        };

        private static readonly Dictionary<string, Func<string, string>> Formatters = new Dictionary<string, Func<string, string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "lower", x => x.ToLowerInvariant() },
            { "upper", x => x.ToUpperInvariant() },
            { "camel", x => x.ToCamelCase() },
            { "pascal", x => x.ToPascalCase() },
        };

        private readonly string _pattern;

        public NameResolver(string pattern)
        {
            _pattern = pattern ?? string.Empty;
        }

        public string Resolve(NamingContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

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
                    if (TokenResolvers.TryGetValue(token.ToString(), out var resolverFunc))
                    {
                        if (formatter.Length == 0 || !Formatters.TryGetValue(formatter.ToString(), out var formatterFunc))
                        {
                            formatterFunc = x => x;
                        }

                        output.Append(formatterFunc(resolverFunc(context)));
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
