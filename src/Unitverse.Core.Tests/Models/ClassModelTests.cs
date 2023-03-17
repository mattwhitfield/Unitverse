namespace Unitverse.Core.Tests.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Assets;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using Unitverse.Tests.Common;

    [TestFixture]
    public class ClassModelTests
    {
        private ClassModel _testClass;
        private TypeDeclarationSyntax _declaration;
        private SemanticModel _semanticModel;
        private bool _isSingleItem;

        [SetUp]
        public void SetUp()
        {
            _declaration = TestSemanticModelFactory.Class;
            _semanticModel = TestSemanticModelFactory.Model;
            _isSingleItem = false;
            _testClass = new ClassModel(_declaration, _semanticModel, _isSingleItem);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new ClassModel(_declaration, _semanticModel, _isSingleItem);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullDeclaration()
        {
            Assert.Throws<ArgumentNullException>(() => new ClassModel(default, TestSemanticModelFactory.Model, true));
        }

        [Test]
        public void CannotConstructWithNullSemanticModel()
        {
            Assert.Throws<ArgumentNullException>(() => new ClassModel(TestSemanticModelFactory.Class, default, true));
        }

        [Test]
        public void CanCallGetConstructorParameterFieldName()
        {
            var parameter = new ParameterModel("param", TestSemanticModelFactory.Parameter, "int", default);
            var result = _testClass.GetConstructorParameterFieldName(parameter, DefaultFrameworkSet.Create());
            Assert.That(result, Is.EqualTo("_param"));
        }

        [Test]
        public void CanCallGetConstructorParameterFieldNameForMock()
        {
            var parameter = new ParameterModel("interfaceParam", TestSemanticModelFactory.InterfaceParameter, "ICloneable", _semanticModel.GetTypeInfo(TestSemanticModelFactory.InterfaceParameter.Type));
            var framework = DefaultFrameworkSet.CreateWithNamingOptions(x => x.MockDependencyFieldName = "_mockery{parameterName:pascal}");
            var result = _testClass.GetConstructorParameterFieldName(parameter, DefaultFrameworkSet.Create());
            var result2 = _testClass.GetConstructorParameterFieldName(parameter, framework);
            Assert.That(result, Is.EqualTo("_interfaceParam"));
            Assert.That(result2, Is.EqualTo("_mockeryInterfaceParam"));
        }

        [Test]
        public void CannotCallGetConstructorParameterFieldNameWithNullParameter()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.GetConstructorParameterFieldName(default(ParameterModel), DefaultFrameworkSet.Create()));
        }

        [Test]
        public void CannotCallGetConstructorParameterFieldNameWithNullNamingProvider()
        {
            var parameter = new ParameterModel("param", TestSemanticModelFactory.Parameter, "int", default);
            Assert.Throws<ArgumentNullException>(() => _testClass.GetConstructorParameterFieldName(parameter, null));
        }

        [Test]
        public void CanCallGetIndexerName()
        {
            var indexer = Substitute.For<IIndexerModel>();
            var result = _testClass.GetIndexerName(indexer);
            Assert.That(result, Is.EqualTo("Indexer"));
        }

        [Test]
        public void CannotCallGetIndexerNameWithNullIndexer()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.GetIndexerName(default));
        }

        [Test]
        public void CanCallGetObjectCreationExpression()
        {
            var frameworkSet = Substitute.For<IFrameworkSet>();
            var result = _testClass.GetObjectCreationExpression(frameworkSet, false);
            Assert.That(result.NormalizeWhitespace().ToFullString().StartsWith("new ModelSource(\"TestValue", StringComparison.InvariantCultureIgnoreCase));
        }

        [Test]
        public void CannotCallGetObjectCreationExpressionWithNullFrameworkSet()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.GetObjectCreationExpression(default, false));
        }

        [Test]
        public void CanGetTypeSymbol()
        {
            Assert.That(_testClass.TypeSymbol, Is.InstanceOf<INamedTypeSymbol>());
        }

        [Test]
        public void CanGetClassName()
        {
            Assert.That(_testClass.ClassName, Is.EqualTo("ModelSource"));
        }

        [Test]
        public void CanGetConstructors()
        {
            Assert.That(_testClass.Constructors, Is.InstanceOf<IList<IConstructorModel>>());
        }

        [Test]
        public void DeclarationIsInitializedCorrectly()
        {
            Assert.That(_testClass.Declaration, Is.EqualTo(_declaration));
        }

        [Test]
        public void CanSetAndGetDefaultConstructor()
        {
            var testValue = Substitute.For<IConstructorModel>();
            _testClass.DefaultConstructor = testValue;
            Assert.That(_testClass.DefaultConstructor, Is.EqualTo(testValue));
        }

        [Test]
        public void CanGetIndexers()
        {
            Assert.That(_testClass.Indexers, Is.InstanceOf<IList<IIndexerModel>>());
        }

        [Test]
        public void IsSingleItemIsInitializedCorrectly()
        {
            Assert.That(_testClass.IsSingleItem, Is.EqualTo(_isSingleItem));
        }

        [Test]
        public void CanGetIsStatic()
        {
            Assert.That(_testClass.IsStatic, Is.False);
        }

        [Test]
        public void CanGetMethods()
        {
            Assert.That(_testClass.Methods, Is.InstanceOf<IList<IMethodModel>>());
        }

        [Test]
        public void CanGetProperties()
        {
            Assert.That(_testClass.Properties, Is.InstanceOf<IList<IPropertyModel>>());
        }

        [Test]
        public void CanGetRequiredAssets()
        {
            Assert.That(_testClass.RequiredAssets, Is.InstanceOf<IList<TargetAsset>>());
        }

        [Test]
        public void SemanticModelIsInitializedCorrectly()
        {
            Assert.That(_testClass.SemanticModel, Is.EqualTo(_semanticModel));
        }

        [Test]
        public void CanSetAndGetTargetInstance()
        {
            var testValue = SyntaxFactory.IdentifierName("other");
            Assert.That(_testClass.TargetInstance, Is.InstanceOf<ExpressionSyntax>());
            _testClass.TargetInstance = testValue;
            Assert.That(_testClass.TargetInstance, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetTypeSyntax()
        {
            var testValue = SyntaxFactory.IdentifierName("TestThing");
            Assert.That(_testClass.TypeSyntax, Is.InstanceOf<TypeSyntax>());
            _testClass.TypeSyntax = testValue;
            Assert.That(_testClass.TypeSyntax, Is.EqualTo(testValue));
        }

        [Test]
        public void CanGetUsings()
        {
            Assert.That(_testClass.Usings, Is.InstanceOf<IList<UsingDirectiveSyntax>>());
        }

        [Test]
        public void CanGetInterfaces()
        {
            Assert.That(_testClass.Interfaces, Is.InstanceOf<IList<IInterfaceModel>>());
            Assert.That(_testClass.Interfaces.Count, Is.Zero);
        }
    }
}