namespace SentryOne.UnitTestGenerator.Core.Models
{
    using System;
    using System.Text.RegularExpressions;

    public class SemanticVersion
    {
        private const RegexOptions Flags = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
        private static readonly Regex SemanticVersionRegex = new Regex(@"^(?<Version>\d+(\s*\.\s*\d+){0,3})(?<Release>-[a-z][0-9a-z-.]*)?$", Flags);

        private readonly string _extension;

        private readonly Version _version;

        private SemanticVersion(Version version, string specialVersion)
        {
            _version = new Version(version.Major, version.Minor, Math.Max(version.Build, 0), Math.Max(version.Revision, 0));
            _extension = specialVersion ?? string.Empty;
        }

        public static SemanticVersion Parse(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentNullException(nameof(version));
            }

            var match = SemanticVersionRegex.Match(version.Trim());
            if (!match.Success || !Version.TryParse(match.Groups["Version"].Value, out var versionValue))
            {
                throw new FormatException();
            }

            return new SemanticVersion(versionValue, match.Groups["Release"].Value.TrimStart('-'));
        }

        public bool IsNewerThan(SemanticVersion other)
        {
            if (other == null)
            {
                return true;
            }

            var result = _version.CompareTo(other._version);

            if (result != 0)
            {
                return result > 0;
            }

            if (string.IsNullOrEmpty(other._extension))
            {
                return false;
            }

            if (string.IsNullOrEmpty(_extension))
            {
                return true;
            }

            return StringComparer.OrdinalIgnoreCase.Compare(_extension, other._extension) > 0;
        }
    }
}