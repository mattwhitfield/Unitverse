namespace Unitverse.Core.Tests.Generation
{
    using Unitverse.Core.Generation;
    using NUnit.Framework;
    using FluentAssertions;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Formatting;

    [TestFixture]
    public class CategorizedUsingsTests
    {
        private CategorizedUsings _testClass;
        private IEnumerable<UsingDirectiveSyntax> _usings;
        private bool _separateSystemUsings;

        [SetUp]
        public void SetUp()
        {
            _usings = new[] {
                SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("Microsoft")).WithStaticKeyword(SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System")).WithStaticKeyword(SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("Fred")).WithStaticKeyword(SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("Microsoft")),
                SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("Microsoft.Stuff")),
                SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System")),
                SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("Fred")),
                SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System.Text")),
                SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System")).WithAlias(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName("A")))
            };

            _separateSystemUsings = false;
            _testClass = new CategorizedUsings(_usings, _separateSystemUsings);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new CategorizedUsings(_usings, _separateSystemUsings);

            // Assert
            instance.Should().NotBeNull();
        }

        [Test]
        public void CanCallGetResolvedUsingDirectivesWithNaturalOrder()
        {
            // Act
            var result = _testClass.GetResolvedUsingDirectives();

            // Assert
            Check(result, "using Fred;", "using Microsoft;", "using Microsoft.Stuff;", "using System;", "using System.Text;", "using A = System;", "using static Fred;", "using static Microsoft;", "using static System;");
        }

        [Test]
        public void CanCallGetResolvedUsingDirectivesWithSeparateSystemUsings()
        {
            // Arrange
            _testClass = new CategorizedUsings(_usings, true);

            // Act
            var result = _testClass.GetResolvedUsingDirectives();

            // Assert
            Check(result, "using System;", "using System.Text;", "using Fred;", "using Microsoft;", "using Microsoft.Stuff;", "using A = System;", "using static System;", "using static Fred;", "using static Microsoft;");
        }

        private void Check(IEnumerable<UsingDirectiveSyntax> usingDirectives, params string[] expected)
        {
            var output = new List<string>();
            using (var workspace = new AdhocWorkspace())
            {                
                foreach (var directive in usingDirectives)
                {
                    var expressionText = Formatter.Format(directive, workspace).ToString();
                    output.Add(expressionText);
                }
            }

            output.Should().BeEquivalentTo(expected);
        }
    }
}