namespace Unitverse.Core.Tests.Templating
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;
    using SequelFilter;
    using Unitverse.Core.Options;
    using Unitverse.Core.Templating;
    using Unitverse.Core.Templating.Model.Implementation;
    using Unitverse.Core.Tests.Templating.ModelIntegration;
    using Unitverse.Tests.Common;
    using Microsoft.CodeAnalysis.Formatting;

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
            _content = "string s = string.Empty;";
            _testMethodName = "testname";
            _target = "Property";
            _includeExpressions = new[] { SequelFilterParser.Parse("owningType.Type.Name == 'TestClass' && model.Name == 'ThisIsAReadWriteString'") };
            _excludeExpressions = new List<ExecutableExpression>();
            _isExclusive = false;
            _stopMatching = false;
            _priority = 1;
            _isAsync = false;
            _isStatic = false;
            _description = "description of the test method";
            _testClass = new Template(_content, _testMethodName, _target, _includeExpressions, _excludeExpressions, _isExclusive, _stopMatching, _priority, _isAsync, _isStatic, _description);
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
            var model = ClassModelProvider.CreateModel(TemplateModelSources.TS_SampleClass);
            var owningType = new OwningTypeFilterModel(model);

            // Act
            var result = owningType.Properties.FirstOrDefault(x => _testClass.AppliesTo(x, owningType));

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("ThisIsAReadWriteString");
        }

        [Test]
        public void AppliesToWillNotMatchIncorrectModelTypes()
        {
            _testClass = new Template(_content, _testMethodName, _target, new List<ExecutableExpression>(), _excludeExpressions, _isExclusive, _stopMatching, _priority, _isAsync, _isStatic, _description);

            var model = ClassModelProvider.CreateModel(TemplateModelSources.TS_SampleClass);
            var owningType = new OwningTypeFilterModel(model);

            // Act
            var result = owningType.Methods.FirstOrDefault(x => _testClass.AppliesTo(x, owningType));

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void AppliesToWillNotMatchExcludedItems()
        {
            var includes = new[] { SequelFilterParser.Parse("model.Name == 'ThisIsAReadWriteString'") };
            var excludes = new[] { SequelFilterParser.Parse("owningType.Type.Name == 'TestClass'") };

            _testClass = new Template(_content, _testMethodName, _target, includes, excludes, _isExclusive, _stopMatching, _priority, _isAsync, _isStatic, _description);

            var model = ClassModelProvider.CreateModel(TemplateModelSources.TS_SampleClass);
            var owningType = new OwningTypeFilterModel(model);

            // Act
            var result = owningType.Properties.FirstOrDefault(x => _testClass.AppliesTo(x, owningType));

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void CanCallCreate()
        {
            // Arrange
            var frameworkSet = DefaultFrameworkSet.Create();

            var model = ClassModelProvider.CreateModel(TemplateModelSources.TS_SampleClass);
            var owningType = new OwningTypeFilterModel(model);
            var property = owningType.Properties.FirstOrDefault(x => x.Name == "ThisIsAReadWriteString");

            var namingContext = new NamingContext("TestClass");

            // Act
            var result = _testClass.Create(frameworkSet, property, owningType, namingContext);

            // Assert
            using (var workspace = new AdhocWorkspace())
            {
                var node = Formatter.Format(result, workspace).NormalizeWhitespace();

                node.ToFullString().Should().Be("[Fact]\r\npublic void testname()\r\n{\r\n    string s = string.Empty;\r\n}");
            }
        }
    }
}