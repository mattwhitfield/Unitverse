namespace Unitverse.Core.Frameworks.Test
{
    using Unitverse.Core.Options;

    public abstract class BaseTestFramework
    {
        protected BaseTestFramework(IUnitTestGeneratorOptions options)
        {
            Options = options;
        }

        public IUnitTestGeneratorOptions Options { get; }
    }
}
