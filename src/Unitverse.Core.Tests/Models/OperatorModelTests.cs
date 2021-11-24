namespace Unitverse.Core.Tests.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;
    using Unitverse.Core.Models;

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