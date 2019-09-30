namespace SentryOne.UnitTestGenerator.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class StringExtensions
    {
        public static string ToCamelCase(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return source;
            }

            if (source.Length > 1)
            {
#pragma warning disable CA1308 // Normalize strings to uppercase
                return source.Substring(0, 1).ToLowerInvariant() + source.Substring(1);
#pragma warning restore CA1308 // Normalize strings to uppercase
            }

#pragma warning disable CA1308 // Normalize strings to uppercase
            return source.ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase
        }

        public static string ToPascalCase(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return source;
            }

            if (source.Length > 1)
            {
                return source.Substring(0, 1).ToUpperInvariant() + source.Substring(1);
            }

            return source.ToUpperInvariant();
        }

        public static IEnumerable<string> Lines(this string input)
        {
            return string.IsNullOrEmpty(input) ?
                Enumerable.Empty<string>() :
                input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
