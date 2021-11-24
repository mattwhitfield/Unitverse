namespace Unitverse.Core.Tests.Strategies.InterfaceGeneration
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Frameworks.Mocking;
    using Unitverse.Core.Frameworks.Test;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Strategies.InterfaceGeneration;

    [TestFixture]
    public class ComparableGenerationStrategyTests
    {
        private ComparableGenerationStrategy _testClass;
        private IFrameworkSet _frameworkSet;

        [SetUp]
        public void SetUp()
        {
            var generationContext = new GenerationContext();

            _frameworkSet = new FrameworkSet(new NUnit3TestFramework(), new NSubstituteMockingFramework(generationContext), new NUnit3TestFramework(), generationContext, "{0}Tests");
            _testClass = new ComparableGenerationStrategy(_frameworkSet);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new ComparableGenerationStrategy(_frameworkSet);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullFrameworkSet()
        {
            Assert.Throws<ArgumentNullException>(() => new ComparableGenerationStrategy(default(IFrameworkSet)));
        }

        [Test]
        public void CanGetSupportedInterfaceName()
        {
            Assert.That(_testClass.SupportedInterfaceName, Is.InstanceOf<string>());
            Assert.That(_testClass.SupportedInterfaceName, Is.EqualTo("System.IComparable"));
        }

        [Test]
        public void CanCallCreate()
        {
            var syntaxTree = TestSemanticModelFactory.CreateTree(TestClasses.IComparableTestFile);
            var model = TestSemanticModelFactory.CreateSemanticModel(syntaxTree);
            var extractor = new TestableItemExtractor(syntaxTree, model);
            var classModel = extractor.Extract(syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().First()).First();

            var result = _testClass.Create(classModel, classModel);

            // 3 IComparable definitions on the test class
            Assert.That(result.Count(), Is.EqualTo(classModel.Interfaces.Count));
        }

        [Test]
        public void CannotCallCreateWithNullClassModel()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Create(default(ClassModel), new ClassModel(TestSemanticModelFactory.Class, TestSemanticModelFactory.Model, true)).Consume());
        }

        [Test]
        public void CannotCallCreateWithNullModel()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Create(new ClassModel(TestSemanticModelFactory.Class, TestSemanticModelFactory.Model, true), default(ClassModel)).Consume());
        }
    }
}