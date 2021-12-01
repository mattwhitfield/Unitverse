namespace Unitverse.Core.Frameworks
{
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;

    public interface IFrameworkSet
    {
        ITestFramework TestFramework { get; }

        IMockingFramework MockingFramework { get; }

        IAssertionFramework AssertionFramework { get; }

        INamingProvider NamingProvider { get; }

        IGenerationContext Context { get; }

        string TestTypeNaming { get; }
    }
}