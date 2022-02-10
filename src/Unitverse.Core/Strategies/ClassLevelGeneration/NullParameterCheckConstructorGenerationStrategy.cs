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

    public class NullParameterCheckConstructorGenerationStrategy : IGenerationStrategy<ClassModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public NullParameterCheckConstructorGenerationStrategy(IFrameworkSet frameworkSet)
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

            if (model.Declaration is RecordDeclarationSyntax)
            {
                return false;
            }

            return model.Constructors.SelectMany(x => x.Parameters).Any(x => x.TypeInfo.Type.IsReferenceType && x.TypeInfo.Type.SpecialType != SpecialType.System_String) && !model.IsStatic;
        }

        public IEnumerable<MethodDeclarationSyntax> Create(ClassModel method, ClassModel model, NamingContext namingContext)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var nullableParameters = new HashSet<string>(model.Constructors.SelectMany(x => x.Parameters).Where(x => x.TypeInfo.Type.IsReferenceType && x.TypeInfo.Type.SpecialType != SpecialType.System_String).Select(x => x.Name), StringComparer.OrdinalIgnoreCase);

            foreach (var nullableParameter in nullableParameters)
            {
                var isNonNullable = model.Constructors.SelectMany(x => x.Parameters.Where(p => string.Equals(p.Name, nullableParameter, StringComparison.OrdinalIgnoreCase))).All(x => x.Node.Type is NullableTypeSyntax);

                if (isNonNullable)
                {
                    // all params are explicitly nullable, so skip
                    continue;
                }

                namingContext = namingContext.WithParameterName(nullableParameter.ToPascalCase());

                var generatedMethod = _frameworkSet.TestFramework.CreateTestMethod(_frameworkSet.NamingProvider.CannotConstructWithNull, namingContext, false, false);

                foreach (var constructorModel in model.Constructors.Where(x => x.Parameters.Any(p => string.Equals(p.Name, nullableParameter, StringComparison.OrdinalIgnoreCase))))
                {
                    var constructorParam = constructorModel.Parameters.First(p => string.Equals(p.Name, nullableParameter, StringComparison.OrdinalIgnoreCase));
                    if (constructorParam.Node.Type is NullableTypeSyntax)
                    {
                        continue;
                    }

                    var paramExpressions = constructorModel.Parameters.Select(param => string.Equals(param.Name, nullableParameter, StringComparison.OrdinalIgnoreCase) ? SyntaxFactory.DefaultExpression(param.TypeInfo.ToTypeSyntax(_frameworkSet.Context)) : AssignmentValueHelper.GetDefaultAssignmentValue(param.TypeInfo, model.SemanticModel, _frameworkSet)).ToList();
                    var methodCall = Generate.ObjectCreation(model.TypeSyntax, paramExpressions.ToArray());
                    generatedMethod = generatedMethod.AddBodyStatements(_frameworkSet.AssertionFramework.AssertThrows(SyntaxFactory.IdentifierName("ArgumentNullException"), methodCall));
                }

                yield return generatedMethod;
            }
        }
    }
}