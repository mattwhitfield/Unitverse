namespace Unitverse.Core.Tests.Templating
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using Unitverse.Core.Templating;
    using Unitverse.Core.Templating.Model;

    [TestFixture]
    public class SpecificTemplatingContextTests
    {
        private SpecificTemplatingContext _testClass;
        private ModelGenerationContext _modelGenerationContext;
        private IList<ITemplate> _templates;
        private IOwningType _classModel;
        private IEnumerable<ITemplateTarget> _targets;

        [SetUp]
        public void SetUp()
        {
            _modelGenerationContext = new ModelGenerationContext(ClassModelProvider.Instance, Substitute.For<IFrameworkSet>(), false, new NamingContext("TestValue565412182"), Substitute.For<IMessageLogger>());
            _templates = new[] { Substitute.For<ITemplate>(), Substitute.For<ITemplate>(), Substitute.For<ITemplate>() };
            _classModel = Substitute.For<IOwningType>();
            _targets = new[] { Substitute.For<ITemplateTarget>(), Substitute.For<ITemplateTarget>(), Substitute.For<ITemplateTarget>() };
            _testClass = new SpecificTemplatingContext(_modelGenerationContext, _templates, _classModel, _targets);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new SpecificTemplatingContext(_modelGenerationContext, _templates, _classModel, _targets);

            // Assert
            instance.Should().NotBeNull();
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
        public void ClassModelIsInitializedCorrectly()
        {
            _testClass.ClassModel.Should().BeSameAs(_classModel);
        }

        [Test]
        public void TargetsIsInitializedCorrectly()
        {
            _testClass.Targets.Should().BeEquivalentTo(_targets);
        }
    }
}