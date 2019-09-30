namespace SentryOne.UnitTestGenerator.Core.Models
{
    using System;

    public interface IReferencedAssembly
    {
        string Name { get; }

        // this is because nunit.framework is the same for v2 and v3 - so we need to have a locatable name that contains a version reference
        string LocatableName { get; }

        Version Version { get; }

        string Location { get; }
    }
}
