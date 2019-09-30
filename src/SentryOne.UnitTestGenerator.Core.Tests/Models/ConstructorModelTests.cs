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
    public class ConstructorModelTests
    {
        private ConstructorModel _testClass;
        private string _name;
        private IList<ParameterModel> _parameters;
        private ConstructorDeclarationSyntax _node;

        [SetUp]
        public void SetUp()
        {
            _name = "TestValue1471544744";
            _parameters = new[] { new ParameterModel("TestValue823457729", SyntaxFactory.Parameter(SyntaxFactory.Identifier("p1")), TestSemanticModelFactory.Parameter.Type.ToFullString(), TestSemanticModelFactory.Model.GetTypeInfo(TestSemanticModelFactory.Parameter)) };
            _node = TestSemanticModelFactory.Constructor;
            _testClass = new ConstructorModel(_name, _parameters, _node);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new ConstructorModel(_name, _parameters, _node);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CanConstructWithNullParameters()
        {
            Assert.That(new ConstructorModel("TestValue743567324", default(IList<ParameterModel>), SyntaxFactory.ConstructorDeclaration("crt")).Parameters, Is.InstanceOf<IList<ParameterModel>>());
        }

        [Test]
        public void CannotConstructWithNullNode()
        {
            Assert.Throws<ArgumentNullException>(() => new ConstructorModel("TestValue1015047371", new[] { new ParameterModel("TestValue823457729", SyntaxFactory.Parameter(SyntaxFactory.Identifier("p1")), TestSemanticModelFactory.Parameter.Type.ToFullString(), TestSemanticModelFactory.Model.GetTypeInfo(TestSemanticModelFactory.Parameter)) }, default(ConstructorDeclarationSyntax)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidName(string value)
        {
            Assert.Throws<ArgumentNullException>(() => new ConstructorModel(value, new[] { new ParameterModel("TestValue823457729", SyntaxFactory.Parameter(SyntaxFactory.Identifier("p1")), TestSemanticModelFactory.Parameter.Type.ToFullString(), TestSemanticModelFactory.Model.GetTypeInfo(TestSemanticModelFactory.Parameter)) }, SyntaxFactory.ConstructorDeclaration("crt")));
        }

        [Test]
        public void ParametersIsInitializedCorrectly()
        {
            Assert.That(_testClass.Parameters, Is.EqualTo(_parameters));
        }
    }
}