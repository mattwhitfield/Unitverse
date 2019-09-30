namespace SentryOne.UnitTestGenerator.Core.Strategies.PropertyGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Assets;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Resources;

    public class NotifyPropertyChangedGenerationStrategy : IGenerationStrategy<IPropertyModel>
    {
        private static readonly IList<SpecialType> SupportedTypes = new[] { SpecialType.System_String, SpecialType.System_Boolean, SpecialType.System_Byte, SpecialType.System_Int16, SpecialType.System_Int32, SpecialType.System_Int64, SpecialType.System_Decimal, SpecialType.System_DateTime };
        private readonly IFrameworkSet _frameworkSet;

        public NotifyPropertyChangedGenerationStrategy(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        public bool IsExclusive => true;

        public int Priority => 2;

        public bool CanHandle(IPropertyModel property, ClassModel model)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var classImplementsNotifyPropertyChanged = model.Declaration.BaseList?.Types.Any(t => t.Type.GetText().ToString().IndexOf("INotifyPropertyChanged", StringComparison.Ordinal) >= 0) ?? false;
            return classImplementsNotifyPropertyChanged && property.HasGet && property.HasSet;
        }

        public IEnumerable<MethodDeclarationSyntax> Create(IPropertyModel property, ClassModel model)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var withDefaults = !SupportedTypes.Contains(property.TypeInfo.Type.SpecialType);

            model.RequiredAssets.Add(TargetAsset.PropertyTester);

            var methodName = string.Format(CultureInfo.InvariantCulture, "CanSetAndGet{0}", property.Name);
            var method = _frameworkSet.TestFramework.CreateTestMethod(methodName, false, model.IsStatic)
                .AddBodyStatements(GetPropertyAssertionBodyStatements(property, model, withDefaults).ToArray());

            yield return method;
        }

        private IEnumerable<StatementSyntax> GetPropertyAssertionBodyStatements(IPropertyModel property, ClassModel sourceModel, bool withDefaults)
        {
            var propertyLambda = SyntaxFactory.Argument(
                SyntaxFactory.SimpleLambdaExpression(
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier(Strings.Identifier_x)),
                    SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName(Strings.Identifier_x), SyntaxFactory.IdentifierName(property.Name))));

            var argumentList = new List<SyntaxNodeOrToken> { propertyLambda };

            if (withDefaults)
            {
                argumentList.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
                argumentList.Add(SyntaxFactory.Argument(AssignmentValueHelper.GetDefaultAssignmentValue(property.TypeInfo, sourceModel.SemanticModel, _frameworkSet)));
                argumentList.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
                argumentList.Add(SyntaxFactory.Argument(AssignmentValueHelper.GetDefaultAssignmentValue(property.TypeInfo, sourceModel.SemanticModel, _frameworkSet)));
            }

            yield return SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("_testClass"), SyntaxFactory.IdentifierName("CheckProperty")))
                    .WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList<ArgumentSyntax>(argumentList))));
        }
    }
}