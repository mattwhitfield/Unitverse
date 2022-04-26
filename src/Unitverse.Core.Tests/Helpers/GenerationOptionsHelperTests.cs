namespace Unitverse.Core.Tests.Helpers
{
    using Unitverse.Core.Helpers;
    using System;
    using NUnit.Framework;
    using FluentAssertions;
    using NSubstitute;
    using Unitverse.Core.Options;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.CSharp;

    [TestFixture]
    public static class GenerationOptionsHelperTests
    {
        [Test]
        public static void CanCallQualifyFieldReference()
        {
            // Arrange
            var options = Substitute.For<IGenerationOptions>();
            var nameSyntax = SyntaxFactory.IdentifierName("fred");
            
            // Act
            var result = options.QualifyFieldReference(nameSyntax);
            
            // Assert
            Assert.Fail("Create or modify test");
            result.Should().NotBeNull();
        }

        [Test]
        public static void CannotCallQualifyFieldReferenceWithNullOptions()
        {
            FluentActions.Invoking(() => default(IGenerationOptions).QualifyFieldReference(SyntaxFactory.IdentifierName("fred"))).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public static void CannotCallQualifyFieldReferenceWithNullNameSyntax()
        {
            FluentActions.Invoking(() => Substitute.For<IGenerationOptions>().QualifyFieldReference(default(SimpleNameSyntax))).Should().Throw<ArgumentNullException>();
        }
    }
}