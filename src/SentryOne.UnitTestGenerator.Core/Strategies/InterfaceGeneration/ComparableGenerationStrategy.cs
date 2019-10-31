namespace SentryOne.UnitTestGenerator.Core.Strategies.InterfaceGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;

    public class ComparableGenerationStrategy : InterfaceGenerationStrategyBase
    {
        private const string InterfaceNameForMatch = "System.IComparable";
        private const string MethodNamePattern = "ImplementsIComparable{0}";

        public ComparableGenerationStrategy(IFrameworkSet frameworkSet)
            : base(frameworkSet)
        {
        }

        protected override string GeneratedMethodNamePattern => MethodNamePattern;

        public override string SupportedInterfaceName => InterfaceNameForMatch;

        public override IEnumerable<MethodDeclarationSyntax> Create(ClassModel classModel, ClassModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return GenerateMethods(classModel, model, GetBodyStatements);
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

            yield return FrameworkSet.TestFramework.AssertEqual(
                SyntaxHelper.CreateInvocationStatement(
                    SyntaxFactory.IdentifierName("baseValue"),
                    SyntaxFactory.IdentifierName("CompareTo"),
                    SyntaxFactory.IdentifierName("equalToBaseValue")),
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(0)));

            yield return FrameworkSet.TestFramework.AssertLessThan(
                SyntaxHelper.CreateInvocationStatement(
                    SyntaxFactory.IdentifierName("baseValue"),
                    SyntaxFactory.IdentifierName("CompareTo"),
                    SyntaxFactory.IdentifierName("greaterThanBaseValue")),
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(0)));

            yield return FrameworkSet.TestFramework.AssertGreaterThan(
                SyntaxHelper.CreateInvocationStatement(
                    SyntaxFactory.IdentifierName("greaterThanBaseValue"),
                    SyntaxFactory.IdentifierName("CompareTo"),
                    SyntaxFactory.IdentifierName("baseValue")),
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(0)));
        }
    }
}