namespace SentryOne.UnitTestGenerator.Core.Frameworks
{
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Options;

    public interface IFrameworkSet
    {
        ITestFramework TestFramework { get; }

        IMockingFramework MockingFramework { get; }

        IGenerationContext Context { get; }

        IUnitTestGeneratorOptions Options { get; }
    }
}