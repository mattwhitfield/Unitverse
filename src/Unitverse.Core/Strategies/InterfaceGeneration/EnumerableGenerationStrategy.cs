namespace Unitverse.Core.Strategies.InterfaceGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public class EnumerableGenerationStrategy : InterfaceGenerationStrategyBase
    {
        private const string InterfaceNameForMatch = "System.Collections.Generic.IEnumerable";

        public EnumerableGenerationStrategy(IFrameworkSet frameworkSet)
            : base(frameworkSet)
        {
        }

        public override string SupportedInterfaceName => InterfaceNameForMatch;

        protected override NameResolver GeneratedMethodNamePattern => FrameworkSet.NamingProvider.ImplementsIEnumerable;

        public override IEnumerable<MethodDeclarationSyntax> Create(ClassModel classModel, ClassModel model, NamingContext namingContext)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return GenerateMethods(classModel, model, namingContext, GetBodyStatements);
        }

        private IEnumerable<StatementSyntax> GetBodyStatements(ClassModel sourceModel, IInterfaceModel interfaceModel)
        {
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
                    SyntaxHelper.CreateVariableDeclaration("enumerator", SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("enumerable"),
                            SyntaxFactory.IdentifierName("GetEnumerator")))));

            yield return FrameworkSet.AssertionFramework.AssertEqual(SyntaxFactory.IdentifierName("actualCount"), SyntaxFactory.IdentifierName("expectedCount"), false);
        }
    }
}