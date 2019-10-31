namespace SentryOne.UnitTestGenerator.Core.Tests.Frameworks
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Frameworks.Mocking;
    using SentryOne.UnitTestGenerator.Core.Frameworks.Test;
    using SentryOne.UnitTestGenerator.Core.Options;

    [TestFixture]
    public static class FrameworkSetFactoryTests
    {
        [Test]
        [TestCase(TestFrameworkTypes.NUnit2, typeof(NUnit2TestFramework))]
        [TestCase(TestFrameworkTypes.NUnit3, typeof(NUnit3TestFramework))]
        [TestCase(TestFrameworkTypes.XUnit, typeof(XUnitTestFramework))]
        [TestCase(TestFrameworkTypes.MsTest, typeof(MsTestTestFramework))]
        public static void CanCallCreateForTestFramework(TestFrameworkTypes type, Type expectedType)
        {
            var options = Substitute.For<IUnitTestGeneratorOptions>();
            options.GenerationOptions.TestTypeNaming.Returns("{0}Tests");
            options.GenerationOptions.FrameworkType.Returns(type);
            options.GenerationOptions.MockingFrameworkType.Returns(MockingFrameworkType.Moq);
            var result = FrameworkSetFactory.Create(options);
            Assert.That(result.TestFramework, Is.InstanceOf(expectedType));
        }

        [Test]
        [TestCase(MockingFrameworkType.FakeItEasy, typeof(FakeItEasyMockingFramework))]
        [TestCase(MockingFrameworkType.RhinoMocks, typeof(RhinoMocksMockingFramework))]
        [TestCase(MockingFrameworkType.Moq, typeof(MoqMockingFramework))]
        [TestCase(MockingFrameworkType.NSubstitute, typeof(NSubstituteMockingFramework))]
        public static void CanCallCreateForMockingFramework(MockingFrameworkType type, Type expectedType)
        {
            var options = Substitute.For<IUnitTestGeneratorOptions>();
            options.GenerationOptions.TestTypeNaming.Returns("{0}Tests");
            options.GenerationOptions.FrameworkType.Returns(TestFrameworkTypes.NUnit2);
            options.GenerationOptions.MockingFrameworkType.Returns(type);
            var result = FrameworkSetFactory.Create(options);
            Assert.That(result.MockingFramework, Is.InstanceOf(expectedType));
        }

        [Test]
        public static void CannotCallCreateWithInvalidMockingFramework()
        {
            var options = Substitute.For<IUnitTestGeneratorOptions>();
            options.GenerationOptions.FrameworkType.Returns(TestFrameworkTypes.NUnit2);
            options.GenerationOptions.MockingFrameworkType.Returns((MockingFrameworkType)99999);
            Assert.Throws<NotSupportedException>(() => FrameworkSetFactory.Create(options));
        }

        [Test]
        public static void CannotCallCreateWithInvalidTestingFramework()
        {
            var options = Substitute.For<IUnitTestGeneratorOptions>();
            options.GenerationOptions.FrameworkType.Returns((TestFrameworkTypes)0);
            options.GenerationOptions.MockingFrameworkType.Returns(MockingFrameworkType.NSubstitute);
            Assert.Throws<NotSupportedException>(() => FrameworkSetFactory.Create(options));
        }

        [Test]
        public static void CannotCallCreateWithNullOptions()
        {
            Assert.Throws<ArgumentNullException>(() => FrameworkSetFactory.Create(default(IUnitTestGeneratorOptions)));
        }
    }
}