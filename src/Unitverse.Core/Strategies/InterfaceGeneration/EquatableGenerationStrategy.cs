namespace Unitverse.Core.Strategies.InterfaceGeneration
{
    using System;
    using System.Collections.Generic;
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

        protected override IEnumerable<StatementSyntax> GetBodyStatements(ClassModel sourceModel, IInterfaceModel interfaceModel)
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
                sameExpression = sourceModel.GetObjectCreationExpression(FrameworkSet);
            }
            else
            {
                sameExpression = AssignmentValueHelper.GetDefaultAssignmentValue(equatableTypeSymbol, sourceModel.SemanticModel, FrameworkSet);
            }

            yield return Generate.VariableDeclaration(equatableTypeSymbol, FrameworkSet, "same", sameExpression);
            yield return Generate.VariableDeclaration(equatableTypeSymbol, FrameworkSet, "different", differentExpression);

            var same = SyntaxFactory.IdentifierName("same");
            var different = SyntaxFactory.IdentifierName("different");

            var objType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword));

            yield return FrameworkSet.AssertionFramework.AssertFalse(Generate.Invocation(sourceModel.TargetInstance, "Equals", SyntaxFactory.DefaultExpression(objType)));
            yield return FrameworkSet.AssertionFramework.AssertFalse(Generate.Invocation(sourceModel.TargetInstance, "Equals", SyntaxFactory.ObjectCreationExpression(objType).WithArgumentList(SyntaxFactory.ArgumentList())));
            yield return FrameworkSet.AssertionFramework.AssertTrue(Generate.Invocation(sourceModel.TargetInstance, "Equals", SyntaxFactory.CastExpression(objType, same)));
            yield return FrameworkSet.AssertionFramework.AssertFalse(Generate.Invocation(sourceModel.TargetInstance, "Equals", SyntaxFactory.CastExpression(objType, different)));

            yield return FrameworkSet.AssertionFramework.AssertTrue(Generate.Invocation(sourceModel.TargetInstance, "Equals", same));
            yield return FrameworkSet.AssertionFramework.AssertFalse(Generate.Invocation(sourceModel.TargetInstance, "Equals", different));

            yield return FrameworkSet.AssertionFramework.AssertEqual(Generate.Invocation(sourceModel.TargetInstance, "GetHashCode"), Generate.Invocation(same, "GetHashCode"), false);
            yield return FrameworkSet.AssertionFramework.AssertNotEqual(Generate.Invocation(sourceModel.TargetInstance, "GetHashCode"), Generate.Invocation(different, "GetHashCode"), false);

            yield return FrameworkSet.AssertionFramework.AssertTrue(SyntaxFactory.BinaryExpression(SyntaxKind.EqualsExpression, sourceModel.TargetInstance, same));
            yield return FrameworkSet.AssertionFramework.AssertFalse(SyntaxFactory.BinaryExpression(SyntaxKind.EqualsExpression, sourceModel.TargetInstance, different));

            yield return FrameworkSet.AssertionFramework.AssertFalse(SyntaxFactory.BinaryExpression(SyntaxKind.NotEqualsExpression, sourceModel.TargetInstance, same));
            yield return FrameworkSet.AssertionFramework.AssertTrue(SyntaxFactory.BinaryExpression(SyntaxKind.NotEqualsExpression, sourceModel.TargetInstance, different));
        }
    }
}