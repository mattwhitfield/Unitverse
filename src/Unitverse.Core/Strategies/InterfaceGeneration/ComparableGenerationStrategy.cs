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

    public class ComparableGenerationStrategy : InterfaceGenerationStrategyBase
    {
        private const string InterfaceNameForMatch = "System.IComparable";

        public ComparableGenerationStrategy(IFrameworkSet frameworkSet)
            : base(frameworkSet)
        {
        }

        protected override NameResolver GeneratedMethodNamePattern => FrameworkSet.NamingProvider.ImplementsIComparable;

        public override string SupportedInterfaceName => InterfaceNameForMatch;

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
            ITypeSymbol comparableTypeIdentifier = sourceModel.TypeSymbol;
            if (interfaceModel.IsGeneric)
            {
                Debug.Assert(interfaceModel.GenericTypes.Count == 1, "Expecting one type argument for IComparable");
                comparableTypeIdentifier = interfaceModel.GenericTypes.First();
            }

            var typeSyntax = comparableTypeIdentifier.ToTypeSyntax(FrameworkSet.Context);

            yield return SyntaxHelper.CreateVariableDeclaration(
                    sourceModel.TypeSymbol.ToTypeSyntax(FrameworkSet.Context),
                    "baseValue",
                    SyntaxFactory.DefaultExpression(sourceModel.TypeSyntax))
                .AsLocalVariableDeclarationStatementSyntax();

            yield return SyntaxHelper.CreateVariableDeclaration(
                    typeSyntax,
                    "equalToBaseValue",
                    SyntaxFactory.DefaultExpression(typeSyntax))
                .AsLocalVariableDeclarationStatementSyntax();

            yield return SyntaxHelper.CreateVariableDeclaration(
                    typeSyntax,
                    "greaterThanBaseValue",
                    SyntaxFactory.DefaultExpression(typeSyntax))
                .AsLocalVariableDeclarationStatementSyntax();

            yield return FrameworkSet.AssertionFramework.AssertEqual(CreateInvocationStatement("baseValue", "CompareTo", "equalToBaseValue"), Generate.Literal(0), false);

            yield return FrameworkSet.AssertionFramework.AssertLessThan(CreateInvocationStatement("baseValue", "CompareTo", "greaterThanBaseValue"), Generate.Literal(0));

            yield return FrameworkSet.AssertionFramework.AssertGreaterThan(CreateInvocationStatement("greaterThanBaseValue", "CompareTo", "baseValue"), Generate.Literal(0));
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