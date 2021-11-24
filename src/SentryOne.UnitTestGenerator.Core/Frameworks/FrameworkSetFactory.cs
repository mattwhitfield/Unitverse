namespace Unitverse.Core.Frameworks
{
    using System;
    using Unitverse.Core.Frameworks.Assertion;
    using Unitverse.Core.Frameworks.Mocking;
    using Unitverse.Core.Frameworks.Test;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;
    using Unitverse.Core.Resources;

    public static class FrameworkSetFactory
    {
        public static IFrameworkSet Create(IUnitTestGeneratorOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var context = new GenerationContext();
            var testFramework = Create(options.GenerationOptions.FrameworkType);
            IAssertionFramework assertionFramework = testFramework;
            return new FrameworkSet(testFramework, Create(options.GenerationOptions.MockingFrameworkType, context), options.GenerationOptions.UseFluentAssertions ? new FluentAssertionFramework() : assertionFramework, context, options.GenerationOptions.TestTypeNaming);
        }

        private static IMockingFramework Create(MockingFrameworkType mockingFrameworkType, IGenerationContext context)
        {
            switch (mockingFrameworkType)
            {
                case MockingFrameworkType.NSubstitute:
                    return new NSubstituteMockingFramework(context);
                case MockingFrameworkType.Moq:
                    return new MoqMockingFramework(context);
                case MockingFrameworkType.FakeItEasy:
                    return new FakeItEasyMockingFramework(context);
                default:
                    throw new NotSupportedException(Strings.FrameworkSetFactory_Create_Couldn_t_find_the_required_mocking_framework);
            }
        }

        private static ITestFramework Create(TestFrameworkTypes testFrameworkTypes)
        {
            if ((testFrameworkTypes & TestFrameworkTypes.XUnit) > 0)
            {
                return new XUnitTestFramework();
            }

            if ((testFrameworkTypes & TestFrameworkTypes.NUnit3) > 0)
            {
                return new NUnit3TestFramework();
            }

            if ((testFrameworkTypes & TestFrameworkTypes.NUnit2) > 0)
            {
                return new NUnit2TestFramework();
            }

            if ((testFrameworkTypes & TestFrameworkTypes.MsTest) > 0)
            {
                return new MsTestTestFramework();
            }

            throw new NotSupportedException(Strings.FrameworkSetFactory_Create_Couldn_t_find_the_required_testing_framework);
        }
    }
}