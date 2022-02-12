namespace Unitverse.Core.Tests.Strategies.InterfaceGeneration
{
    using Unitverse.Core.Strategies.InterfaceGeneration;
    using System;
    using NUnit.Framework;
    using FluentAssertions;
    using NSubstitute;
    using Unitverse.Core.Models;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Options;
    using Unitverse.Core.Frameworks;
    using System.Linq;

    [TestFixture]
    public class EquatableGenerationStrategyTests
    {
        private class TestEquatableGenerationStrategy : EquatableGenerationStrategy
        {
            public TestEquatableGenerationStrategy(IFrameworkSet frameworkSet) : base(frameworkSet)
            {
            }

            public IEnumerable<StatementSyntax> PublicGetBodyStatements(ClassModel sourceModel, IInterfaceModel interfaceModel)
            {
                return base.GetBodyStatements(sourceModel, interfaceModel);
            }

            public NameResolver PublicGeneratedMethodNamePattern => base.GeneratedMethodNamePattern;
        }

        private TestEquatableGenerationStrategy _testClass;
        private IFrameworkSet _frameworkSet;

        [SetUp]
        public void SetUp()
        {
            _frameworkSet = Substitute.For<IFrameworkSet>();
            _testClass = new TestEquatableGenerationStrategy(_frameworkSet);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new TestEquatableGenerationStrategy(_frameworkSet);
            instance.Should().NotBeNull();
        }

        [Test]
        public void CannotConstructWithNullFrameworkSet()
        {
            FluentActions.Invoking(() => new TestEquatableGenerationStrategy(default(IFrameworkSet))).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotCallGetBodyStatementsWithNullSourceModel()
        {
            FluentActions.Invoking(() => _testClass.PublicGetBodyStatements(default(ClassModel), Substitute.For<IInterfaceModel>()).ToList()).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotCallGetBodyStatementsWithNullInterfaceModel()
        {
            FluentActions.Invoking(() => _testClass.PublicGetBodyStatements(new ClassModel(TestSemanticModelFactory.Class, TestSemanticModelFactory.Model, true), default(IInterfaceModel)).ToList()).Should().Throw<ArgumentNullException>();
        }
    }
}