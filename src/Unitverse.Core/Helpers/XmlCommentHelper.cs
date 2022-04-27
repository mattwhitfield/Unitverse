namespace Unitverse.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class XmlCommentHelper
    {
        public static BaseMethodDeclarationSyntax WithXmlDocumentation(this BaseMethodDeclarationSyntax originalMethod, DocumentationCommentTriviaSyntax documentationComment)
        {
            if (originalMethod is null)
            {
                throw new ArgumentNullException(nameof(originalMethod));
            }

            if (documentationComment is null)
            {
                throw new ArgumentNullException(nameof(documentationComment));
            }

            if (originalMethod.AttributeLists.Count > 0)
            {
                return originalMethod.WithAttributeLists(GetNewAttributeLists(originalMethod.AttributeLists, documentationComment));
            }

            return originalMethod.WithModifiers(GetNewModifiers(originalMethod.Modifiers, documentationComment));
        }

        public static MethodDeclarationSyntax WithXmlDocumentation(this MethodDeclarationSyntax originalMethod, DocumentationCommentTriviaSyntax documentationComment)
        {
            if (originalMethod is null)
            {
                throw new ArgumentNullException(nameof(originalMethod));
            }

            if (documentationComment is null)
            {
                throw new ArgumentNullException(nameof(documentationComment));
            }

            if (originalMethod.AttributeLists.Count > 0)
            {
                return originalMethod.WithAttributeLists(GetNewAttributeLists(originalMethod.AttributeLists, documentationComment));
            }

            return originalMethod.WithModifiers(GetNewModifiers(originalMethod.Modifiers, documentationComment));
        }

        public static ClassDeclarationSyntax WithXmlDocumentation(this ClassDeclarationSyntax originalClass, DocumentationCommentTriviaSyntax documentationComment)
        {
            if (originalClass is null)
            {
                throw new ArgumentNullException(nameof(originalClass));
            }

            if (documentationComment is null)
            {
                throw new ArgumentNullException(nameof(documentationComment));
            }

            if (originalClass.AttributeLists.Count > 0)
            {
                return originalClass.WithAttributeLists(GetNewAttributeLists(originalClass.AttributeLists, documentationComment));
            }

            return originalClass.WithModifiers(GetNewModifiers(originalClass.Modifiers, documentationComment));
        }

        private static SyntaxList<AttributeListSyntax> GetNewAttributeLists(SyntaxList<AttributeListSyntax> existingAttributes, DocumentationCommentTriviaSyntax documentationComment)
        {
            var attributes = new List<AttributeListSyntax>();

            attributes.Add(existingAttributes[0].WithOpenBracketToken(SyntaxFactory.Token(SyntaxFactory.TriviaList(SyntaxFactory.Trivia(documentationComment)), SyntaxKind.OpenBracketToken, SyntaxFactory.TriviaList())));

            for (int i = 1; i < existingAttributes.Count; i++)
            {
                attributes.Add(existingAttributes[i]);
            }

            return SyntaxFactory.List(attributes);
        }

        private static SyntaxTokenList GetNewModifiers(SyntaxTokenList existingModifiers, DocumentationCommentTriviaSyntax documentationComment)
        {
            var tokens = new List<SyntaxToken>();

            var firstSyntaxKind = (existingModifiers.Count == 0) ? SyntaxKind.PublicKeyword : existingModifiers[0].Kind();
            tokens.Add(SyntaxFactory.Token(SyntaxFactory.TriviaList(SyntaxFactory.Trivia(documentationComment)), firstSyntaxKind, SyntaxFactory.TriviaList()));

            for (int i = 1; i < existingModifiers.Count; i++)
            {
                tokens.Add(existingModifiers[i]);
            }

            return SyntaxFactory.TokenList(tokens);
        }

        private static SyntaxToken NewLineLiteral()
        {
            return SyntaxFactory.XmlTextNewLine(SyntaxFactory.TriviaList(), Environment.NewLine, Environment.NewLine, SyntaxFactory.TriviaList());
        }

        private static SyntaxToken LineStartLiteral()
        {
            return SyntaxFactory.XmlTextLiteral(SyntaxFactory.TriviaList(SyntaxFactory.DocumentationCommentExterior("///")), " ", " ", SyntaxFactory.TriviaList());
        }

        private static XmlNodeSyntax LineStart()
        {
            return SyntaxFactory.XmlText(LineStartLiteral());
        }

        private static XmlNodeSyntax NewLine()
        {
            return SyntaxFactory.XmlText(NewLineLiteral());
        }

        private static XmlNodeSyntax NewLineAndLineStart()
        {
            return SyntaxFactory.XmlText(NewLineLiteral(), LineStartLiteral());
        }

        public static XmlNodeSyntax TextLiteral(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            return SyntaxFactory.XmlText(SyntaxFactory.XmlTextLiteral(SyntaxFactory.TriviaList(), text, text, SyntaxFactory.TriviaList()));
        }

        public static XmlNodeSyntax See(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            return SyntaxFactory.XmlSeeElement(SyntaxFactory.NameMemberCref(SyntaxFactory.IdentifierName(typeName)));
        }

        public static XmlElementSyntax Summary(params XmlNodeSyntax[] nodes)
        {
            var list = new List<XmlNodeSyntax>();
            list.Add(NewLineAndLineStart());
            list.AddRange(nodes);
            list.Add(NewLineAndLineStart());

            return SyntaxFactory.XmlSummaryElement(list.ToArray());
        }

        public static XmlElementSyntax Param(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            return SyntaxFactory.XmlParamElement(name, TextLiteral(description));
        }

        public static XmlElementSyntax Returns(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            return SyntaxFactory.XmlReturnsElement(TextLiteral(description));
        }

        public static DocumentationCommentTriviaSyntax DocumentationComment(params XmlElementSyntax[] elements)
        {
            return DocumentationComment(elements.AsEnumerable());
        }

        public static DocumentationCommentTriviaSyntax DocumentationComment(IEnumerable<XmlElementSyntax> elements)
        {
            if (elements is null)
            {
                throw new ArgumentNullException(nameof(elements));
            }

            var list = new List<XmlNodeSyntax>();
            var first = true;
            foreach (var element in elements)
            {
                if (first)
                {
                    list.Add(LineStart());
                    first = false;
                }
                else
                {
                    list.Add(NewLineAndLineStart());
                }

                list.Add(element);
            }

            list.Add(NewLine());

            return SyntaxFactory.DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxFactory.List<XmlNodeSyntax>(list));
        }
    }
}
