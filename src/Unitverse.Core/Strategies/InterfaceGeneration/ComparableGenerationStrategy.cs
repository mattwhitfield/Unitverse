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
            ITypeSymbol? comparableTypeIdentifier = sourceModel.TypeSymbol;
            if (interfaceModel.IsGeneric)
            {
                comparableTypeIdentifier = interfaceModel.GenericTypes.First();
            }

            if (comparableTypeIdentifier != null && sourceModel.TypeSymbol != null)
            {
                var typeSyntax = comparableTypeIdentifier.ToTypeSyntax(FrameworkSet.Context);

                method.Arrange(Generate.VariableDeclarator("baseValue", SyntaxFactory.DefaultExpression(sourceModel.TypeSyntax)).AsLocal(sourceModel.TypeSymbol.ToTypeSyntax(FrameworkSet.Context)));
                method.Arrange(Generate.VariableDeclarator("equalToBaseValue", SyntaxFactory.DefaultExpression(typeSyntax)).AsLocal(typeSyntax));
                method.Arrange(Generate.VariableDeclarator("greaterThanBaseValue", SyntaxFactory.DefaultExpression(typeSyntax)).AsLocal(typeSyntax));

                method.Assert(FrameworkSet.AssertionFramework.AssertEqual(CreateInvocationStatement("baseValue", "CompareTo", "equalToBaseValue"), Generate.Literal(0), false));

                method.Assert(FrameworkSet.AssertionFramework.AssertLessThan(CreateInvocationStatement("baseValue", "CompareTo", "greaterThanBaseValue"), Generate.Literal(0)));

                method.Assert(FrameworkSet.AssertionFramework.AssertGreaterThan(CreateInvocationStatement("greaterThanBaseValue", "CompareTo", "baseValue"), Generate.Literal(0)));
            }
        }

        private static InvocationExpressionSyntax CreateInvocationStatement(string targetName, string memberName, string parameterName)
        {
            return Generate.MemberInvocation(targetName, memberName, SyntaxFactory.IdentifierName(parameterName));
        }
    }
}