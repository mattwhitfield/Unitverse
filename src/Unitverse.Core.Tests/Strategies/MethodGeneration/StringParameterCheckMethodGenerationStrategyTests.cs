namespace Unitverse.Core.Tests.Strategies.MethodGeneration
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using Unitverse.Core.Strategies.MethodGeneration;

    [TestFixture]
    public class StringParameterCheckMethodGenerationStrategyTests
    {
        private StringParameterCheckMethodGenerationStrategy _testClass;
        private IFrameworkSet _frameworkSet;

        [SetUp]
        public void SetUp()
        {
            _frameworkSet = Substitute.For<IFrameworkSet>();
            _testClass = new StringParameterCheckMethodGenerationStrategy(_frameworkSet);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new StringParameterCheckMethodGenerationStrategy(_frameworkSet);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullFrameworkSet()
        {
            Assert.Throws<ArgumentNullException>(() => new StringParameterCheckMethodGenerationStrategy(default(IFrameworkSet)));
        }

        [Test]
        public void CannotCallCanHandleWithNullMethod()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.CanHandle(default(IMethodModel), ClassModelProvider.Instance));
        }

        [Test]
        public void CannotCallCanHandleWithNullModel()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.CanHandle(Substitute.For<IMethodModel>(), default(ClassModel)));
        }

        [Test]
        public void CannotCallCreateWithNullMethod()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Create(default(IMethodModel), ClassModelProvider.Instance, new NamingContext("class")).Consume());
        }

        [Test]
        public void CannotCallCreateWithNullModel()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Create(Substitute.For<IMethodModel>(), default(ClassModel), new NamingContext("class")).Consume());
        }

        [Test]
        public void CanGetIsExclusive()
        {
            Assert.That(_testClass.IsExclusive, Is.EqualTo(false));
        }

        [Test]
        public void CanGetPriority()
        {
            Assert.That(_testClass.Priority, Is.EqualTo(1));
        }
    }
}