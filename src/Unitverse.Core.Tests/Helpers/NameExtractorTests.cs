namespace Unitverse.Core.Tests.Helpers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;
    using Unitverse.Core.Helpers;

    [TestFixture]
    public static class NameExtractorTests
    {
        [Test]
        public static void CanCallGetClassName()
        {
            var declaration = TestSemanticModelFactory.Class;
            var result = declaration.GetClassName();
            Assert.That(result, Is.EqualTo("ModelSource"));
        }

        [Test]
        public static void CannotCallGetClassNameWithNullDeclaration()
        {
            Assert.Throws<ArgumentNullException>(() => default(TypeDeclarationSyntax).GetClassName());
        }

        [Test]
        public static async Task CanCallGetNamespace()
        {
            var model = TestSemanticModelFactory.Model;
            var result = await model.GetNamespace().ConfigureAwait(true);
            Assert.That(result, Is.EqualTo("Test"));
        }

        [Test]
        public static void CannotCallGetNamespaceWithNullModel()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => default(SemanticModel).GetNamespace());
        }
    }
}