namespace SentryOne.UnitTestGenerator.Core.Models
{
    using System;
    using System.Globalization;

    public class ReferencedAssembly : IReferencedAssembly
    {
        public ReferencedAssembly(string name, Version version, string location)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            if (string.IsNullOrWhiteSpace(location))
            {
                throw new ArgumentNullException(nameof(location));
            }

            Name = name;
            Version = version;
            Location = location;
        }

        public string Name { get; }

        public string LocatableName
        {
            get
            {
                if (!string.Equals(Name, "nunit.framework", StringComparison.OrdinalIgnoreCase))
                {
                    return Name;
                }

                return string.Format(CultureInfo.InvariantCulture, "nunit.framework({0})", Version.Major);
            }
        }

        public Version Version { get; }

        public string Location { get; }
    }
}
