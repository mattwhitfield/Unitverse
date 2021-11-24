namespace SentryOne.UnitTestGenerator.Core.Frameworks
{
    using SentryOne.UnitTestGenerator.Core.Helpers;

    public interface IFrameworkSet
    {
        ITestFramework TestFramework { get; }

        IMockingFramework MockingFramework { get; }

        IAssertionFramework AssertionFramework { get; }

        IGenerationContext Context { get; }

        string TestTypeNaming { get; }
    }
}