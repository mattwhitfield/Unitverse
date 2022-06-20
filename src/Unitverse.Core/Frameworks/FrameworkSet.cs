namespace Unitverse.Core.Frameworks
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public class FrameworkSet : IFrameworkSet
    {
        private ITestMethodFactory _testMethodFactory;

        public FrameworkSet(IExtendedTestFramework testFramework, IMockingFramework mockingFramework, IAssertionFramework assertionFramework, INamingProvider namingProvider, IGenerationContext context, IUnitTestGeneratorOptions options)
        {
            TestFramework = testFramework ?? throw new ArgumentNullException(nameof(testFramework));
            _testMethodFactory = testFramework;
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

        public SectionedMethodHandler CreateSetupMethod(string targetTypeName, string className)
        {
            var methodHandler = _testMethodFactory.CreateSetupMethod(targetTypeName, className);
            Context.CurrentMethod = methodHandler;
            return methodHandler;
        }

        public SectionedMethodHandler CreateTestCaseMethod(NameResolver nameResolver, NamingContext namingContext, bool isAsync, bool isStatic, TypeSyntax valueType, IEnumerable<object?> testValues, string description)
        {
            var methodHandler = _testMethodFactory.CreateTestCaseMethod(nameResolver, namingContext, isAsync, isStatic, valueType, testValues, description);
            Context.CurrentMethod = methodHandler;
            return methodHandler;
        }

        public SectionedMethodHandler CreateTestMethod(NameResolver nameResolver, NamingContext namingContext, bool isAsync, bool isStatic, string description)
        {
            var methodHandler = _testMethodFactory.CreateTestMethod(nameResolver, namingContext, isAsync, isStatic, description);
            Context.CurrentMethod = methodHandler;
            return methodHandler;
        }

        public void EvaluateTargetModel(ClassModel classModel)
        {
            (TestFramework as IClassModelEvaluator)?.EvaluateTargetModel(classModel);
            (AssertionFramework as IClassModelEvaluator)?.EvaluateTargetModel(classModel);
            (MockingFramework as IClassModelEvaluator)?.EvaluateTargetModel(classModel);
        }
    }
}
