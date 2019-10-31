namespace SentryOne.UnitTestGenerator.Core.Tests.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NSubstitute;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Models;

    [TestFixture]
    public class OperatorModelTests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void CannotConstructWithNullNode()
        {
            Assert.Throws<ArgumentNullException>(() => new OperatorModel("TestValue526920974", new List<ParameterModel>(), default(OperatorDeclarationSyntax), ClassModelProvider.Instance.SemanticModel));
        }
    }
}