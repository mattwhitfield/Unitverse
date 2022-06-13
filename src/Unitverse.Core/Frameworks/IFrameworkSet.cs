namespace Unitverse.Core.Frameworks
{
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;

    public interface IFrameworkSet : IClassModelEvaluator, ITestMethodFactory
    {
        IUnitTestGeneratorOptions Options { get; }

        ITestFramework TestFramework { get; }

        IMockingFramework MockingFramework { get; }

        IAssertionFramework AssertionFramework { get; }

        INamingProvider NamingProvider { get; }

        IGenerationContext Context { get; }
    }
}