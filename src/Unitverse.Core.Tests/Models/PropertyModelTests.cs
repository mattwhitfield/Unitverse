namespace Unitverse.Core.Tests.Models
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Models;

    [TestFixture]
    public class PropertyModelTests
    {
        private PropertyModel _testClass;
        private string _name;
        private PropertyDeclarationSyntax _node;
        private TypeInfo _typeInfo;

        [SetUp]
        public void SetUp()
        {
            _name = "TestValue1619496715";
            _node = TestSemanticModelFactory.Property;
            _typeInfo = default(TypeInfo);
            _testClass = new PropertyModel(_name, _node, _typeInfo, Substitute.For<SemanticModel>());
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new PropertyModel(_name, _node, _typeInfo, Substitute.For<SemanticModel>());
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullNode()
        {
            Assert.Throws<ArgumentNullException>(() => new PropertyModel("TestValue1565919566", default(PropertyDeclarationSyntax), default(TypeInfo), Substitute.For<SemanticModel>()));
        }

        [Test]
        public void CannotConstructWithNullSemanticModel()
        {
            Assert.Throws<ArgumentNullException>(() => new PropertyModel("TestValue1565919566", SyntaxFactory.PropertyDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), "name"), default(TypeInfo), null));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidName(string value)
        {
            Assert.Throws<ArgumentNullException>(() => new PropertyModel(value, SyntaxFactory.PropertyDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), "name"), default(TypeInfo), Substitute.For<SemanticModel>()));
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
        }
    }
}