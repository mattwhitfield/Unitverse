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

            // naming
            var namingProvider = new NamingProvider(options.NamingOptions);

            var context = new GenerationContext(options.GenerationOptions, namingProvider);

            // test
            var testFramework = CreateTestFramework(options);

            // assertion
            IAssertionFramework assertionFramework = testFramework;
            if (options.GenerationOptions.UseFluentAssertions)
            {
                assertionFramework = new FluentAssertionFramework(assertionFramework);
            }

            // mocking
            var mockingFramework = Create(options.GenerationOptions.MockingFrameworkType, context);
            if (options.GenerationOptions.CanUseAutoFixtureForMocking())
            {
                mockingFramework = new AutoFixtureMockingAdaptor(mockingFramework, context);
            }

            return new FrameworkSet(testFramework, mockingFramework, assertionFramework, namingProvider, context, options);
        }

        private static IMockingFramework Create(MockingFrameworkType mockingFrameworkType, IGenerationContext context)
        {
            switch (mockingFrameworkType)
            {
                case MockingFrameworkType.NSubstitute:
                    return new NSubstituteMockingFramework(context);
                case MockingFrameworkType.Moq:
                    return new MoqMockingFramework(context);
                case MockingFrameworkType.MoqAutoMock:
                    return new MoqAutoMockMockingFramework(context);
                case MockingFrameworkType.FakeItEasy:
                    return new FakeItEasyMockingFramework(context);
                case MockingFrameworkType.JustMock:
                    return new JustMockMockingFramework(context);
                default:
                    throw new NotSupportedException(Strings.FrameworkSetFactory_Create_Couldn_t_find_the_required_mocking_framework);
            }
        }

        private static IExtendedTestFramework CreateTestFramework(IUnitTestGeneratorOptions options)
        {
            var testFrameworkTypes = options.GenerationOptions.FrameworkType;

            if ((testFrameworkTypes & TestFrameworkTypes.XUnit) > 0)
            {
                return new XUnitTestFramework(options);
            }

            if ((testFrameworkTypes & TestFrameworkTypes.NUnit3) > 0)
            {
                return new NUnit3TestFramework(options);
            }

            if ((testFrameworkTypes & TestFrameworkTypes.NUnit2) > 0)
            {
                return new NUnit2TestFramework(options);
            }

            if ((testFrameworkTypes & TestFrameworkTypes.MsTest) > 0)
            {
                return new MsTestTestFramework(options);
            }

            throw new NotSupportedException(Strings.FrameworkSetFactory_Create_Couldn_t_find_the_required_testing_framework);
        }
    }
}