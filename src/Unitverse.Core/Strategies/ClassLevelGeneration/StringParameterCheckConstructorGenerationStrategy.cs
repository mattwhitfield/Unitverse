namespace Unitverse.Core.Strategies.ClassLevelGeneration
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
                object?[] testValues = isNonNullable ? new object?[] { string.Empty, "   " } : new object?[] { null, string.Empty, "   " };
                var generatedMethod = FrameworkSet.CreateTestCaseMethod(FrameworkSet.NamingProvider.CannotConstructWithInvalid, namingContext, false, false, SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)), testValues, "Checks that the constructor throws when the " + nullableParameter + " parameter is null, empty or white space.");

                foreach (var constructorModel in model.Constructors.Where(x => x.Parameters.Any(p => string.Equals(p.Name, nullableParameter, StringComparison.OrdinalIgnoreCase))))
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