namespace SentryOne.UnitTestGenerator.Core.Strategies.InterfaceGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;

    public class EnumerableGenerationStrategy : InterfaceGenerationStrategyBase
    {
        private const string InterfaceNameForMatch = "System.Collections.Generic.IEnumerable";
        private const string MethodNamePattern = "ImplementsIEnumerable{0}";

        public EnumerableGenerationStrategy(IFrameworkSet frameworkSet)
            : base(frameworkSet)
        {
        }

        public override string SupportedInterfaceName => InterfaceNameForMatch;

        protected override string GeneratedMethodNamePattern => MethodNamePattern;

        public override IEnumerable<MethodDeclarationSyntax> Create(ClassModel classModel, ClassModel model)
        {
            return GenerateMethods(classModel, model, GetBodyStatements);
        }

        private IEnumerable<StatementSyntax> GetBodyStatements(ClassModel sourceModel, IInterfaceModel interfaceModel)
        {
            if (sourceModel == null)
            {
                throw new ArgumentNullException(nameof(sourceModel));
            }

            if (interfaceModel == null)
            {
                throw new ArgumentNullException(nameof(interfaceModel));
            }

            ITypeSymbol enumerableTypeSymbol = sourceModel.TypeSymbol;
            if (interfaceModel.IsGeneric)
            {
                Debug.Assert(interfaceModel.GenericTypes.Count == 1, "Expecting one type argument for IEnumerable");
                enumerableTypeSymbol = interfaceModel.GenericTypes.First();
            }

            yield return SyntaxHelper.CreateVariableDeclaration(
                sourceModel.TypeSymbol.ToTypeSyntax(FrameworkSet.Context),
                "enumerable",
                SyntaxFactory.DefaultExpression(sourceModel.TypeSyntax))
                .AsLocalVariableDeclarationStatementSyntax();

            yield return SyntaxHelper.CreateVariableDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                "expectedCount",
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(-1)))
                .AsLocalVariableDeclarationStatementSyntax();

            yield return SyntaxHelper.CreateVariableDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                "actualCount",
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0)))
                .AsLocalVariableDeclarationStatementSyntax();

            yield return SyntaxFactory.UsingStatement(
                SyntaxFactory.Block(
                    FrameworkSet.TestFramework.AssertNotNull(SyntaxFactory.IdentifierName("enumerator")),
                    SyntaxFactory.WhileStatement(
                        SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName("enumerator"),
                                SyntaxFactory.IdentifierName("MoveNext"))),
                        SyntaxFactory.Block(
                            SyntaxFactory.ExpressionStatement(
                                SyntaxFactory.PostfixUnaryExpression(
                                    SyntaxKind.PostIncrementExpression,
                                    SyntaxFactory.IdentifierName("actualCount"))),
                            FrameworkSet.TestFramework.AssertIsInstanceOf(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.IdentifierName("enumerator"),
                                    SyntaxFactory.IdentifierName("Current")),
                                enumerableTypeSymbol.ToTypeSyntax(FrameworkSet.Context))))))
                .WithDeclaration(
                    SyntaxHelper.CreateVariableDeclaration("enumerator", SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("enumerable"),
                            SyntaxFactory.IdentifierName("GetEnumerator")))));

            yield return FrameworkSet.TestFramework.AssertEqual(SyntaxFactory.IdentifierName("actualCount"), SyntaxFactory.IdentifierName("expectedCount"));
        }
    }
}