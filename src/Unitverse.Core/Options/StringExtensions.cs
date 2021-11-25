namespace Unitverse.Core.Options
{
    using System;
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        public static string Replace(this string input, string oldValue, string newValue, StringComparison comparisonType)
        {
            return Regex.Replace(input, Regex.Escape(oldValue), newValue.Replace("$", "$$"), comparisonType == StringComparison.OrdinalIgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
        }
    }
}
