using System;

namespace SentryOne.UnitTestGenerator.Core.Tests.Models
{
    // TODO - tests
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
