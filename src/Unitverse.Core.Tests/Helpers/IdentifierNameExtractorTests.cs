namespace Unitverse.Core.Tests.Helpers
{
    using Unitverse.Core.Helpers;
    using System;
    using NUnit.Framework;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [TestFixture]
    public class IdentifierNameExtractorTests
    {
        [Test]
        public void CanCallExtractFrom()
        {
            var classModel = ClassModelProvider.CreateModel(TestClasses.AutomaticMockGeneration);

            var result = IdentifierNameExtractor.ExtractFrom(classModel.GetNode<ConstructorDeclarationSyntax>());
            result.Should().BeEquivalentTo("IDummyService", "IDummyService2", "_dummyService", "dummyService", "_dummyService2", "dummyService2", "_someIntField", "dummyService");
        }

        [Test]
        public void CannotCallExtractFromWithNullNode()
        {
            FluentActions.Invoking(() => IdentifierNameExtractor.ExtractFrom(default(CSharpSyntaxNode))).Should().Throw<ArgumentNullException>();
        }
    }
}