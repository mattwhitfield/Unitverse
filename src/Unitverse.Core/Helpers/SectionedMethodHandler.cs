namespace Unitverse.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class SectionedMethodHandler
    {
        public SectionedMethodHandler(MethodDeclarationSyntax method, string arrangeComment, string actComment, string assertComment)
        {
            Method = method ?? throw new ArgumentNullException(nameof(method));
            _arrangeComment = arrangeComment;
            _actComment = actComment;
            _assertComment = assertComment;
        }

        private readonly string _arrangeComment;
        private readonly string _actComment;
        private readonly string _assertComment;

        private enum Section
        {
            None,
            Arrange,
            Act,
            Assert,
        }

        private Section _currentSection;

        private bool _blankLineRequired;

        private bool _anyEmitted;

        public MethodDeclarationSyntax Method { get; private set; }

        public void Arrange(StatementSyntax statement)
        {
            if (statement is null)
            {
                throw new ArgumentNullException(nameof(statement));
            }

            Method = Method.AddBodyStatements(Prepare(statement, Section.Arrange));
        }

        public void Arrange(IEnumerable<StatementSyntax> statements)
        {
            if (statements is null)
            {
                throw new ArgumentNullException(nameof(statements));
            }

            foreach (var statement in statements)
            {
                Arrange(statement);
            }
        }

        public void Act(StatementSyntax statement)
        {
            if (statement is null)
            {
                throw new ArgumentNullException(nameof(statement));
            }

            Method = Method.AddBodyStatements(Prepare(statement, Section.Act));
        }

        public void Assert(StatementSyntax statement)
        {
            if (statement is null)
            {
                throw new ArgumentNullException(nameof(statement));
            }

            Method = Method.AddBodyStatements(Prepare(statement, Section.Assert));
        }

        public void Assert(IEnumerable<StatementSyntax> statements)
        {
            if (statements is null)
            {
                throw new ArgumentNullException(nameof(statements));
            }

            foreach (var statement in statements)
            {
                Assert(statement);
            }
        }

        public void BlankLine()
        {
            _blankLineRequired = true;
        }

        public void Emit(StatementSyntax statement)
        {
            if (statement is null)
            {
                throw new ArgumentNullException(nameof(statement));
            }

            Method = Method.AddBodyStatements(Prepare(statement, Section.None));
        }

        public void Emit(IEnumerable<StatementSyntax> statements)
        {
            if (statements is null)
            {
                throw new ArgumentNullException(nameof(statements));
            }

            foreach (var statement in statements)
            {
                Emit(statement);
            }
        }

        private StatementSyntax Prepare(StatementSyntax syntax, Section section)
        {
            var comment = string.Empty;
            if (_currentSection != section)
            {
                switch (section)
                {
                    case Section.Arrange:
                        comment = _arrangeComment;
                        _blankLineRequired = _anyEmitted;
                        break;
                    case Section.Act:
                        comment = _actComment;
                        _blankLineRequired = _anyEmitted;
                        break;
                    case Section.Assert:
                        comment = _assertComment;
                        _blankLineRequired = _anyEmitted;
                        break;
                }

                _currentSection = section;
            }

            if (!string.IsNullOrWhiteSpace(comment))
            {
                var commentSyntax = SyntaxFactory.Comment("// " + comment.Trim() + Environment.NewLine);
                if (_anyEmitted)
                {
                    syntax = syntax.WithLeadingTrivia(SyntaxFactory.Comment(Environment.NewLine), commentSyntax);
                }
                else
                {
                    syntax = syntax.WithLeadingTrivia(commentSyntax);
                }
            }
            else if (_blankLineRequired)
            {
                syntax = syntax.WithLeadingTrivia(SyntaxFactory.Comment(Environment.NewLine));
            }

            _blankLineRequired = false;
            _anyEmitted = true;

            return syntax;
        }
    }
}
