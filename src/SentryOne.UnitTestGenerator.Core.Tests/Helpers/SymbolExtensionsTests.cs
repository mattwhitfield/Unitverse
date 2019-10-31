namespace SentryOne.UnitTestGenerator.Core.Tests.Helpers
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Helpers;

    [TestFixture]
    public static class SymbolExtensionsTests
    {
        [Test]
        public static void CanCallIsAwaitableNonDynamic()
        {
            var symbols = TestSemanticModelFactory.Class.DescendantNodes().OfType<MethodDeclarationSyntax>().Select(node => TestSemanticModelFactory.Model.GetDeclaredSymbol(node)).OfType<IMethodSymbol>().ToList();
            Assert.That(symbols.First(x => x.Name == "Method").IsAwaitableNonDynamic(), Is.False);
            Assert.That(symbols.First(x => x.Name == "AsyncMethod").IsAwaitableNonDynamic(), Is.True);
        }

        [Test]
        public static void CanCallIsAwaitableNonDynamicWithNullSymbol()
        {
            Assert.That(() => default(IMethodSymbol).IsAwaitableNonDynamic(), Is.False);
        }
    }
}