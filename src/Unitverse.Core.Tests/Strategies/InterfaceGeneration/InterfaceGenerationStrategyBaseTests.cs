namespace Unitverse.Core.Tests.Strategies.InterfaceGeneration
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using Unitverse.Core.Strategies.InterfaceGeneration;

    [TestFixture]
    public class InterfaceGenerationStrategyBaseTests
    {
        private class TestInterfaceGenerationStrategyBase : InterfaceGenerationStrategyBase
        {
            public TestInterfaceGenerationStrategyBase(IFrameworkSet frameworkSet)
                : base(frameworkSet, "SomeThing")
            {
            }

            public IFrameworkSet PublicFrameworkSet => FrameworkSet;

            public NameResolver PublicGeneratedMethodNamePattern => GeneratedMethodNamePattern;

            protected override IEnumerable<StatementSyntax> GetBodyStatements(ClassModel sourceModel, IInterfaceModel interfaceModel)
            {
                yield break;
            }

            protected override NameResolver GeneratedMethodNamePattern { get; }
        }

        private TestInterfaceGenerationStrategyBase _testClass;
        private IFrameworkSet _frameworkSet;

        [SetUp]
        public void SetUp()
        {
            _frameworkSet = Substitute.For<IFrameworkSet>();
            _testClass = new TestInterfaceGenerationStrategyBase(_frameworkSet);
        }

        [Test]
        public void CannotConstructWithNullFrameworkSet()
        {
            Assert.Throws<ArgumentNullException>(() => new TestInterfaceGenerationStrategyBase(default(IFrameworkSet)));
        }

        [Test]
        public void CannotCallCanHandleWithNullClassModel()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.CanHandle(default(ClassModel), ClassModelProvider.Instance));
        }

        [Test]
        public void CannotCallCanHandleWithNullModel()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.CanHandle(ClassModelProvider.Instance, default(ClassModel)));
        }

        [Test]
        public void CanGetIsExclusive()
        {
            Assert.That(_testClass.IsExclusive, Is.EqualTo(false));
        }

        [Test]
        public void CanGetPriority()
        {
            Assert.That(_testClass.Priority, Is.EqualTo(2));
        }

        [Test]
        public void CanGetPublicFrameworkSet()
        {
            Assert.That(_testClass.PublicFrameworkSet, Is.InstanceOf<IFrameworkSet>());
        }
    }
}