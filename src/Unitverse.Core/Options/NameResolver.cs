namespace Unitverse.Core.Options
{
    using System;
    using System.Collections.Generic;

    public class NameResolver : TokenResolver<NamingContext>
    {
        private static readonly Dictionary<string, Func<NamingContext, string?>> TokenResolvers = new Dictionary<string, Func<NamingContext, string?>>(StringComparer.OrdinalIgnoreCase)
        {
            { "typeName", x => x.TypeName },
            { "interfaceName", x => x.InterfaceName },
            { "memberName", x => x.MemberName },
            { "memberBareName", x => x.MemberBareName },
            { "parameterName", x => x.ParameterName },
            { "typeParameters", x => x.TypeParameters },
        };

        public NameResolver(string pattern)
            : base(pattern)
        {
        }

        protected override bool GetTokenValue(NamingContext context, string token, out string? value)
        {
            if (TokenResolvers.TryGetValue(token, out var resolverFunc))
            {
                var resolved = resolverFunc(context);
                if (resolved != null)
                {
                    value = resolved;
                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}
