namespace Unitverse.Core.Tests.Helpers
{
    using Unitverse.Core.Helpers;
    using System;
    using NUnit.Framework;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp;

    [TestFixture]
    public static class XmlCommentHelperTests
    {
        [Test]
        public static void CannotCallWithXmlDocumentationWithBaseMethodDeclarationSyntaxAndDocumentationCommentTriviaSyntaxWithNullOriginalMethod()
        {
            FluentActions.Invoking(() => default(BaseMethodDeclarationSyntax).WithXmlDocumentation(SyntaxFactory.DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia))).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public static void CannotCallWithXmlDocumentationWithBaseMethodDeclarationSyntaxAndDocumentationCommentTriviaSyntaxWithNullDocumentationComment()
        {
            FluentActions.Invoking(() => SyntaxFactory.MethodDeclaration(SyntaxFactory.IdentifierName("a"), SyntaxFactory.Identifier("b")).WithXmlDocumentation(default(DocumentationCommentTriviaSyntax))).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public static void CannotCallWithXmlDocumentationWithMethodDeclarationSyntaxAndDocumentationCommentTriviaSyntaxWithNullOriginalMethod()
        {
            FluentActions.Invoking(() => default(MethodDeclarationSyntax).WithXmlDocumentation(SyntaxFactory.DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia))).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public static void CannotCallWithXmlDocumentationWithMethodDeclarationSyntaxAndDocumentationCommentTriviaSyntaxWithNullDocumentationComment()
        {
            FluentActions.Invoking(() => SyntaxFactory.MethodDeclaration(SyntaxFactory.IdentifierName("a"), SyntaxFactory.Identifier("b")).WithXmlDocumentation(default(DocumentationCommentTriviaSyntax))).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public static void CannotCallWithXmlDocumentationWithOriginalClassAndDocumentationCommentWithNullOriginalClass()
        {
            FluentActions.Invoking(() => default(ClassDeclarationSyntax).WithXmlDocumentation(SyntaxFactory.DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia))).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public static void CannotCallWithXmlDocumentationWithOriginalClassAndDocumentationCommentWithNullDocumentationComment()
        {
            FluentActions.Invoking(() => SyntaxFactory.ClassDeclaration(SyntaxFactory.Identifier("a")).WithXmlDocumentation(default(DocumentationCommentTriviaSyntax))).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CannotCallTextLiteralWithInvalidText(string value)
        {
            FluentActions.Invoking(() => XmlCommentHelper.TextLiteral(value)).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CannotCallSeeWithInvalidTypeName(string value)
        {
            FluentActions.Invoking(() => XmlCommentHelper.See(value)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public static void CannotCallSummaryWithNullNodes()
        {
            FluentActions.Invoking(() => XmlCommentHelper.Summary(default(XmlNodeSyntax[]))).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CannotCallParamWithInvalidName(string value)
        {
            FluentActions.Invoking(() => XmlCommentHelper.Param(value, "TestValue1113737288")).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CannotCallParamWithInvalidDescription(string value)
        {
            FluentActions.Invoking(() => XmlCommentHelper.Param("TestValue399244705", value)).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void CannotCallReturnsWithInvalidDescription(string value)
        {
            FluentActions.Invoking(() => XmlCommentHelper.Returns(value)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public static void CannotCallDocumentationCommentWithArrayOfXmlElementSyntaxWithNullElements()
        {
            FluentActions.Invoking(() => XmlCommentHelper.DocumentationComment(default(XmlElementSyntax[]))).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public static void CannotCallDocumentationCommentWithIEnumerableOfXmlElementSyntaxWithNullElements()
        {
            FluentActions.Invoking(() => XmlCommentHelper.DocumentationComment(default(IEnumerable<XmlElementSyntax>))).Should().Throw<ArgumentNullException>();
        }
    }
}