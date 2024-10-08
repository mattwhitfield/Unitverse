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

            if (_frameworkSet.Options.GenerationOptions.CanUseAutoFixtureForMocking())
            {
                return false;
            }

            var isSingleConstructorProperty = constructorCount == 1 &&
                   model.DefaultConstructor != null && model.DefaultConstructor.Parameters.Any(p => string.Equals(p.Name, property.Name, StringComparison.OrdinalIgnoreCase));

            return !isSingleConstructorProperty;
        }

        public IEnumerable<SectionedMethodHandler> Create(IPropertyModel property, ClassModel model, NamingContext namingContext)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var method = _frameworkSet.CreateTestMethod(_frameworkSet.NamingProvider.IsInitializedCorrectly, namingContext, false, model.IsStatic, "Checks that the " + property.Name + " property is initialized correctly by the constructors.");
            method.Emit(GetPropertyAssertionBodyStatements(property, model).ToArray());

            yield return method;
        }

        private IEnumerable<StatementSyntax> GetPropertyAssertionBodyStatements(IPropertyModel property, ClassModel model)
        {
            bool declared = false;

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

                if (!declared)
                {
                    yield return Generate.ImplicitlyTypedVariableDeclaration("instance", objectCreation);
                    declared = true;
                }
                else
                {
                    var assignment = SyntaxFactory.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        SyntaxFactory.IdentifierName("instance"),
                        objectCreation);

                    yield return Generate.Statement(assignment);
                }

                var parameterToCheck = model.Constructors.SelectMany(x => x.Parameters).First(x => string.Equals(x.Name, property.Name, StringComparison.OrdinalIgnoreCase));

                yield return _frameworkSet.AssertionFramework.AssertEqual(property.Access(SyntaxFactory.IdentifierName("instance")), model.GetConstructorFieldReference(parameterToCheck, _frameworkSet), parameterToCheck.TypeInfo.Type.IsReferenceTypeAndNotString());
            }
        }
    }
}