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
    using Microsoft.CodeAnalysis.CSharp;

    [TestFixture]
    public class EquatableGenerationStrategyTests
    {
        private class TestEquatableGenerationStrategy : EquatableGenerationStrategy
        {
            public TestEquatableGenerationStrategy(IFrameworkSet frameworkSet) : base(frameworkSet)
            {
            }

            public void PublicAddBodyStatements(ClassModel sourceModel, IInterfaceModel interfaceModel)
            {
                base.AddBodyStatements(sourceModel, interfaceModel, new Core.Helpers.SectionedMethodHandler(SyntaxFactory.MethodDeclaration(SyntaxFactory.IdentifierName("string"), "id"), "Arrange", "Act", "Assert"));
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
            FluentActions.Invoking(() => _testClass.PublicAddBodyStatements(default(ClassModel), Substitute.For<IInterfaceModel>())).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotCallGetBodyStatementsWithNullInterfaceModel()
        {
            FluentActions.Invoking(() => _testClass.PublicAddBodyStatements(new ClassModel(TestSemanticModelFactory.Class, TestSemanticModelFactory.Model, true), default(IInterfaceModel))).Should().Throw<ArgumentNullException>();
        }
    }
}