namespace Unitverse.Core.Strategies.ClassLevelGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public class StringParameterCheckConstructorGenerationStrategy : ParameterCheckConstructorGenerationStrategyBase
    {
        public StringParameterCheckConstructorGenerationStrategy(IFrameworkSet frameworkSet)
            : base(frameworkSet)
        {
        }

        protected override bool CanHandleParameter(ParameterModel parameter)
        {
            return parameter.TypeInfo.Type != null && parameter.TypeInfo.Type.IsReferenceType && parameter.TypeInfo.Type.SpecialType == SpecialType.System_String;
        }

        public override IEnumerable<SectionedMethodHandler> Create(ClassModel method, ClassModel model, NamingContext namingContext)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var nullableParameters = new HashSet<string>(model.Constructors.SelectMany(x => x.Parameters).Where(x => x.TypeInfo.Type != null && x.TypeInfo.Type.SpecialType == SpecialType.System_String).Select(x => x.Name), StringComparer.OrdinalIgnoreCase);

            foreach (var nullableParameter in nullableParameters)
            {
                namingContext = namingContext.WithParameterName(nullableParameter.ToPascalCase());
                var isNonNullable = model.Constructors.SelectMany(x => x.Parameters.Where(p => string.Equals(p.Name, nullableParameter, StringComparison.OrdinalIgnoreCase))).All(x => x.IsNullableTypeSyntax || x.HasNullDefaultValue);

                var stringKeyword = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword));

                var constructors = model.Constructors.Where(x => x.Parameters.Any(p => string.Equals(p.Name, nullableParameter, StringComparison.OrdinalIgnoreCase)));
                var shouldUseSeparatedNullableChecks = constructors.Any(x => FrameworkSet.Options.GenerationOptions.ShouldUseSeparateChecksForNullAndEmpty(x.Node));

                if (shouldUseSeparatedNullableChecks)
                {
                    if (!isNonNullable)
                    {
                        var nullDescription = "Checks that the constructor throws when the " + nullableParameter + " parameter is null.";
                        var nullMethod = FrameworkSet.CreateTestMethod(FrameworkSet.NamingProvider.CannotConstructWithNull, namingContext, false, false, nullDescription);

                        foreach (var constructorModel in constructors)
                        {
                            var paramExpressions = constructorModel.Parameters.Select(param => string.Equals(param.Name, nullableParameter, StringComparison.OrdinalIgnoreCase) ? SyntaxFactory.DefaultExpression(param.TypeInfo.ToTypeSyntax(FrameworkSet.Context)) : GetFieldReferenceOrNewObjectFor(model, param)).ToList();
                            var methodCall = Generate.ObjectCreation(model.TypeSyntax, paramExpressions.ToArray());
                            nullMethod.Emit(FrameworkSet.AssertionFramework.AssertThrows(SyntaxFactory.IdentifierName("ArgumentNullException"), methodCall, nullableParameter));
                        }

                        yield return nullMethod;
                    }

                    var description = "Checks that the constructor throws when the " + nullableParameter + " parameter is empty or white space.";
                    var generatedMethod = FrameworkSet.CreateTestCaseMethod(FrameworkSet.NamingProvider.CannotConstructWithInvalid, namingContext, false, false, stringKeyword, new object?[] { string.Empty, "   " }, description);

                    foreach (var constructorModel in constructors)
                    {
                        var paramExpressions = constructorModel.Parameters.Select(param => string.Equals(param.Name, nullableParameter, StringComparison.OrdinalIgnoreCase) ? SyntaxFactory.IdentifierName("value") : GetFieldReferenceOrNewObjectFor(model, param)).ToList();
                        var methodCall = Generate.ObjectCreation(model.TypeSyntax, paramExpressions.ToArray());
                        generatedMethod.Emit(FrameworkSet.AssertionFramework.AssertThrows(SyntaxFactory.IdentifierName("ArgumentException"), methodCall, nullableParameter));
                    }

                    yield return generatedMethod;
                }
                else
                {
                    var description = isNonNullable ?
                        "Checks that the constructor throws when the " + nullableParameter + " parameter is empty or white space." :
                        "Checks that the constructor throws when the " + nullableParameter + " parameter is null, empty or white space.";

                    object?[] testValues = isNonNullable ?
                        new object?[] { string.Empty, "   " } :
                        new object?[] { null, string.Empty, "   " };

                    var generatedMethod = FrameworkSet.CreateTestCaseMethod(FrameworkSet.NamingProvider.CannotConstructWithInvalid, namingContext, false, false, stringKeyword, testValues, description);

                    foreach (var constructorModel in constructors)
                    {
                        var paramExpressions = constructorModel.Parameters.Select(param => string.Equals(param.Name, nullableParameter, StringComparison.OrdinalIgnoreCase) ? SyntaxFactory.IdentifierName("value") : GetFieldReferenceOrNewObjectFor(model, param)).ToList();
                        var methodCall = Generate.ObjectCreation(model.TypeSyntax, paramExpressions.ToArray());
                        generatedMethod.Emit(FrameworkSet.AssertionFramework.AssertThrows(SyntaxFactory.IdentifierName("ArgumentNullException"), methodCall, nullableParameter));
                    }

                    yield return generatedMethod;
                }
            }
        }
    }
}