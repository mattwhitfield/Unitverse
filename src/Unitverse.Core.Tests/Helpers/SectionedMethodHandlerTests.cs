namespace Unitverse.Core.Tests.Helpers
{
    using Unitverse.Core.Helpers;
    using System;
    using NUnit.Framework;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis;
    using System.Linq;
    using Microsoft.CodeAnalysis.Formatting;
    using Unitverse.Tests.Common;

    [TestFixture]
    public class SectionedMethodHandlerTests
    {
        private SectionedMethodHandler _testClass;
        private MethodDeclarationSyntax _method;

        [SetUp]
        public void SetUp()
        {
            _method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), "method");
            _testClass = new SectionedMethodHandler(_method, new DefaultGenerationOptions(), "Arrange", "Act", "Assert");
        }

        [Test]
        public void CannotConstructWithNullMethod()
        {
            FluentActions.Invoking(() => new SectionedMethodHandler(default(MethodDeclarationSyntax), new DefaultGenerationOptions(), "Arrange", "Act", "Assert")).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotConstructWithNullGenerationOptions()
        {
            FluentActions.Invoking(() => new SectionedMethodHandler(_method, null, "Arrange", "Act", "Assert")).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void SingleTransitionNoComments()
        {
            _testClass = new SectionedMethodHandler(_method, new DefaultGenerationOptions(), null, null, null);
            _testClass.Arrange(Generate.ImplicitlyTypedVariableDeclaration("someVar", Generate.Literal(5)));
            AssertOutput("var someVar = 5;");
        }

        [Test]
        public void EndingBlankLineNoComments()
        {
            _testClass = new SectionedMethodHandler(_method, new DefaultGenerationOptions(), null, null, null);
            _testClass.Arrange(Generate.ImplicitlyTypedVariableDeclaration("someVar", Generate.Literal(5)));
            _testClass.BlankLine();
            AssertOutput("var someVar = 5;");
        }

        [Test]
        public void DoubleTransitionNoComments()
        {
            _testClass = new SectionedMethodHandler(_method, new DefaultGenerationOptions(), null, null, null);
            _testClass.Arrange(Generate.ImplicitlyTypedVariableDeclaration("someVar", Generate.Literal(5)));
            _testClass.Act(Generate.ImplicitlyTypedVariableDeclaration("someVar2", Generate.Literal(6)));
            AssertOutput("var someVar = 5;", "", "var someVar2 = 6;");
        }

        [Test]
        public void DoubleTransitionExplicitBlankNoComments()
        {
            _testClass = new SectionedMethodHandler(_method, new DefaultGenerationOptions(), null, null, null);
            _testClass.Arrange(Generate.ImplicitlyTypedVariableDeclaration("someVar", Generate.Literal(5)));
            _testClass.BlankLine();
            _testClass.Act(Generate.ImplicitlyTypedVariableDeclaration("someVar2", Generate.Literal(6)));
            AssertOutput("var someVar = 5;", "", "var someVar2 = 6;");
        }

        [Test]
        public void DoubleTransitionStupidBlankNoComments()
        {
            _testClass = new SectionedMethodHandler(_method, new DefaultGenerationOptions(), null, null, null);
            _testClass.Arrange(Generate.ImplicitlyTypedVariableDeclaration("someVar", Generate.Literal(5)));
            _testClass.BlankLine();
            _testClass.BlankLine();
            _testClass.BlankLine();
            _testClass.BlankLine();
            _testClass.BlankLine();
            _testClass.Act(Generate.ImplicitlyTypedVariableDeclaration("someVar2", Generate.Literal(6)));
            AssertOutput("var someVar = 5;", "", "var someVar2 = 6;");
        }

        [Test]
        public void PlainEmitNoComments()
        {
            _testClass = new SectionedMethodHandler(_method, new DefaultGenerationOptions(), null, null, null);
            _testClass.Emit(Generate.ImplicitlyTypedVariableDeclaration("someVar", Generate.Literal(5)));
            AssertOutput("var someVar = 5;");
        }

        [Test]
        public void PlainEmitBlankLinesNoComments()
        {
            _testClass = new SectionedMethodHandler(_method, new DefaultGenerationOptions(), null, null, null);
            _testClass.Emit(Generate.ImplicitlyTypedVariableDeclaration("someVar", Generate.Literal(5)));
            _testClass.BlankLine();
            _testClass.Emit(Generate.ImplicitlyTypedVariableDeclaration("someVar2", Generate.Literal(6)));
            AssertOutput("var someVar = 5;", "", "var someVar2 = 6;");
        }

        [Test]
        public void SingleTransition()
        {
            _testClass.Arrange(Generate.ImplicitlyTypedVariableDeclaration("someVar", Generate.Literal(5)));
            AssertOutput("// Arrange", "var someVar = 5;");
        }

        [Test]
        public void EndingBlankLine()
        {
            _testClass.Arrange(Generate.ImplicitlyTypedVariableDeclaration("someVar", Generate.Literal(5)));
            _testClass.BlankLine();
            AssertOutput("// Arrange", "var someVar = 5;");
        }

        [Test]
        public void DoubleTransition()
        {
            _testClass.Arrange(Generate.ImplicitlyTypedVariableDeclaration("someVar", Generate.Literal(5)));
            _testClass.Act(Generate.ImplicitlyTypedVariableDeclaration("someVar2", Generate.Literal(6)));
            AssertOutput("// Arrange", "var someVar = 5;", "", "// Act", "var someVar2 = 6;");
        }

        [Test]
        public void DoubleTransitionExplicitBlank()
        {
            _testClass.Arrange(Generate.ImplicitlyTypedVariableDeclaration("someVar", Generate.Literal(5)));
            _testClass.BlankLine();
            _testClass.Act(Generate.ImplicitlyTypedVariableDeclaration("someVar2", Generate.Literal(6)));
            AssertOutput("// Arrange", "var someVar = 5;", "", "// Act", "var someVar2 = 6;");
        }

        [Test]
        public void DoubleTransitionStupidBlank()
        {
            _testClass.Arrange(Generate.ImplicitlyTypedVariableDeclaration("someVar", Generate.Literal(5)));
            _testClass.BlankLine();
            _testClass.BlankLine();
            _testClass.BlankLine();
            _testClass.BlankLine();
            _testClass.BlankLine();
            _testClass.Act(Generate.ImplicitlyTypedVariableDeclaration("someVar2", Generate.Literal(6)));
            AssertOutput("// Arrange", "var someVar = 5;", "", "// Act", "var someVar2 = 6;");
        }

        [Test]
        public void PlainEmit()
        {
            _testClass.Emit(Generate.ImplicitlyTypedVariableDeclaration("someVar", Generate.Literal(5)));
            AssertOutput("var someVar = 5;");
        }

        [Test]
        public void PlainEmitBlankLines()
        {
            _testClass.Emit(Generate.ImplicitlyTypedVariableDeclaration("someVar", Generate.Literal(5)));
            _testClass.BlankLine();
            _testClass.Emit(Generate.ImplicitlyTypedVariableDeclaration("someVar2", Generate.Literal(6)));
            AssertOutput("var someVar = 5;", "", "var someVar2 = 6;");
        }

        private void AssertOutput(params string[] expectedOutput)
        {
            using (var workspace = new AdhocWorkspace())
            {
                var type = SyntaxFactory.ClassDeclaration(SyntaxFactory.Identifier("cls")).AddMembers(_testClass.Method);
                var targetNamespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName("namespace")).AddMembers(type);
                var compilation = SyntaxFactory.CompilationUnit().AddMembers(targetNamespace);
                
                compilation = (CompilationUnitSyntax)Formatter.Format(compilation, workspace);

                var methodAsText = compilation.ToFullString().Lines().ToList();
                var refinedLines = methodAsText.Skip(6).Take(methodAsText.Count - 9).Select(x => x.Trim()).ToList();
                refinedLines.Should().BeEquivalentTo(expectedOutput);
            }

        }

        [Test]
        public void CannotCallArrangeWithStatementSyntaxWithNullStatement()
        {
            FluentActions.Invoking(() => _testClass.Arrange(default(StatementSyntax))).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotCallArrangeWithIEnumerableOfStatementSyntaxWithNullStatements()
        {
            FluentActions.Invoking(() => _testClass.Arrange(default(IEnumerable<StatementSyntax>))).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotCallActWithNullStatement()
        {
            FluentActions.Invoking(() => _testClass.Act(default(StatementSyntax))).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotCallAssertWithStatementSyntaxWithNullStatement()
        {
            FluentActions.Invoking(() => _testClass.Assert(default(StatementSyntax))).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotCallAssertWithIEnumerableOfStatementSyntaxWithNullStatements()
        {
            FluentActions.Invoking(() => _testClass.Assert(default(IEnumerable<StatementSyntax>))).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotCallEmitWithStatementSyntaxWithNullStatement()
        {
            FluentActions.Invoking(() => _testClass.Emit(default(StatementSyntax))).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotCallEmitWithIEnumerableOfStatementSyntaxWithNullStatements()
        {
            FluentActions.Invoking(() => _testClass.Emit(default(IEnumerable<StatementSyntax>))).Should().Throw<ArgumentNullException>();
        }
    }
}