namespace SentryOne.UnitTestGenerator.Core.Tests.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Models;

    [TestFixture]
    public class IndexerModelTests
    {
        private IndexerModel _testClass;
        private string _name;
        private List<ParameterModel> _parameters;
        private TypeInfo _typeInfo;
        private IndexerDeclarationSyntax _node;

        [SetUp]
        public void SetUp()
        {
            _name = "TestValue1050025500";
            _parameters = new List<ParameterModel>();
            _typeInfo = default(TypeInfo);
            _node = TestSemanticModelFactory.Indexer;
            _testClass = new IndexerModel(_name, _parameters, _typeInfo, _node);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new IndexerModel(_name, _parameters, _typeInfo, _node);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CanConstructWithNullParameters()
        {
            Assert.That(new IndexerModel("TestValue747644282", default(List<ParameterModel>), default(TypeInfo), TestSemanticModelFactory.Indexer).Parameters, Is.InstanceOf<IList<ParameterModel>>());
        }

        [Test]
        public void CannotConstructWithNullNode()
        {
            Assert.Throws<ArgumentNullException>(() => new IndexerModel("TestValue1247954846", new List<ParameterModel>(), default(TypeInfo), default(IndexerDeclarationSyntax)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidName(string value)
        {
            Assert.Throws<ArgumentNullException>(() => new IndexerModel(value, new List<ParameterModel>(), default(TypeInfo), TestSemanticModelFactory.Indexer));
        }

        [Test]
        public void ParametersIsInitializedCorrectly()
        {
            Assert.That(_testClass.Parameters, Is.EqualTo(_parameters));
        }

        [Test]
        public void TypeInfoIsInitializedCorrectly()
        {
            Assert.That(_testClass.TypeInfo, Is.EqualTo(_typeInfo));
        }

        [Test]
        public void CanGetHasGet()
        {
            Assert.That(_testClass.HasGet, Is.True);
        }

        [Test]
        public void CanGetHasSet()
        {
            Assert.That(_testClass.HasSet, Is.False);
            var node = _node.WithAccessorList(SyntaxFactory.AccessorList(SyntaxFactory.SingletonList(SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration))));
            _testClass = new IndexerModel(_name, _parameters, _typeInfo, node);
            Assert.That(_testClass.HasSet, Is.True);
        }
    }
}