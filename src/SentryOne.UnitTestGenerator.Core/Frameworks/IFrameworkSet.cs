namespace SentryOne.UnitTestGenerator.Core.Frameworks
{
    using SentryOne.UnitTestGenerator.Core.Helpers;

    public interface IFrameworkSet
    {
        ITestFramework TestFramework { get; }

        IMockingFramework MockingFramework { get; }

        IGenerationContext Context { get; }

        string TestTypeNaming { get; }
    }
}