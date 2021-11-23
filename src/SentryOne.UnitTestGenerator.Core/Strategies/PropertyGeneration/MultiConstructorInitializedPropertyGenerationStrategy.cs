namespace SentryOne.UnitTestGenerator.Core.Strategies.PropertyGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;

    public class MultiConstructorInitializedPropertyGenerationStrategy : IGenerationStrategy<IPropertyModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public MultiConstructorInitializedPropertyGenerationStrategy(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        public bool IsExclusive => false;

        public int Priority => 3;

        public bool CanHandle(IPropertyModel property, ClassModel model)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (property.IsStatic)
            {
                return false;
            }

            var constructorCount = model.Constructors.Count(x => x.Parameters.Any(p => string.Equals(p.Name, property.Name, StringComparison.OrdinalIgnoreCase)));

            if (constructorCount == 0)
            {
                return false;
            }

            var isSingleConstructorProperty = constructorCount == 1 &&
                   model.DefaultConstructor != null && model.DefaultConstructor.Parameters.Any(p => string.Equals(p.Name, property.Name, StringComparison.OrdinalIgnoreCase));

            return !isSingleConstructorProperty;
        }

        public IEnumerable<MethodDeclarationSyntax> Create(IPropertyModel property, ClassModel model)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var methodName = string.Format(CultureInfo.InvariantCulture, "{0}IsInitializedCorrectly", property.Name);
            var method = _frameworkSet.TestFramework.CreateTestMethod(methodName, false, model.IsStatic)
                .AddBodyStatements(GetPropertyAssertionBodyStatements(property, model).ToArray());

            yield return method;
        }

        private IEnumerable<StatementSyntax> GetPropertyAssertionBodyStatements(IPropertyModel property, ClassModel model)
        {
            foreach (var targetConstructor in model.Constructors.Where(x => x.Parameters.Any(p => string.Equals(p.Name, property.Name, StringComparison.OrdinalIgnoreCase))))
            {
                var tokenList = new List<SyntaxNodeOrToken>();

                foreach (var parameter in targetConstructor.Parameters)
                {
                    if (tokenList.Count > 0)
                    {
                        tokenList.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
                    }

                    tokenList.Add(SyntaxFactory.Argument(model.GetConstructorFieldReference(parameter, _frameworkSet)));
                }

                var objectCreation = SyntaxFactory.ObjectCreationExpression(model.TypeSyntax).WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList<ArgumentSyntax>(tokenList)));

                var assignment = SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    model.TargetInstance,
                    objectCreation);

                yield return SyntaxFactory.ExpressionStatement(assignment);

                var parameterToCheck = model.Constructors.SelectMany(x => x.Parameters).First(x => string.Equals(x.Name, property.Name, StringComparison.OrdinalIgnoreCase));

                yield return _frameworkSet.TestFramework.AssertEqual(property.Access(model.TargetInstance), model.GetConstructorFieldReference(parameterToCheck, _frameworkSet));
            }
        }
    }
}