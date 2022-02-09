namespace Unitverse.Core.Tests.Helpers
{
    using Unitverse.Core.Helpers;
    using System;
    using NUnit.Framework;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis;
    using NSubstitute;
    using Microsoft.CodeAnalysis.CSharp;
    using System.Linq;

    [TestFixture]
    public class ConstructorFieldAssignmentExtractorTests
    {
        [Test]
        public void CanCallExtractMapFrom()
        {
            var classModel = ClassModelProvider.CreateModel(TestClasses.AutomaticMockGeneration);

            var result = ConstructorFieldAssignmentExtractor.ExtractMapFrom(classModel.Declaration, classModel.SemanticModel);
            result.GetConstructorParametersFor("_dummyService").Single().Name.Should().BeEquivalentTo("dummyService");
            result.GetConstructorParametersFor("_dummyService2").Single().Name.Should().BeEquivalentTo("dummyService2");
        }

        [Test]
        public void CannotCallExtractMapFromWithNullClassDeclaration()
        {
            FluentActions.Invoking(() => ConstructorFieldAssignmentExtractor.ExtractMapFrom(default(TypeDeclarationSyntax), Substitute.For<SemanticModel>())).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotCallExtractMapFromWithNullModel()
        {
            FluentActions.Invoking(() => ConstructorFieldAssignmentExtractor.ExtractMapFrom(SyntaxFactory.TypeDeclaration(SyntaxKind.ClassDeclaration, "cls"), default(SemanticModel))).Should().Throw<ArgumentNullException>();
        }
    }
}