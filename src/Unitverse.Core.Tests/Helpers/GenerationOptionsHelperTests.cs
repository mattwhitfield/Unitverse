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
    using Unitverse.Core.Frameworks;

    [TestFixture]
    public static class GenerationOptionsHelperTests
    {
        [Test]
        public static void CanCallQualifyFieldReference()
        {
            // Arrange
            var set = Substitute.For<IFrameworkSet>();
            var fullOptions = Substitute.For<IUnitTestGeneratorOptions>();
            var options = Substitute.For<IGenerationOptions>();
            set.Options.Returns(fullOptions);
            fullOptions.GenerationOptions.Returns(options);
            options.PrefixFieldReferencesWithThis.Returns(true);
            var nameSyntax = SyntaxFactory.IdentifierName("fred");

            // Act
            var result = set.QualifyFieldReference(nameSyntax);

            // Assert
            result.ToFullString().Should().Be("this.fred");
            result.Should().NotBeNull();
        }

        [Test]
        public static void QualifyFieldReferenceDoesNotQualifyWhenNotConfigured()
        {
            // Arrange
            var set = Substitute.For<IFrameworkSet>();
            var fullOptions = Substitute.For<IUnitTestGeneratorOptions>();
            var options = Substitute.For<IGenerationOptions>();
            set.Options.Returns(fullOptions);
            fullOptions.GenerationOptions.Returns(options);
            options.PrefixFieldReferencesWithThis.Returns(false);
            var nameSyntax = SyntaxFactory.IdentifierName("fred");

            // Act
            var result = set.QualifyFieldReference(nameSyntax);

            // Assert
            result.ToFullString().Should().Be("fred");
            result.Should().NotBeNull();
        }

        [Test]
        public static void CannotCallQualifyFieldReferenceWithNullOptions()
        {
            FluentActions.Invoking(() => default(IFrameworkSet).QualifyFieldReference(SyntaxFactory.IdentifierName("fred"))).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public static void CannotCallQualifyFieldReferenceWithNullNameSyntax()
        {
            FluentActions.Invoking(() => Substitute.For<IFrameworkSet>().QualifyFieldReference(default(SimpleNameSyntax))).Should().Throw<ArgumentNullException>();
        }
    }
}