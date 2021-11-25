namespace Unitverse.Core.Frameworks
{
    using System;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;

    public class FrameworkSet : IFrameworkSet
    {
        public FrameworkSet(ITestFramework testFramework, IMockingFramework mockingFramework, IAssertionFramework assertionFramework, INamingProvider namingProvider, IGenerationContext context, string testTypeNaming)
        {
            if (string.IsNullOrWhiteSpace(testTypeNaming))
            {
                throw new ArgumentNullException(nameof(testTypeNaming));
            }

            TestFramework = testFramework ?? throw new ArgumentNullException(nameof(testFramework));
            MockingFramework = mockingFramework ?? throw new ArgumentNullException(nameof(mockingFramework));
            AssertionFramework = assertionFramework ?? throw new ArgumentNullException(nameof(assertionFramework));
            NamingProvider = namingProvider ?? throw new ArgumentNullException(nameof(namingProvider));
            Context = context ?? throw new ArgumentNullException(nameof(context));
            TestTypeNaming = testTypeNaming;
        }

        public void ResetContext()
        {
            Context = new GenerationContext();
        }

        public ITestFramework TestFramework { get; }

        public IMockingFramework MockingFramework { get; }

        public IAssertionFramework AssertionFramework { get; }

        public INamingProvider NamingProvider { get; }

        public IGenerationContext Context { get; private set; }

        public string TestTypeNaming { get; }
    }
}
