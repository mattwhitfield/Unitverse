namespace Unitverse.Core.Models
{
    using System;

    public class NugetPackageReference : INugetPackageReference
    {
        public NugetPackageReference(string name, string version)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            Version = string.IsNullOrWhiteSpace(version) ? null : version;
        }

        public string Name { get; }

        public string Version { get; }
    }
}