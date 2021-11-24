namespace Unitverse.Core.Tests.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Models;

    [TestFixture]
    public class MethodModelTests
    {
        private MethodModel _testClass;
        private string _name;
        private List<ParameterModel> _parameters;
        private MethodDeclarationSyntax _node;
        private SemanticModel _model;

        [SetUp]
        public void SetUp()
        {
            _name = "TestValue1917291340";
            _parameters = new List<ParameterModel>();
            _node = TestSemanticModelFactory.Method;
            _model = TestSemanticModelFactory.Model;
            _testClass = new MethodModel(_name, _parameters, _node, _model);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new MethodModel(_name, _parameters, _node, _model);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CanConstructWithNullParameters()
        {
            Assert.That(new MethodModel("TestValue24901811", default(List<ParameterModel>), TestSemanticModelFactory.Method, TestSemanticModelFactory.Model).Parameters, Is.InstanceOf<IList<ParameterModel>>());
        }

        [Test]
        public void CannotConstructWithNullNode()
        {
            Assert.Throws<ArgumentNullException>(() => new MethodModel("TestValue227068250", new List<ParameterModel>(), default(MethodDeclarationSyntax), TestSemanticModelFactory.Model));
        }

        [Test]
        public void CannotConstructWithNullModel()
        {
            Assert.Throws<ArgumentNullException>(() => new MethodModel("TestValue1279475931", new List<ParameterModel>(), SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), "someMethod"), default(SemanticModel)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidName(string value)
        {
            Assert.Throws<ArgumentNullException>(() => new MethodModel(value, new List<ParameterModel>(), TestSemanticModelFactory.Method, TestSemanticModelFactory.Model));
        }

        [Test]
        public void CanCallInvoke()
        {
            var owner = new ClassModel(TestSemanticModelFactory.Class, TestSemanticModelFactory.Model, false);
            var result = _testClass.Invoke(owner, false, Substitute.For<IFrameworkSet>());
            Assert.That(result.ToFullString(), Is.EqualTo("_testClass.Method()"));
            _testClass.MutateName("fred");
            result = _testClass.Invoke(owner, false, Substitute.For<IFrameworkSet>());
            Assert.That(result.ToFullString(), Is.EqualTo("_testClass.fred()"));
        }

        [Test]
        public void CannotCallInvokeWithNullOwner()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Invoke(default(ClassModel), false, Substitute.For<IFrameworkSet>(), default(ExpressionSyntax[])));
        }

        [Test]
        public void CannotCallInvokeWithNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Invoke(new ClassModel(TestSemanticModelFactory.Class, TestSemanticModelFactory.Model, false), true, Substitute.For<IFrameworkSet>(), default(ExpressionSyntax[])));
        }

        [Test]
        public void CannotCallInvokeWithNullFrameworkSet()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Invoke(new ClassModel(TestSemanticModelFactory.Class, TestSemanticModelFactory.Model, false), true, null));
        }

        [Test]
        public void ParametersIsInitializedCorrectly()
        {
            Assert.That(_testClass.Parameters, Is.EqualTo(_parameters));
        }

        [Test]
        public void CanGetIsAsync()
        {
            Assert.That(_testClass.IsAsync, Is.False);
        }

        [Test]
        public void CanGetIsVoid()
        {
            Assert.That(_testClass.IsVoid, Is.False);
        }
    }
}