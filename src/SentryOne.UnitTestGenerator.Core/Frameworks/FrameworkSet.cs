namespace SentryOne.UnitTestGenerator.Core.Frameworks
{
    using System;
    using SentryOne.UnitTestGenerator.Core.Helpers;

    public class FrameworkSet : IFrameworkSet
    {
        public FrameworkSet(ITestFramework testFramework, IMockingFramework mockingFramework, IGenerationContext context, string testTypeNaming)
        {
            if (string.IsNullOrWhiteSpace(testTypeNaming))
            {
                throw new ArgumentNullException(nameof(testTypeNaming));
            }

            TestFramework = testFramework ?? throw new ArgumentNullException(nameof(testFramework));
            MockingFramework = mockingFramework ?? throw new ArgumentNullException(nameof(mockingFramework));
            Context = context ?? throw new ArgumentNullException(nameof(context));
            TestTypeNaming = testTypeNaming;
        }

        public ITestFramework TestFramework { get; }

        public IMockingFramework MockingFramework { get; }

        public IGenerationContext Context { get; }

        public string TestTypeNaming { get; }
    }
}
