namespace SentryOne.UnitTestGenerator.Core.Frameworks
{
    using System;
    using SentryOne.UnitTestGenerator.Core.Assets;
    using SentryOne.UnitTestGenerator.Core.Frameworks.Mocking;
    using SentryOne.UnitTestGenerator.Core.Frameworks.Test;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Options;
    using SentryOne.UnitTestGenerator.Core.Resources;

    public static class FrameworkSetFactory
    {
        public static IFrameworkSet Create(IUnitTestGeneratorOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var context = new GenerationContext();
            return new FrameworkSet(Create(options.GenerationOptions.FrameworkType), Create(options.GenerationOptions.MockingFrameworkType, context), context, options);
        }

        private static IMockingFramework Create(MockingFrameworkType mockingFrameworkType, IGenerationContext context)
        {
            switch (mockingFrameworkType)
            {
                case MockingFrameworkType.NSubstitute:
                    return new NSubstituteMockingFramework(context);
                case MockingFrameworkType.Moq:
                    return new MoqMockingFramework(context);
                case MockingFrameworkType.RhinoMocks:
                    return new RhinoMocksMockingFramework(context);
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