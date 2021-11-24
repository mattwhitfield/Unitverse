namespace Unitverse.Core.Frameworks
{
    using Unitverse.Core.Helpers;

    public interface IFrameworkSet
    {
        ITestFramework TestFramework { get; }

        IMockingFramework MockingFramework { get; }

        IAssertionFramework AssertionFramework { get; }

        IGenerationContext Context { get; }

        string TestTypeNaming { get; }
    }
}