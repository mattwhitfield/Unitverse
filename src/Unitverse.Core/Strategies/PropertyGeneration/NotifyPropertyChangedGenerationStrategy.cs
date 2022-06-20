namespace Unitverse.Core.Strategies.PropertyGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Assets;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

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

        public Func<IStrategyOptions, bool> IsEnabled => x => x.PropertyChecksAreEnabled;

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

            var classImplementsNotifyPropertyChanged = model.TypeSymbol.AllInterfaces.Any(x => string.Equals(x.Name, "INotifyPropertyChanged", StringComparison.Ordinal));
            return classImplementsNotifyPropertyChanged && property.HasGet && property.HasSet;
        }

        public IEnumerable<SectionedMethodHandler> Create(IPropertyModel property, ClassModel model, NamingContext namingContext)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var withDefaults = property.TypeInfo.Type != null && !SupportedTypes.Contains(property.TypeInfo.Type.SpecialType);

            model.RequiredAssets.Add(TargetAsset.PropertyTester);

            var method = _frameworkSet.CreateTestMethod(_frameworkSet.NamingProvider.CanSetAndGet, namingContext, false, model.IsStatic, "Checks that setting the " + property.Name + " property correctly raises PropertyChanged events.");
            method.Emit(GetPropertyAssertionBodyStatements(property, model, withDefaults).ToArray());

            yield return method;
        }

        private IEnumerable<StatementSyntax> GetPropertyAssertionBodyStatements(IPropertyModel property, ClassModel sourceModel, bool withDefaults)
        {
            var propertyLambda = SyntaxFactory.Argument(
                SyntaxFactory.SimpleLambdaExpression(
                    Generate.Parameter("x"),
                    Generate.MemberAccess("x", property.Name)));

            var argumentList = new List<SyntaxNodeOrToken> { propertyLambda };

            if (withDefaults)
            {
                argumentList.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
                argumentList.Add(SyntaxFactory.Argument(AssignmentValueHelper.GetDefaultAssignmentValue(property.TypeInfo, sourceModel.SemanticModel, _frameworkSet)));
                argumentList.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
                argumentList.Add(SyntaxFactory.Argument(AssignmentValueHelper.GetDefaultAssignmentValue(property.TypeInfo, sourceModel.SemanticModel, _frameworkSet)));
            }

            yield return Generate.Statement(
                Generate.MemberInvocation(sourceModel.TargetInstance, "CheckProperty")
                    .WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList<ArgumentSyntax>(argumentList))));
        }
    }
}