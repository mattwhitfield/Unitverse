namespace SentryOne.UnitTestGenerator.Core.Strategies.ClassLevelGeneration
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
    using SentryOne.UnitTestGenerator.Core.Resources;

    public class StringParameterCheckConstructorGenerationStrategy : IGenerationStrategy<ClassModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public StringParameterCheckConstructorGenerationStrategy(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        public bool IsExclusive => false;

        public int Priority => 1;

        public bool CanHandle(ClassModel method, ClassModel model)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return model.Constructors.SelectMany(x => x.Parameters).Any(x => x.TypeInfo.Type.IsReferenceType && x.TypeInfo.Type.SpecialType == SpecialType.System_String) && !model.IsStatic;
        }

        public IEnumerable<MethodDeclarationSyntax> Create(ClassModel method, ClassModel model)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var nullableParameters = new HashSet<string>(model.Constructors.SelectMany(x => x.Parameters).Where(x => x.TypeInfo.Type.SpecialType == SpecialType.System_String).Select(x => x.Name), StringComparer.OrdinalIgnoreCase);

            foreach (var nullableParameter in nullableParameters)
            {
                var methodName = string.Format(CultureInfo.InvariantCulture, "CannotConstructWithInvalid{0}", nullableParameter.ToPascalCase());
                var generatedMethod = _frameworkSet.TestFramework.CreateTestCaseMethod(methodName, false, false, SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)), new object[] { null, string.Empty, "   " });

                foreach (var constructorModel in model.Constructors.Where(x => x.Parameters.Any(p => string.Equals(p.Name, nullableParameter, StringComparison.OrdinalIgnoreCase))))
                {
                    var paramExpressions = constructorModel.Parameters.Select(param => string.Equals(param.Name, nullableParameter, StringComparison.OrdinalIgnoreCase) ? SyntaxFactory.IdentifierName(Strings.MsTestTestFramework_CreateTestCaseMethod_value) : AssignmentValueHelper.GetDefaultAssignmentValue(param.TypeInfo, model.SemanticModel, _frameworkSet)).ToList();
                    var methodCall = Generate.ObjectCreation(model.TypeSyntax, paramExpressions.ToArray());
                    generatedMethod = generatedMethod.AddBodyStatements(_frameworkSet.AssertionFramework.AssertThrows(SyntaxFactory.IdentifierName("ArgumentNullException"), methodCall));
                }

                yield return generatedMethod;
            }
        }
    }
}