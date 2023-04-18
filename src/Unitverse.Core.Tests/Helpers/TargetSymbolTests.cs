namespace Unitverse.Core.Tests.Helpers
{
    using FluentAssertions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Helpers;

    [TestFixture]
    public class TargetSymbolTests
    {
        private TargetSymbol _testClass;
        private SyntaxNode _node;
        private ISymbol _symbol;
        private TypeInfo _type;

        [SetUp]
        public void SetUp()
        {
            _node = SyntaxFactory.TypeDeclaration(SyntaxKind.ClassKeyword, "class");
            _symbol = Substitute.For<ISymbol>();
            _type = new TypeInfo();
            _testClass = new TargetSymbol(_node, _symbol, _type);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new TargetSymbol(_node, _symbol, _type);

            // Assert
            instance.Should().NotBeNull();
        }

        [Test]
        public void NodeIsInitializedCorrectly()
        {
            _testClass.Node.Should().BeSameAs(_node);
        }

        [Test]
        public void SymbolIsInitializedCorrectly()
        {
            _testClass.Symbol.Should().BeSameAs(_symbol);
        }

        [Test]
        public void TypeIsInitializedCorrectly()
        {
            _testClass.Type.Should().Be(_type);
        }
    }
}