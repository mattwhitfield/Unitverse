namespace SentryOne.UnitTestGenerator.Core.Tests.Models
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Models;

    [TestFixture]
    public class ParameterModelTests
    {
        private ParameterModel _testClass;
        private string _name;
        private ParameterSyntax _node;
        private string _type;
        private TypeInfo _typeInfo;

        [SetUp]
        public void SetUp()
        {
            _name = "TestValue308762189";
            _node = SyntaxFactory.Parameter(SyntaxFactory.Identifier("name"));
            _type = "TestValue1468504349";
            _typeInfo = default(TypeInfo);
            _testClass = new ParameterModel(_name, _node, _type, _typeInfo);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new ParameterModel(_name, _node, _type, _typeInfo);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullNode()
        {
            Assert.Throws<ArgumentNullException>(() => new ParameterModel("TestValue1595240331", default(ParameterSyntax), "TestValue1362881784", default(TypeInfo)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidName(string value)
        {
            Assert.Throws<ArgumentNullException>(() => new ParameterModel(value, SyntaxFactory.Parameter(SyntaxFactory.Identifier("name")), "TestValue562013549", default(TypeInfo)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidType(string value)
        {
            Assert.Throws<ArgumentNullException>(() => new ParameterModel("TestValue772323883", SyntaxFactory.Parameter(SyntaxFactory.Identifier("name")), value, default(TypeInfo)));
        }

        [Test]
        public void TypeIsInitializedCorrectly()
        {
            Assert.That(_testClass.Type, Is.EqualTo(_type));
        }

        [Test]
        public void TypeInfoIsInitializedCorrectly()
        {
            Assert.That(_testClass.TypeInfo, Is.EqualTo(_typeInfo));
        }
    }
}