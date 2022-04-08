namespace Unitverse.Core.Tests.Models
{
    using Unitverse.Core.Models;
    using System;
    using NUnit.Framework;
    using FluentAssertions;
    using NSubstitute;
    using Microsoft.CodeAnalysis;

    [TestFixture]
    public class TypeSymbolProviderTests
    {
        private TypeSymbolProvider _testClass;
        private INamedTypeSymbol _typeSymbol;

        [SetUp]
        public void SetUp()
        {
            _typeSymbol = Substitute.For<INamedTypeSymbol>();
            _testClass = new TypeSymbolProvider(_typeSymbol);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new TypeSymbolProvider(_typeSymbol);
            
            // Assert
            instance.Should().NotBeNull();
        }

        [Test]
        public void CannotConstructWithNullTypeSymbol()
        {
            FluentActions.Invoking(() => new TypeSymbolProvider(default(INamedTypeSymbol))).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CanGetClassName()
        {
            // Arrange
            _typeSymbol.Name.Returns("foo");

            // Assert
            _testClass.ClassName.Should().Be("foo");
        }

        [Test]
        public void TypeSymbolIsInitializedCorrectly()
        {
            _testClass.TypeSymbol.Should().BeSameAs(_typeSymbol);
        }
    }
}