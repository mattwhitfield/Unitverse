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

            method.Arrange(Generate.VariableDeclarator("enumerable", SyntaxFactory.DefaultExpression(sourceModel.TypeSyntax)).AsLocal(sourceModel.TypeSymbol.ToTypeSyntax(FrameworkSet.Context)));
            method.Arrange(Generate.VariableDeclarator("expectedCount", Generate.Literal(-1)).AsLocal(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))));
            method.Arrange(Generate.VariableDeclarator("actualCount", Generate.Literal(0)).AsLocal(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))));

            method.Act(SyntaxFactory.UsingStatement(
                SyntaxFactory.Block(
                    FrameworkSet.AssertionFramework.AssertNotNull(SyntaxFactory.IdentifierName("enumerator")),
                    SyntaxFactory.WhileStatement(
                        Generate.MemberInvocation("enumerator", "MoveNext"),
                        SyntaxFactory.Block(
                            Generate.Statement(
                                SyntaxFactory.PostfixUnaryExpression(
                                    SyntaxKind.PostIncrementExpression,
                                    SyntaxFactory.IdentifierName("actualCount"))),
                            FrameworkSet.AssertionFramework.AssertIsInstanceOf(
                                Generate.MemberAccess("enumerator", "Current"),
                                enumerableTypeSymbol.ToTypeSyntax(FrameworkSet.Context),
                                enumerableTypeSymbol.IsReferenceType)))))
                .WithDeclaration(
                    Generate.VariableDeclarator("enumerator", Generate.MemberInvocation("enumerable", "GetEnumerator")).AsDeclaration(SyntaxFactory.IdentifierName("var"))));

            method.Assert(FrameworkSet.AssertionFramework.AssertEqual(SyntaxFactory.IdentifierName("actualCount"), SyntaxFactory.IdentifierName("expectedCount"), false));
        }
    }
}