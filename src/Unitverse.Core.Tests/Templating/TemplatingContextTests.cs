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
    using Unitverse.Core.Templating.Model.Implementation;

    [TestFixture]
    public class TemplatingContextTests
    {
        private TemplatingContext _testClass;
        private ModelGenerationContext _modelGenerationContext;
        private IList<ITemplate> _templates;
        private ITemplate _constructorTemplate1;
        private ITemplate _constructorTemplate2;
        private ITemplate _methodTemplate1;
        private ITemplate _methodTemplate2;
        private ITemplate _propertyTemplate1;
        private ITemplate _propertyTemplate2;

        private static ITemplate GetTemplate(string target)
        {
            var template = Substitute.For<ITemplate>();
            template.Target.Returns(target);
            return template;
        }

        [SetUp]
        public void SetUp()
        {
            _modelGenerationContext = new ModelGenerationContext(ClassModelProvider.Instance, Substitute.For<IFrameworkSet>(), true, new NamingContext("TestValue2133156538"), Substitute.For<IMessageLogger>());
            _constructorTemplate1 = GetTemplate(ConstructorFilterModel.Target);
            _constructorTemplate2 = GetTemplate(ConstructorFilterModel.Target);
            _methodTemplate1 = GetTemplate(MethodFilterModel.Target);
            _methodTemplate2 = GetTemplate(MethodFilterModel.Target);
            _propertyTemplate1 = GetTemplate(PropertyFilterModel.Target);
            _propertyTemplate2 = GetTemplate(PropertyFilterModel.Target);
            _templates = new[] { _constructorTemplate1, _methodTemplate1, _propertyTemplate1, _constructorTemplate2, _methodTemplate2, _propertyTemplate2 };
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
        public void CanCallForConstructors()
        {
            // Act
            var result = _testClass.ForConstructors();

            // Assert
            result.Templates.Should().BeEquivalentTo(new[] { _constructorTemplate1, _constructorTemplate2 });
        }

        [Test]
        public void CanCallForMethods()
        {
            // Act
            var result = _testClass.ForMethods();

            // Assert
            result.Templates.Should().BeEquivalentTo(new[] { _methodTemplate1, _methodTemplate2 });
        }

        [Test]
        public void CanCallForProperties()
        {
            // Act
            var result = _testClass.ForProperties();

            // Assert
            result.Templates.Should().BeEquivalentTo(new[] { _propertyTemplate1, _propertyTemplate2 });
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
        }
    }
}