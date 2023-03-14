namespace Unitverse.Core.Tests.Templating
{
    using System.Collections.Generic;
    using AutoFixture;
    using AutoFixture.AutoMoq;
    using FluentAssertions;
    using NUnit.Framework;
    using SequelFilter;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Options;
    using Unitverse.Core.Templating;
    using Unitverse.Core.Templating.Model;

    [TestFixture]
    public class TemplateTests
    {
        private Template _testClass;
        private string _content;
        private string _testMethodName;
        private string _target;
        private IList<ExecutableExpression> _includeExpressions;
        private IList<ExecutableExpression> _excludeExpressions;
        private bool _isExclusive;
        private bool _stopMatching;
        private int _priority;
        private bool _isAsync;
        private bool _isStatic;
        private string _description;

        [SetUp]
        public void SetUp()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            _content = fixture.Create<string>();
            _testMethodName = fixture.Create<string>();
            _target = fixture.Create<string>();
            _includeExpressions = fixture.Create<IList<ExecutableExpression>>();
            _excludeExpressions = fixture.Create<IList<ExecutableExpression>>();
            _isExclusive = fixture.Create<bool>();
            _stopMatching = fixture.Create<bool>();
            _priority = fixture.Create<int>();
            _isAsync = fixture.Create<bool>();
            _isStatic = fixture.Create<bool>();
            _description = fixture.Create<string>();
            _testClass = fixture.Create<Template>();
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new Template(_content, _testMethodName, _target, _includeExpressions, _excludeExpressions, _isExclusive, _stopMatching, _priority, _isAsync, _isStatic, _description);

            // Assert
            instance.Should().NotBeNull();
        }

        [Test]
        public void CanCallAppliesTo()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var model = fixture.Create<ITemplateTarget>();
            var owningType = fixture.Create<IClass>();

            // Act
            var result = _testClass.AppliesTo(model, owningType);

            // Assert
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallCreate()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var frameworkSet = fixture.Create<IFrameworkSet>();
            var model = fixture.Create<ITemplateTarget>();
            var owningType = fixture.Create<IClass>();
            var namingContext = fixture.Create<NamingContext>();

            // Act
            var result = _testClass.Create(frameworkSet, model, owningType, namingContext);

            // Assert
            Assert.Fail("Create or modify test");
        }
    }
}