namespace Unitverse.Core.Tests.Helpers
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;
    using Unitverse.Core.Helpers;

    [TestFixture]
    public class TestableItemExtractorTests
    {
        private TestableItemExtractor _testClass;
        private SyntaxTree _tree;
        private SemanticModel _semanticModel;

        [SetUp]
        public void SetUp()
        {
            _tree = TestSemanticModelFactory.Tree;
            _semanticModel = TestSemanticModelFactory.Model;
            _testClass = new TestableItemExtractor(_tree, _semanticModel);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new TestableItemExtractor(_tree, _semanticModel);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullTree()
        {
            Assert.Throws<ArgumentNullException>(() => new TestableItemExtractor(default(SyntaxTree), _semanticModel));
        }

        [Test]
        public void CannotConstructWithNullSemanticModel()
        {
            Assert.Throws<ArgumentNullException>(() => new TestableItemExtractor(_tree, default(SemanticModel)));
        }

        [Test]
        public void CanCallExtract()
        {
            var result = _testClass.Extract(null).Single();
            Assert.That(result.Constructors.Count, Is.EqualTo(1));
            Assert.That(result.Methods.Count, Is.EqualTo(2));
            Assert.That(result.Indexers.Count, Is.EqualTo(1));
            Assert.That(result.Properties.Count, Is.EqualTo(1));
        }

        [Test]
        public void CanCallExtractWithMethodSymbol()
        {
            var result = _testClass.Extract(TestSemanticModelFactory.Method).Single();
            Assert.That(result.Constructors.Count(x => x.ShouldGenerate), Is.EqualTo(0));
            Assert.That(result.Methods.Count(x => x.ShouldGenerate), Is.EqualTo(1));
            Assert.That(result.Indexers.Count(x => x.ShouldGenerate), Is.EqualTo(0));
            Assert.That(result.Properties.Count(x => x.ShouldGenerate), Is.EqualTo(0));
        }

        [Test]
        public void CanCallExtractWithConstructorSymbol()
        {
            var result = _testClass.Extract(TestSemanticModelFactory.Constructor).Single();
            Assert.That(result.Constructors.Count, Is.EqualTo(1));
            Assert.That(result.Methods.Count, Is.EqualTo(2));
            Assert.That(result.Indexers.Count, Is.EqualTo(1));
            Assert.That(result.Properties.Count, Is.EqualTo(1));
            Assert.That(result.Constructors.Count(x => x.ShouldGenerate), Is.EqualTo(1));
            Assert.That(result.Methods.Count(x => x.ShouldGenerate), Is.EqualTo(0));
            Assert.That(result.Indexers.Count(x => x.ShouldGenerate), Is.EqualTo(0));
            Assert.That(result.Properties.Count(x => x.ShouldGenerate), Is.EqualTo(0));
        }

        [Test]
        public void CanCallExtractWithPropertySymbol()
        {
            var result = _testClass.Extract(TestSemanticModelFactory.Property).Single();
            Assert.That(result.Constructors.Count, Is.EqualTo(1));
            Assert.That(result.Methods.Count, Is.EqualTo(2));
            Assert.That(result.Indexers.Count, Is.EqualTo(1));
            Assert.That(result.Properties.Count, Is.EqualTo(1));
            Assert.That(result.Constructors.Count(x => x.ShouldGenerate), Is.EqualTo(0));
            Assert.That(result.Methods.Count(x => x.ShouldGenerate), Is.EqualTo(0));
            Assert.That(result.Indexers.Count(x => x.ShouldGenerate), Is.EqualTo(0));
            Assert.That(result.Properties.Count(x => x.ShouldGenerate), Is.EqualTo(1));
        }

        [Test]
        public void CanCallExtractWithIndexerSymbol()
        {
            var result = _testClass.Extract(TestSemanticModelFactory.Indexer).Single();
            Assert.That(result.Constructors.Count, Is.EqualTo(1));
            Assert.That(result.Methods.Count, Is.EqualTo(2));
            Assert.That(result.Indexers.Count, Is.EqualTo(1));
            Assert.That(result.Properties.Count, Is.EqualTo(1));
            Assert.That(result.Constructors.Count(x => x.ShouldGenerate), Is.EqualTo(0));
            Assert.That(result.Methods.Count(x => x.ShouldGenerate), Is.EqualTo(0));
            Assert.That(result.Indexers.Count(x => x.ShouldGenerate), Is.EqualTo(1));
            Assert.That(result.Properties.Count(x => x.ShouldGenerate), Is.EqualTo(0));
        }

        [Test]
        public void CanCallExtractWithTypeSymbol()
        {
            var result = _testClass.Extract(TestSemanticModelFactory.Class).Single();
            Assert.That(result.Constructors.Count, Is.EqualTo(1));
            Assert.That(result.Methods.Count, Is.EqualTo(2));
            Assert.That(result.Indexers.Count, Is.EqualTo(1));
            Assert.That(result.Properties.Count, Is.EqualTo(1));
            Assert.That(result.Constructors.Count(x => x.ShouldGenerate), Is.EqualTo(1));
            Assert.That(result.Methods.Count(x => x.ShouldGenerate), Is.EqualTo(2));
            Assert.That(result.Indexers.Count(x => x.ShouldGenerate), Is.EqualTo(1));
            Assert.That(result.Properties.Count(x => x.ShouldGenerate), Is.EqualTo(1));
        }

        [Test]
        public void CanCallGetTypeDeclarations()
        {
            var root = TestSemanticModelFactory.Tree.GetRoot();
            var result = TestableItemExtractor.GetTypeDeclarations(root);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].GetClassName(), Is.EqualTo("ModelSource"));
        }

        [Test]
        public void CannotCallGetTypeDeclarationsWithNullRoot()
        {
            Assert.Throws<ArgumentNullException>(() => TestableItemExtractor.GetTypeDeclarations(default(SyntaxNode)));
        }
    }
}