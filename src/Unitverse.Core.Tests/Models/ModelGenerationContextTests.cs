namespace Unitverse.Core.Tests.Models
{
    using Unitverse.Core.Models;
    using System;
    using FluentAssertions;
    using FakeItEasy;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Options;
    using NUnit.Framework;
    using Unitverse.Core.Helpers;

    [TestFixture]
    public class ModelGenerationContextTests
    {
        private ModelGenerationContext _testClass;
        private ClassModel _model;
        private IFrameworkSet _frameworkSet;
        private bool _withRegeneration;
        private NamingContext _baseNamingContext;
        private IMessageLogger _messageLogger;

        [SetUp]
        public void SetUp()
        {
            _model = ClassModelProvider.Instance;
            _frameworkSet = A.Fake<IFrameworkSet>();
            _withRegeneration = false;
            _baseNamingContext = new NamingContext("TestValue1956429780");
            _messageLogger = A.Fake<IMessageLogger>();
            _testClass = new ModelGenerationContext(_model, _frameworkSet, _withRegeneration, _baseNamingContext, _messageLogger);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new ModelGenerationContext(_model, _frameworkSet, _withRegeneration, _baseNamingContext, _messageLogger);

            // Assert
            instance.Should().NotBeNull();
        }

        [Test]
        public void CannotConstructWithNullModel()
        {
            FluentActions.Invoking(() => new ModelGenerationContext(default(ClassModel), A.Fake<IFrameworkSet>(), false, new NamingContext("TestValue205118814"), A.Fake<IMessageLogger>())).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotConstructWithNullFrameworkSet()
        {
            FluentActions.Invoking(() => new ModelGenerationContext(ClassModelProvider.Instance, default(IFrameworkSet), true, new NamingContext("TestValue1200384518"), A.Fake<IMessageLogger>())).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotConstructWithNullBaseNamingContext()
        {
            FluentActions.Invoking(() => new ModelGenerationContext(ClassModelProvider.Instance, A.Fake<IFrameworkSet>(), false, default(NamingContext), A.Fake<IMessageLogger>())).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void ModelIsInitializedCorrectly()
        {
            _testClass.Model.Should().BeSameAs(_model);
        }

        [Test]
        public void FrameworkSetIsInitializedCorrectly()
        {
            _testClass.FrameworkSet.Should().BeSameAs(_frameworkSet);
        }

        [Test]
        public void WithRegenerationIsInitializedCorrectly()
        {
            _testClass.WithRegeneration.Should().Be(_withRegeneration);
        }

        [Test]
        public void BaseNamingContextIsInitializedCorrectly()
        {
            _testClass.BaseNamingContext.Should().BeSameAs(_baseNamingContext);
        }

        [Test]
        public void MessageLoggerIsInitializedCorrectly()
        {
            _testClass.MessageLogger.Should().BeSameAs(_messageLogger);
        }

        [Test]
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