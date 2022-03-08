namespace Unitverse.Core.Strategies.InterfaceGeneration
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public class EquatableGenerationStrategy : InterfaceGenerationStrategyBase
    {
        public const string InterfaceNameForMatch = "System.IEquatable";

        public EquatableGenerationStrategy(IFrameworkSet frameworkSet)
            : base(frameworkSet, InterfaceNameForMatch)
        {
        }

        public override int MinimumRequiredGenericParameterCount => 1;

        protected override NameResolver GeneratedMethodNamePattern => FrameworkSet.NamingProvider.ImplementsIEquatable;

        protected override void AddBodyStatements(ClassModel sourceModel, IInterfaceModel interfaceModel, SectionedMethodHandler method)
        {
            if (sourceModel is null)
            {
                throw new ArgumentNullException(nameof(sourceModel));
            }

            if (interfaceModel is null)
            {
                throw new ArgumentNullException(nameof(interfaceModel));
            }

            ITypeSymbol equatableTypeSymbol = interfaceModel.GenericTypes.First();

            var differentExpression = AssignmentValueHelper.GetDefaultAssignmentValue(equatableTypeSymbol, sourceModel.SemanticModel, FrameworkSet);
            ExpressionSyntax sameExpression;
            if (equatableTypeSymbol == sourceModel.TypeSymbol)
            {
                sameExpression = sourceModel.GetObjectCreationExpression(FrameworkSet, false);
            }
            else
            {
                sameExpression = AssignmentValueHelper.GetDefaultAssignmentValue(equatableTypeSymbol, sourceModel.SemanticModel, FrameworkSet);
            }

            method.Arrange(Generate.VariableDeclaration(equatableTypeSymbol, FrameworkSet, "same", sameExpression));
            method.Arrange(Generate.VariableDeclaration(equatableTypeSymbol, FrameworkSet, "different", differentExpression));

            var same = SyntaxFactory.IdentifierName("same");
            var different = SyntaxFactory.IdentifierName("different");

            var objType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword));

            method.Assert(FrameworkSet.AssertionFramework.AssertFalse(Generate.Invocation(sourceModel.TargetInstance, "Equals", SyntaxFactory.DefaultExpression(objType))));
            method.Assert(FrameworkSet.AssertionFramework.AssertFalse(Generate.Invocation(sourceModel.TargetInstance, "Equals", SyntaxFactory.ObjectCreationExpression(objType).WithArgumentList(SyntaxFactory.ArgumentList()))));
            method.Assert(FrameworkSet.AssertionFramework.AssertTrue(Generate.Invocation(sourceModel.TargetInstance, "Equals", SyntaxFactory.CastExpression(objType, same))));
            method.Assert(FrameworkSet.AssertionFramework.AssertFalse(Generate.Invocation(sourceModel.TargetInstance, "Equals", SyntaxFactory.CastExpression(objType, different))));

            method.Assert(FrameworkSet.AssertionFramework.AssertTrue(Generate.Invocation(sourceModel.TargetInstance, "Equals", same)));
            method.Assert(FrameworkSet.AssertionFramework.AssertFalse(Generate.Invocation(sourceModel.TargetInstance, "Equals", different)));

            method.Assert(FrameworkSet.AssertionFramework.AssertEqual(Generate.Invocation(sourceModel.TargetInstance, "GetHashCode"), Generate.Invocation(same, "GetHashCode"), false));
            method.Assert(FrameworkSet.AssertionFramework.AssertNotEqual(Generate.Invocation(sourceModel.TargetInstance, "GetHashCode"), Generate.Invocation(different, "GetHashCode"), false));

            method.Assert(FrameworkSet.AssertionFramework.AssertTrue(SyntaxFactory.BinaryExpression(SyntaxKind.EqualsExpression, sourceModel.TargetInstance, same)));
            method.Assert(FrameworkSet.AssertionFramework.AssertFalse(SyntaxFactory.BinaryExpression(SyntaxKind.EqualsExpression, sourceModel.TargetInstance, different)));

            method.Assert(FrameworkSet.AssertionFramework.AssertFalse(SyntaxFactory.BinaryExpression(SyntaxKind.NotEqualsExpression, sourceModel.TargetInstance, same)));
            method.Assert(FrameworkSet.AssertionFramework.AssertTrue(SyntaxFactory.BinaryExpression(SyntaxKind.NotEqualsExpression, sourceModel.TargetInstance, different)));
        }
    }
}