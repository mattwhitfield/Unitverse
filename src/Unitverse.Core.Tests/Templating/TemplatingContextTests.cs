namespace Unitverse.Core.Tests.Templating
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using Unitverse.Core.Templating;
    using Unitverse.Core.Templating.Model;

    [TestFixture]
    public class TemplatingContextTests
    {
        private TemplatingContext _testClass;
        private ModelGenerationContext _modelGenerationContext;
        private IList<ITemplate> _templates;

        [SetUp]
        public void SetUp()
        {
            _modelGenerationContext = new ModelGenerationContext(ClassModelProvider.Instance, Substitute.For<IFrameworkSet>(), true, new NamingContext("TestValue2133156538"), Substitute.For<IMessageLogger>());
            _templates = new[] { Substitute.For<ITemplate>(), Substitute.For<ITemplate>(), Substitute.For<ITemplate>() };
            _testClass = new TemplatingContext(_modelGenerationContext, _templates);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new TemplatingContext(_modelGenerationContext, _templates);

            // Assert
            instance.Should().NotBeNull();
        }

        [Test]
        public void CannotConstructWithNullModelGenerationContext()
        {
            FluentActions.Invoking(() => new TemplatingContext(default(ModelGenerationContext), new[] { Substitute.For<ITemplate>(), Substitute.For<ITemplate>(), Substitute.For<ITemplate>() })).Should().Throw<ArgumentNullException>().WithParameterName("modelGenerationContext");
        }

        [Test]
        public void CannotConstructWithNullTemplates()
        {
            FluentActions.Invoking(() => new TemplatingContext(new ModelGenerationContext(ClassModelProvider.Instance, Substitute.For<IFrameworkSet>(), false, new NamingContext("TestValue1909171961"), Substitute.For<IMessageLogger>()), default(IList<ITemplate>))).Should().Throw<ArgumentNullException>().WithParameterName("templates");
        }

        [Test]
        public void CanCallForConstructors()
        {
            // Act
            var result = _testClass.ForConstructors();

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallForMethods()
        {
            // Act
            var result = _testClass.ForMethods();

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallForProperties()
        {
            // Act
            var result = _testClass.ForProperties();

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void ModelGenerationContextIsInitializedCorrectly()
        {
            _testClass.ModelGenerationContext.Should().BeSameAs(_modelGenerationContext);
        }

        [Test]
        public void TemplatesIsInitializedCorrectly()
        {
            _testClass.Templates.Should().BeSameAs(_templates);
        }

        [Test]
        public void CanGetClassModel()
        {
            // Assert
            _testClass.ClassModel.Should().BeAssignableTo<IOwningType>();

            Assert.Fail("Create or modify test");
        }
    }
}