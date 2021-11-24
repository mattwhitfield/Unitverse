namespace Unitverse.Core.Frameworks
{
    using System;
    using Unitverse.Core.Helpers;

    public class FrameworkSet : IFrameworkSet
    {
        public FrameworkSet(ITestFramework testFramework, IMockingFramework mockingFramework, IAssertionFramework assertionFramework, IGenerationContext context, string testTypeNaming)
        {
            if (string.IsNullOrWhiteSpace(testTypeNaming))
            {
                throw new ArgumentNullException(nameof(testTypeNaming));
            }

            TestFramework = testFramework ?? throw new ArgumentNullException(nameof(testFramework));
            MockingFramework = mockingFramework ?? throw new ArgumentNullException(nameof(mockingFramework));
            AssertionFramework = assertionFramework ?? throw new ArgumentNullException(nameof(assertionFramework));
            Context = context ?? throw new ArgumentNullException(nameof(context));
            TestTypeNaming = testTypeNaming;
        }

        public ITestFramework TestFramework { get; }

        public IMockingFramework MockingFramework { get; }

        public IAssertionFramework AssertionFramework { get; }

        public IGenerationContext Context { get; }

        public string TestTypeNaming { get; }
    }
}
