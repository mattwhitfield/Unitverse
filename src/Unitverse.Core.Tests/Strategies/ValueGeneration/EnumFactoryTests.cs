namespace Unitverse.Core.Tests.Strategies.ValueGeneration
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Strategies.ValueGeneration;

    [TestFixture]
    public static class EnumFactoryTests
    {
        [Test]
        public static void CannotCallRandomWithNullTypeSymbol()
        {
            Assert.Throws<ArgumentNullException>(() => EnumFactory.Random(default(ITypeSymbol), ClassModelProvider.Instance.SemanticModel, new HashSet<string>(StringComparer.OrdinalIgnoreCase), Substitute.For<IFrameworkSet>()));
        }

        [Test]
        public static void CannotCallRandomWithNullFrameworkSet()
        {
            Assert.Throws<ArgumentNullException>(() => EnumFactory.Random(Substitute.For<ITypeSymbol>(), ClassModelProvider.Instance.SemanticModel, new HashSet<string>(StringComparer.OrdinalIgnoreCase), default(IFrameworkSet)));
        }
    }
}