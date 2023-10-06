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
    using Unitverse.Core.Models;

    [TestFixture]
    public class InvocationExtractorTests
    {
        [Test]
        public void CanCallExtractFrom()
        {
            var classModel = ClassModelProvider.CreateModel(TestClasses.AutomaticMockGeneration);

            var targetFields = new[] { "_dummyService", "_dummyService2" };
            var result = InvocationExtractor.ExtractFrom(classModel, classModel.Methods.Single(x => x.Name == "SampleNoReturn").Node, targetFields);
            result.GetAccessedPropertySymbolsFor("_dummyService2").Single().Name.Should().Be("SomeProp");
            result.GetAccessedMethodSymbolsFor("_dummyService").Select(x => x.Name).Should().BeEquivalentTo("NoReturnMethod", "GenericMethod");
            result.GetAccessedMethodSymbolsFor("_dummyService2").Select(x => x.Name).Should().BeEquivalentTo("ReturnMethod");
        }

        [Test]
        public void ExtractFrom_DependencyCalledInsidePrivateMethod_ReturnsCalledMethods()
        {
            var classModel = ClassModelProvider.CreateModel(TestClasses.AutomaticMockGeneration);

            var targetFields = new[] { "_dummyService", "_dummyService2" };
            var result = InvocationExtractor.ExtractFrom(classModel, classModel.Methods.Single(x => x.Name == "SampleAsyncMethod").Node, targetFields);
            result.GetAccessedMethodSymbolsFor("_dummyService").Select(x => x.Name).Should().BeEquivalentTo("AsyncMethod");
            result.GetAccessedMethodSymbolsFor("_dummyService2").Select(x => x.Name).Should().BeEquivalentTo("AsyncMethod");
        }

        [Test]
        public void ExtractFrom_DependencyCalledInsidePublicMethod_ReturnsCalledMethods()
        {
            var classModel = ClassModelProvider.CreateModel(TestClasses.AutomaticMockGeneration);

            var targetFields = new[] { "_dummyService", "_dummyService2" };
            var result = InvocationExtractor.ExtractFrom(classModel, classModel.Methods.Single(x => x.Name == "SampleDependencyCalledInsidePublicMethod").Node, targetFields);
            result.GetAccessedMethodSymbolsFor("_dummyService").Select(x => x.Name).Should().BeEquivalentTo("AsyncMethod");
            result.GetAccessedMethodSymbolsFor("_dummyService2").Select(x => x.Name).Should().BeEquivalentTo("AsyncMethod");
        }

        [Test]
        public void ExtractFrom_DeeperNestedDependencyCall_ReturnsCalledMethods()
        {
            var classModel = ClassModelProvider.CreateModel(TestClasses.AutomaticMockGeneration);

            var targetFields = new[] { "_dummyService", "_dummyService2" };
            var result = InvocationExtractor.ExtractFrom(classModel, classModel.Methods.Single(x => x.Name == "SampleDeeperNestedDependencyCall").Node, targetFields);
            result.GetAccessedMethodSymbolsFor("_dummyService").Select(x => x.Name).Should().BeEquivalentTo("AsyncMethod");
            result.GetAccessedMethodSymbolsFor("_dummyService2").Select(x => x.Name).Should().BeEquivalentTo("AsyncMethod");
        }

        [Test]
        public void ExtractFrom_DependencyCalledWithDelegate_ReturnsCalledMethods()
        {
            var classModel = ClassModelProvider.CreateModel(TestClasses.AutomaticMockGeneration);

            var targetFields = new[] { "_dummyService", "_dummyService2" };
            var result = InvocationExtractor.ExtractFrom(classModel, classModel.Methods.Single(x => x.Name == "SampleDependencyCalledAsADelegateMethod").Node, targetFields);
            result.GetAccessedMethodSymbolsFor("_dummyService").Select(x => x.Name).Should().BeEquivalentTo("AsyncMethod");
            result.GetAccessedMethodSymbolsFor("_dummyService2").Select(x => x.Name).Should().BeEquivalentTo("AsyncMethod");
        }

        [Test]
        public void ExtractFrom_DependencyCalledWithLambda_ReturnsCalledMethods()
        {
            var classModel = ClassModelProvider.CreateModel(TestClasses.AutomaticMockGeneration);

            var targetFields = new[] { "_dummyService", "_dummyService2" };
            var result = InvocationExtractor.ExtractFrom(classModel, classModel.Methods.Single(x => x.Name == "SampleDependencyCalledAsALambdaMethod").Node, targetFields);
            result.GetAccessedMethodSymbolsFor("_dummyService").Select(x => x.Name).Should().BeEquivalentTo("AsyncMethod");
            result.GetAccessedMethodSymbolsFor("_dummyService2").Select(x => x.Name).Should().BeEquivalentTo("AsyncMethod");
        }

        [Test]
        public void ExtractFrom_DependencyCalledWithAction_ReturnsCalledMethods()
        {
            var classModel = ClassModelProvider.CreateModel(TestClasses.AutomaticMockGeneration);

            var targetFields = new[] { "_dummyService", "_dummyService2" };
            var result = InvocationExtractor.ExtractFrom(classModel, classModel.Methods.Single(x => x.Name == "SampleDependencyCalledAsAActionMethod").Node, targetFields);
            result.GetAccessedMethodSymbolsFor("_dummyService").Select(x => x.Name).Should().BeEquivalentTo("AsyncMethod");
            result.GetAccessedMethodSymbolsFor("_dummyService2").Select(x => x.Name).Should().BeEquivalentTo("AsyncMethod");
        }

        [Test]
        public void CannotCallExtractFromWithNullNode()
        {
            FluentActions.Invoking(() => InvocationExtractor.ExtractFrom(ClassModelProvider.CreateModel(TestClasses.AutomaticMockGeneration), default(CSharpSyntaxNode), new[] { "TestValue1478414786", "TestValue1253389239", "TestValue1543172025" })).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotCallExtractFromWithNullModel()
        {
            FluentActions.Invoking(() => InvocationExtractor.ExtractFrom(default(ClassModel), SyntaxFactory.IdentifierName("hello"), new[] { "TestValue1562618265", "TestValue1888707362", "TestValue2031161598" })).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotCallExtractFromWithNullTargetFields()
        {
            FluentActions.Invoking(() => InvocationExtractor.ExtractFrom(ClassModelProvider.CreateModel(TestClasses.AutomaticMockGeneration), SyntaxFactory.IdentifierName("hello"), default(IEnumerable<string>))).Should().Throw<ArgumentNullException>();
        }
    }
}