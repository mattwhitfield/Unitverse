namespace Unitverse.Core.Tests.Models
{
    using System;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;
    using Unitverse.Core.Models;
    using T = Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax;

    [TestFixture]
    public class TestableModelTests
    {
        private string _name;

        private T _node;

        private TestTestableModel _testClass;

        [Test]
        public void CanCallMutateName()
        {
            var newName = "TestValue1930814240";
            _testClass.MutateName(newName);
            Assert.That(_testClass.OriginalName, Is.EqualTo(_name));
            Assert.That(_testClass.Name, Is.EqualTo(newName));
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new TestTestableModel(_name, _node);
            Assert.That(instance, Is.Not.Null);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallMutateNameWithInvalidNewName(string value)
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.MutateName(value));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidName(string value)
        {
            Assert.Throws<ArgumentNullException>(() => new TestTestableModel(value, SyntaxFactory.ClassDeclaration("c")));
        }

        [Test]
        public void CannotConstructWithNullNode()
        {
            Assert.Throws<ArgumentNullException>(() => new TestTestableModel("TestValue1068014130", default(T)));
        }

        [Test]
        public void NameIsInitializedCorrectly()
        {
            Assert.That(_testClass.Name, Is.EqualTo(_name));
        }

        [Test]
        public void NodeIsInitializedCorrectly()
        {
            Assert.That(_testClass.Node, Is.EqualTo(_node));
        }

        [SetUp]
        public void SetUp()
        {
            _name = "TestValue2071358710";
            _node = SyntaxFactory.ClassDeclaration("c");
            _testClass = new TestTestableModel(_name, _node);
        }

        private class TestTestableModel : TestableModel<T>
        {
            public TestTestableModel(string name, T node)
                : base(name, node)
            {
            }
        }
    }
}