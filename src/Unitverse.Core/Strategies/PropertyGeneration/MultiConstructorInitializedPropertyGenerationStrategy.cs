﻿namespace Unitverse.Core.Strategies.PropertyGeneration
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

    public class MultiConstructorInitializedPropertyGenerationStrategy : IGenerationStrategy<IPropertyModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public MultiConstructorInitializedPropertyGenerationStrategy(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        public bool IsExclusive => false;

        public int Priority => 3;

        public Func<IStrategyOptions, bool> IsEnabled => x => x.InitializedPropertyChecksAreEnabled;

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

        public IEnumerable<MethodDeclarationSyntax> Create(IPropertyModel property, ClassModel model, NamingContext namingContext)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var method = _frameworkSet.TestFramework.CreateTestMethod(_frameworkSet.NamingProvider.IsInitializedCorrectly, namingContext, false, model.IsStatic, "Checks that the " + property.Name + " property is initialized correctly by the constructors.");
            method.Emit(GetPropertyAssertionBodyStatements(property, model).ToArray());

            yield return method.Method;
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

                yield return _frameworkSet.AssertionFramework.AssertEqual(property.Access(model.TargetInstance), model.GetConstructorFieldReference(parameterToCheck, _frameworkSet), parameterToCheck.TypeInfo.Type.IsReferenceTypeAndNotString());
            }
        }
    }
}