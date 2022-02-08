namespace Unitverse.Core.Tests.Helpers
{
    using Unitverse.Core.Helpers;
    using System;
    using NUnit.Framework;
    using FluentAssertions;
    using Microsoft.CodeAnalysis;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp;
    using System.Linq;
    using NSubstitute;

    [TestFixture]
    public class InvocationExtractorTests
    {
        [Test]
        public void CanCallExtractFrom()
        {
            var classModel = ClassModelProvider.CreateModel(TestClasses.AutomaticMockGeneration);

            var targetFields = new[] { "_dummyService", "_dummyService2" };
            var result = InvocationExtractor.ExtractFrom(classModel.Methods.Single(x => x.Name == "SampleNoReturn").Node, classModel.SemanticModel, targetFields);
            result.GetAccessedPropertySymbolsFor("_dummyService2").Single().Name.Should().Be("SomeProp");
            result.GetAccessedMethodSymbolsFor("_dummyService").Select(x => x.Name).Should().BeEquivalentTo("NoReturnMethod", "GenericMethod");
            result.GetAccessedMethodSymbolsFor("_dummyService2").Select(x => x.Name).Should().BeEquivalentTo("ReturnMethod");
        }

        [Test]
        public void CannotCallExtractFromWithNullNode()
        {
            FluentActions.Invoking(() => InvocationExtractor.ExtractFrom(default(CSharpSyntaxNode), Substitute.For<SemanticModel>(), new[] { "TestValue1478414786", "TestValue1253389239", "TestValue1543172025" })).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotCallExtractFromWithNullSemanticModel()
        {
            FluentActions.Invoking(() => InvocationExtractor.ExtractFrom(SyntaxFactory.IdentifierName("hello"), default(SemanticModel), new[] { "TestValue1562618265", "TestValue1888707362", "TestValue2031161598" })).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotCallExtractFromWithNullTargetFields()
        {
            FluentActions.Invoking(() => InvocationExtractor.ExtractFrom(SyntaxFactory.IdentifierName("hello"), Substitute.For<SemanticModel>(), default(IEnumerable<string>))).Should().Throw<ArgumentNullException>();
        }
    }
}