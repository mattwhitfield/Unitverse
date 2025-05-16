namespace Unitverse.Core.Frameworks.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;

    public abstract class BaseTestFramework
    {
        protected BaseTestFramework(IUnitTestGeneratorOptions options)
        {
            Options = options;
        }

        public abstract bool SupportsStaticTestClasses { get; }

        protected abstract string TestAttributeName { get; }

        protected abstract string TestCaseMethodAttributeName { get; }

        protected abstract string TestCaseAttributeName { get; }

        public abstract bool SupportsReadonlyFields { get; }

        protected abstract BaseMethodDeclarationSyntax CreateSetupMethodSyntax(string targetTypeName);

        public IUnitTestGeneratorOptions Options { get; }

        protected MethodDeclarationSyntax AddXmlCommentsIfConfigured(MethodDeclarationSyntax method, string description, string? parameterName = null, string? parameterDescription = null)
        {
            if (Options.GenerationOptions.EmitXmlDocumentation)
            {
                IEnumerable<XmlElementSyntax> Elements()
                {
                    yield return XmlCommentHelper.Summary(XmlCommentHelper.TextLiteral(description));
                    if (parameterName != null && parameterDescription != null && !string.IsNullOrWhiteSpace(parameterName) && !string.IsNullOrWhiteSpace(parameterDescription))
                    {
                        yield return XmlCommentHelper.Param(parameterName, parameterDescription);
                    }

                    if (method.Modifiers.Any(x => x.IsKind(SyntaxKind.AsyncKeyword)))
                    {
                        yield return XmlCommentHelper.Returns("A task that represents the running test.");
                    }
                }

                var documentation = XmlCommentHelper.DocumentationComment(Elements());
                return method.WithXmlDocumentation(documentation);
            }

            return method;
        }

        public SectionedMethodHandler CreateSetupMethod(string targetTypeName, string className)
        {
            if (string.IsNullOrWhiteSpace(targetTypeName))
            {
                throw new ArgumentNullException(nameof(targetTypeName));
            }

            var method = CreateSetupMethodSyntax(targetTypeName);

            if (Options.GenerationOptions.EmitXmlDocumentation)
            {
                var documentation = XmlCommentHelper.DocumentationComment(
                    XmlCommentHelper.Summary(
                        XmlCommentHelper.TextLiteral("Sets up the dependencies required for the tests for "),
                        XmlCommentHelper.See(className),
                        XmlCommentHelper.TextLiteral(".")));
                method = method.WithXmlDocumentation(documentation);
            }

            return new SectionedMethodHandler(method, Options.GenerationOptions);
        }

        public SectionedMethodHandler CreateTestMethod(NameResolver nameResolver, NamingContext namingContext, IGenerationContext generationContext, bool isAsync, bool isStatic, string description)
        {
            if (nameResolver is null)
            {
                throw new ArgumentNullException(nameof(nameResolver));
            }

            if (namingContext is null)
            {
                throw new ArgumentNullException(nameof(namingContext));
            }

            bool canBeStatic = TestCanBeStatic(generationContext, isStatic);
            string name = GetTestMethodName(nameResolver, namingContext, generationContext, isAsync);

            var method = Generate.Method(name, isAsync, canBeStatic)
                                 .AddAttributeLists(Generate.Attribute(TestAttributeName).AsList());

            method = AddXmlCommentsIfConfigured(method, description);

            return new SectionedMethodHandler(method, Options.GenerationOptions, Options.GenerationOptions.ArrangeComment, Options.GenerationOptions.ActComment, Options.GenerationOptions.AssertComment);
        }

        public SectionedMethodHandler CreateTestCaseMethod(NameResolver nameResolver, NamingContext namingContext, IGenerationContext generationContext, bool isAsync, bool isStatic, TypeSyntax valueType, IEnumerable<object?> testValues, string description)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException(nameof(valueType));
            }

            if (testValues == null)
            {
                throw new ArgumentNullException(nameof(testValues));
            }

            if (nameResolver is null)
            {
                throw new ArgumentNullException(nameof(nameResolver));
            }

            if (namingContext is null)
            {
                throw new ArgumentNullException(nameof(namingContext));
            }

            bool canBeStatic = TestCanBeStatic(generationContext, isStatic);
            string name = GetTestMethodName(nameResolver, namingContext, generationContext, isAsync);

            var method = Generate.Method(name, isAsync, canBeStatic);

            method = method.AddParameterListParameters(Generate.Parameter("value").WithType(valueType));
            if (!string.IsNullOrWhiteSpace(TestCaseMethodAttributeName))
            {
                method = method.AddAttributeLists(Generate.Attribute(TestCaseMethodAttributeName).AsList());
            }

            foreach (var testValue in testValues)
            {
                method = method.AddAttributeLists(Generate.Attribute(TestCaseAttributeName, testValue).AsList());
            }

            method = AddXmlCommentsIfConfigured(method, description, "value", "The parameter that receives the test case values.");

            return new SectionedMethodHandler(method, Options.GenerationOptions, Options.GenerationOptions.ArrangeComment, Options.GenerationOptions.ActComment, Options.GenerationOptions.AssertComment);
        }

        private static string GetTestMethodName(NameResolver nameResolver, NamingContext namingContext, IGenerationContext generationContext, bool isAsync)
        {
            var name = nameResolver.Resolve(namingContext);
            if (isAsync)
            {
                if (generationContext.NamingProvider.ForceAsyncSuffix &&
                    !name.EndsWith("Async", StringComparison.OrdinalIgnoreCase))
                {
                    name += "Async";
                }
            }

            return name;
        }

        private bool TestCanBeStatic(IGenerationContext generationContext, bool isStatic)
        {
            var canBeStatic = isStatic && SupportsStaticTestClasses;
            if (Options.GenerationOptions.UseFieldForAutoFixture)
            {
                var modelIsStatic = generationContext.CurrentModel?.IsStatic ?? false;
                if (!modelIsStatic)
                {
                    canBeStatic = false;
                }

                if (!string.IsNullOrWhiteSpace(Options.GenerationOptions.TestTypeBaseClass))
                {
                    canBeStatic = false;
                }
            }

            return canBeStatic;
        }
    }
}
