namespace Unitverse.Core.Strategies.InterfaceGeneration
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public class ComparableGenerationStrategy : InterfaceGenerationStrategyBase
    {
        public const string InterfaceNameForMatch = "System.IComparable";

        public ComparableGenerationStrategy(IFrameworkSet frameworkSet)
            : base(frameworkSet, InterfaceNameForMatch)
        {
        }

        protected override NameResolver GeneratedMethodNamePattern => FrameworkSet.NamingProvider.ImplementsIComparable;

        protected override void AddBodyStatements(ClassModel sourceModel, IInterfaceModel interfaceModel, SectionedMethodHandler method)
        {
            ITypeSymbol comparableTypeIdentifier = sourceModel.TypeSymbol;
            if (interfaceModel.IsGeneric)
            {
                comparableTypeIdentifier = interfaceModel.GenericTypes.First();
            }

            var typeSyntax = comparableTypeIdentifier.ToTypeSyntax(FrameworkSet.Context);

            method.Arrange(SyntaxHelper.CreateVariableDeclaration(
                    sourceModel.TypeSymbol.ToTypeSyntax(FrameworkSet.Context),
                    "baseValue",
                    SyntaxFactory.DefaultExpression(sourceModel.TypeSyntax))
                .AsLocalVariableDeclarationStatementSyntax());

            method.Arrange(SyntaxHelper.CreateVariableDeclaration(
                    typeSyntax,
                    "equalToBaseValue",
                    SyntaxFactory.DefaultExpression(typeSyntax))
                .AsLocalVariableDeclarationStatementSyntax());

            method.Arrange(SyntaxHelper.CreateVariableDeclaration(
                    typeSyntax,
                    "greaterThanBaseValue",
                    SyntaxFactory.DefaultExpression(typeSyntax))
                .AsLocalVariableDeclarationStatementSyntax());

            method.Assert(FrameworkSet.AssertionFramework.AssertEqual(CreateInvocationStatement("baseValue", "CompareTo", "equalToBaseValue"), Generate.Literal(0), false));

            method.Assert(FrameworkSet.AssertionFramework.AssertLessThan(CreateInvocationStatement("baseValue", "CompareTo", "greaterThanBaseValue"), Generate.Literal(0)));

            method.Assert(FrameworkSet.AssertionFramework.AssertGreaterThan(CreateInvocationStatement("greaterThanBaseValue", "CompareTo", "baseValue"), Generate.Literal(0)));
        }

        private static InvocationExpressionSyntax CreateInvocationStatement(string targetName, string memberName, string parameterName)
        {
            return SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(targetName),
                        SyntaxFactory.IdentifierName(memberName)))
                .WithArgumentList(Generate.Arguments(SyntaxFactory.IdentifierName(parameterName)));
        }
    }
}