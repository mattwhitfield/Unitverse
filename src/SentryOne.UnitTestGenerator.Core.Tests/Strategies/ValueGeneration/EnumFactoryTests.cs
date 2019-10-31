namespace SentryOne.UnitTestGenerator.Core.Tests.Strategies.ValueGeneration
{
    using System;
    using Microsoft.CodeAnalysis;
    using NSubstitute;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Strategies.ValueGeneration;

    [TestFixture]
    public static class EnumFactoryTests
    {
        [Test]
        public static void CannotCallRandomWithNullTypeSymbol()
        {
            Assert.Throws<ArgumentNullException>(() => EnumFactory.Random(default(ITypeSymbol), ClassModelProvider.Instance.SemanticModel, Substitute.For<IFrameworkSet>()));
        }

        [Test]
        public static void CannotCallRandomWithNullFrameworkSet()
        {
            Assert.Throws<ArgumentNullException>(() => EnumFactory.Random(Substitute.For<ITypeSymbol>(), ClassModelProvider.Instance.SemanticModel, default(IFrameworkSet)));
        }
    }
}