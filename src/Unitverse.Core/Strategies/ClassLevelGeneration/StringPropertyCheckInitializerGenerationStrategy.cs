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

    public class StringPropertyCheckInitializerGenerationStrategy : IGenerationStrategy<ClassModel>
    {
        private readonly IFrameworkSet _frameworkSet;

        public StringPropertyCheckInitializerGenerationStrategy(IFrameworkSet frameworkSet)
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

            return !model.Constructors.Any() && model.Properties.Any(x => x.TypeInfo.Type.IsReferenceType && x.TypeInfo.Type.SpecialType == SpecialType.System_String && x.HasInit) && !model.IsStatic;
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

            if (namingContext is null)
            {
                throw new ArgumentNullException(nameof(namingContext));
            }

            var initializableProperties = model.Properties.Where(x => x.HasInit).ToList();
            foreach (var property in initializableProperties.Where(x => x.TypeInfo.Type.IsReferenceType && x.TypeInfo.Type.SpecialType == SpecialType.System_String))
            {
                var isNullable = property.Node.Type is NullableTypeSyntax;

                namingContext = namingContext.WithMemberName(property.Name, property.Name);

                object[] testValues = isNullable ? new object[] { string.Empty, "   " } : new object[] { null, string.Empty, "   " };
                var generatedMethod = _frameworkSet.TestFramework.CreateTestCaseMethod(_frameworkSet.NamingProvider.CannotInitializeWithInvalid, namingContext, false, false, SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)), testValues);

                ExpressionSyntax GetAssignedValue(IPropertyModel propertyModel)
                {
                    if (string.Equals(propertyModel.Name, property.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        return SyntaxFactory.IdentifierName("value");
                    }

                    return model.GetConstructorFieldReference(propertyModel, _frameworkSet);
                }

                var methodCall = Generate.ObjectCreation(model.TypeSyntax, initializableProperties.Select(x => Generate.Assignment(x.Name, GetAssignedValue(x))));
                generatedMethod = generatedMethod.AddBodyStatements(_frameworkSet.AssertionFramework.AssertThrows(SyntaxFactory.IdentifierName("ArgumentNullException"), methodCall));

                yield return generatedMethod;
            }
        }
    }
}