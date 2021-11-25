namespace Unitverse.Core.Options
{
    using System;

    public class NameResolver
    {
        private readonly string _pattern;

        public NameResolver(string pattern)
        {
            _pattern = pattern ?? string.Empty;
        }

        public string Resolve(NamingContext context)
        {
            return _pattern
                .Replace("{typeName}", context.TypeName ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("{interfaceName}", context.InterfaceName ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("{memberName}", context.MemberName ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("{memberBareName}", context.MemberBareName ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("{parameterName}", context.ParameterName ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("{typeParameters}", context.TypeParameters ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        }
    }
}
