namespace Unitverse.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Options;

    public class SectionedMethodHandler
    {
        public SectionedMethodHandler(BaseMethodDeclarationSyntax method, IGenerationOptions generationOptions)
            : this(method, generationOptions, string.Empty, string.Empty, string.Empty)
        {
        }

        public SectionedMethodHandler(BaseMethodDeclarationSyntax method, IGenerationOptions generationOptions, string arrangeComment, string actComment, string assertComment)
        {
            _method = method ?? throw new ArgumentNullException(nameof(method));
            _generationOptions = generationOptions ?? throw new ArgumentNullException(nameof(generationOptions));
            _arrangeComment = arrangeComment;
            _actComment = actComment;
            _assertComment = assertComment;
        }

        private readonly string _arrangeComment;
        private readonly string _actComment;
        private readonly string _assertComment;
        private readonly HashSet<string> _requirements = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private enum Section
        {
            None,
            Arrange,
            Act,
            Assert,
        }

        private BaseMethodDeclarationSyntax _method;
        private readonly IGenerationOptions _generationOptions;
        private Section _currentSection;

        private bool _blankLineRequired;

        private bool _anyEmitted;

        private IList<Func<BaseMethodDeclarationSyntax, BaseMethodDeclarationSyntax>> _modifiers = new List<Func<BaseMethodDeclarationSyntax, BaseMethodDeclarationSyntax>>();

        public void AddRequirement(string requirement)
        {
            _requirements.Add(requirement);
        }

        public BaseMethodDeclarationSyntax Method
        {
            get
            {
                var method = _method;

                if (_requirements.Contains(Requirements.AutoMocker))
                {
                    method = method.AddBodyStatements(Prepare(Generate.ImplicitlyTypedVariableDeclaration("mocker", Generate.ObjectCreation(SyntaxFactory.IdentifierName("AutoMocker"))), Section.Arrange));
                }

                if (_requirements.Contains(Requirements.AutoFixture) && !_generationOptions.UseFieldForAutoFixture)
                {
                    method = method.AddBodyStatements(Prepare(AutoFixtureHelper.VariableDeclaration(_generationOptions), Section.Arrange));
                }

                foreach (var modifier in _modifiers)
                {
                    method = modifier(method);
                }

                if (method.Body == null)
                {
                    method = method.WithBody(SyntaxFactory.Block());
                }

                return method;
            }
        }

        public void Arrange(StatementSyntax statement)
        {
            if (statement is null)
            {
                throw new ArgumentNullException(nameof(statement));
            }

            _modifiers.Add(method => method.AddBodyStatements(Prepare(statement, Section.Arrange)));
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

            _modifiers.Add(method => method.AddBodyStatements(Prepare(statement, Section.Act)));
        }

        public void Assert(StatementSyntax statement)
        {
            if (statement is null)
            {
                throw new ArgumentNullException(nameof(statement));
            }

            _modifiers.Add(method => method.AddBodyStatements(Prepare(statement, Section.Assert)));
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
            _modifiers.Add(method =>
            {
                _blankLineRequired = true;
                return method;
            });
        }

        public void Emit(StatementSyntax statement)
        {
            if (statement is null)
            {
                throw new ArgumentNullException(nameof(statement));
            }

            _modifiers.Add(method => method.AddBodyStatements(Prepare(statement, Section.None)));
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
