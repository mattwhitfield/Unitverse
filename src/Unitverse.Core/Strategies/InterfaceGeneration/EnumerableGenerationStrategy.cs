namespace Unitverse.Core.Strategies.InterfaceGeneration
{
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public class EnumerableGenerationStrategy : InterfaceGenerationStrategyBase
    {
        public const string InterfaceNameForMatch = "System.Collections.Generic.IEnumerable";

        public EnumerableGenerationStrategy(IFrameworkSet frameworkSet)
            : base(frameworkSet, InterfaceNameForMatch)
        {
        }

        protected override NameResolver GeneratedMethodNamePattern => FrameworkSet.NamingProvider.ImplementsIEnumerable;

        protected override void AddBodyStatements(ClassModel sourceModel, IInterfaceModel interfaceModel, SectionedMethodHandler method)
        {
            ITypeSymbol enumerableTypeSymbol = sourceModel.TypeSymbol;
            if (interfaceModel.IsGeneric)
            {
                Debug.Assert(interfaceModel.GenericTypes.Count == 1, "Expecting one type argument for IEnumerable");
                enumerableTypeSymbol = interfaceModel.GenericTypes.First();
            }

            method.Arrange(SyntaxHelper.CreateVariableDeclaration(
                sourceModel.TypeSymbol.ToTypeSyntax(FrameworkSet.Context),
                "enumerable",
                SyntaxFactory.DefaultExpression(sourceModel.TypeSyntax))
                .AsLocalVariableDeclarationStatementSyntax());

            method.Arrange(SyntaxHelper.CreateVariableDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                "expectedCount",
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(-1)))
                .AsLocalVariableDeclarationStatementSyntax());

            method.Arrange(SyntaxHelper.CreateVariableDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                "actualCount",
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0)))
                .AsLocalVariableDeclarationStatementSyntax());

            method.Act(SyntaxFactory.UsingStatement(
                SyntaxFactory.Block(
                    FrameworkSet.AssertionFramework.AssertNotNull(SyntaxFactory.IdentifierName("enumerator")),
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
                            FrameworkSet.AssertionFramework.AssertIsInstanceOf(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.IdentifierName("enumerator"),
                                    SyntaxFactory.IdentifierName("Current")),
                                enumerableTypeSymbol.ToTypeSyntax(FrameworkSet.Context),
                                enumerableTypeSymbol.IsReferenceType)))))
                .WithDeclaration(
                    SyntaxHelper.CreateVariableDeclaration(SyntaxFactory.IdentifierName("var"), "enumerator", SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("enumerable"),
                            SyntaxFactory.IdentifierName("GetEnumerator"))))));

            method.Assert(FrameworkSet.AssertionFramework.AssertEqual(SyntaxFactory.IdentifierName("actualCount"), SyntaxFactory.IdentifierName("expectedCount"), false));
        }
    }
}