namespace Unitverse.Core.Tests.Models
{
    using Unitverse.Core.Models;
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;
    using FakeItEasy;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Options;

    [TestClass]
    public class ModelGenerationContextTests
    {
        private ModelGenerationContext _testClass;
        private ClassModel _model;
        private IFrameworkSet _frameworkSet;
        private bool _withRegeneration;
        private bool _partialGenerationAllowed;
        private NamingContext _baseNamingContext;

        [TestInitialize]
        public void SetUp()
        {
            _model = ClassModelProvider.Instance;
            _frameworkSet = A.Fake<IFrameworkSet>();
            _withRegeneration = false;
            _partialGenerationAllowed = true;
            _baseNamingContext = new NamingContext("TestValue1956429780");
            _testClass = new ModelGenerationContext(_model, _frameworkSet, _withRegeneration, _partialGenerationAllowed, _baseNamingContext);
        }

        [TestMethod]
        public void CanConstruct()
        {
            // Act
            var instance = new ModelGenerationContext(_model, _frameworkSet, _withRegeneration, _partialGenerationAllowed, _baseNamingContext);

            // Assert
            instance.Should().NotBeNull();
        }

        [TestMethod]
        public void CannotConstructWithNullModel()
        {
            FluentActions.Invoking(() => new ModelGenerationContext(default(ClassModel), A.Fake<IFrameworkSet>(), false, false, new NamingContext("TestValue205118814"))).Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CannotConstructWithNullFrameworkSet()
        {
            FluentActions.Invoking(() => new ModelGenerationContext(ClassModelProvider.Instance, default(IFrameworkSet), true, true, new NamingContext("TestValue1200384518"))).Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CannotConstructWithNullBaseNamingContext()
        {
            FluentActions.Invoking(() => new ModelGenerationContext(ClassModelProvider.Instance, A.Fake<IFrameworkSet>(), false, true, default(NamingContext))).Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ModelIsInitializedCorrectly()
        {
            _testClass.Model.Should().BeSameAs(_model);
        }

        [TestMethod]
        public void FrameworkSetIsInitializedCorrectly()
        {
            _testClass.FrameworkSet.Should().BeSameAs(_frameworkSet);
        }

        [TestMethod]
        public void WithRegenerationIsInitializedCorrectly()
        {
            _testClass.WithRegeneration.Should().Be(_withRegeneration);
        }

        [TestMethod]
        public void PartialGenerationAllowedIsInitializedCorrectly()
        {
            _testClass.PartialGenerationAllowed.Should().Be(_partialGenerationAllowed);
        }

        [TestMethod]
        public void BaseNamingContextIsInitializedCorrectly()
        {
            _testClass.BaseNamingContext.Should().BeSameAs(_baseNamingContext);
        }

        [TestMethod]
        public void CanSetAndGetMethodsEmitted()
        {
            // Arrange
            var testValue = 1312949164;

            // Act
            _testClass.MethodsEmitted = testValue;

            // Assert
            _testClass.MethodsEmitted.Should().Be(testValue);
        }
    }
}