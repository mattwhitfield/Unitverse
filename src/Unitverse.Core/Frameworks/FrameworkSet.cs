namespace Unitverse.Core.Frameworks
{
    using System;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public class FrameworkSet : IFrameworkSet
    {
        public FrameworkSet(ITestFramework testFramework, IMockingFramework mockingFramework, IAssertionFramework assertionFramework, INamingProvider namingProvider, IGenerationContext context, IUnitTestGeneratorOptions options)
        {
            TestFramework = testFramework ?? throw new ArgumentNullException(nameof(testFramework));
            MockingFramework = mockingFramework ?? throw new ArgumentNullException(nameof(mockingFramework));
            AssertionFramework = assertionFramework ?? throw new ArgumentNullException(nameof(assertionFramework));
            NamingProvider = namingProvider ?? throw new ArgumentNullException(nameof(namingProvider));
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public ITestFramework TestFramework { get; }

        public IMockingFramework MockingFramework { get; }

        public IAssertionFramework AssertionFramework { get; }

        public INamingProvider NamingProvider { get; }

        public IGenerationContext Context { get; private set; }

        public IUnitTestGeneratorOptions Options { get; }

        public void EvaluateTargetModel(ClassModel classModel)
        {
            (TestFramework as IClassModelEvaluator)?.EvaluateTargetModel(classModel);
            (AssertionFramework as IClassModelEvaluator)?.EvaluateTargetModel(classModel);
            (MockingFramework as IClassModelEvaluator)?.EvaluateTargetModel(classModel);
        }
    }
}
