using System;

namespace Unitverse.Core.Tests.Models
{
    public class ReferencedAssembly
    {
        public ReferencedAssembly(string assemblyName, int majorVersion)
        {
            if (string.IsNullOrWhiteSpace(assemblyName))
            {
                throw new ArgumentNullException(nameof(assemblyName));
            }

            AssemblyName = assemblyName;
            MajorVersion = majorVersion;
        }

        public string AssemblyName { get; }
        public int MajorVersion { get; }
    }
}
