namespace Unitverse.Core.Tests.Strategies.ClassLevelGeneration
{
    using Unitverse.Core.Strategies.ClassLevelGeneration;
    using System;
    using NUnit.Framework;
    using FluentAssertions;
    using NSubstitute;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using System.Linq;

    [TestFixture]
    public class StringPropertyCheckInitializerGenerationStrategyTests
    {
        private StringPropertyCheckInitializerGenerationStrategy _testClass;
        private IFrameworkSet _frameworkSet;

        [SetUp]
        public void SetUp()
        {
            _frameworkSet = Substitute.For<IFrameworkSet>();
            _testClass = new StringPropertyCheckInitializerGenerationStrategy(_frameworkSet);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new StringPropertyCheckInitializerGenerationStrategy(_frameworkSet);
            instance.Should().NotBeNull();
        }

        [Test]
        public void CannotConstructWithNullFrameworkSet()
        {
            FluentActions.Invoking(() => new StringPropertyCheckInitializerGenerationStrategy(default(IFrameworkSet))).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotCallCanHandleWithNullMethod()
        {
            FluentActions.Invoking(() => _testClass.CanHandle(default(ClassModel), ClassModelProvider.Instance)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotCallCanHandleWithNullModel()
        {
            FluentActions.Invoking(() => _testClass.CanHandle(ClassModelProvider.Instance, default(ClassModel))).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotCallCreateWithNullMethod()
        {
            FluentActions.Invoking(() => _testClass.Create(default(ClassModel), ClassModelProvider.Instance, new NamingContext("TestValue1676607627")).ToList()).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotCallCreateWithNullModel()
        {
            FluentActions.Invoking(() => _testClass.Create(ClassModelProvider.Instance, default(ClassModel), new NamingContext("TestValue263713840")).ToList()).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotCallCreateWithNullNamingContext()
        {
            FluentActions.Invoking(() => _testClass.Create(ClassModelProvider.Instance, ClassModelProvider.Instance, default(NamingContext)).ToList()).Should().Throw<ArgumentNullException>();
        }
    }
}