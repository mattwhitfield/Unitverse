namespace Unitverse.Core.Strategies.ClassGeneration
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public class StaticClassGenerationStrategy : IClassGenerationStrategy
    {
        private readonly IFrameworkSet _frameworkSet;

        public StaticClassGenerationStrategy(IFrameworkSet frameworkSet)
        {
            _frameworkSet = frameworkSet ?? throw new ArgumentNullException(nameof(frameworkSet));
        }

        public int Priority => 1;

        public bool CanHandle(ClassModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return model.Declaration.Modifiers.Any(x => string.Equals(x.Text, "static", StringComparison.OrdinalIgnoreCase));
        }

        public ClassDeclarationSyntax Create(ClassModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var classDeclaration = SyntaxFactory.ClassDeclaration(_frameworkSet.GetTargetTypeName(model));

            model.TargetInstance = model.TypeSyntax;

            classDeclaration = classDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var isStatic = false;
            if (_frameworkSet.TestFramework.SupportsStaticTestClasses &&
                string.IsNullOrWhiteSpace(_frameworkSet.Options.GenerationOptions.TestTypeBaseClass))
            {
                classDeclaration = classDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
                isStatic = true;
            }

            if (_frameworkSet.Options.GenerationOptions.UseFieldForAutoFixture)
            {
                var autoFixtureFieldName = _frameworkSet.NamingProvider.AutoFixtureFieldName.Resolve(new NamingContext(model.ClassName));
                var defaultExpression = AutoFixtureHelper.GetCreationExpression(_frameworkSet.Options.GenerationOptions);
                var variable = SyntaxFactory.VariableDeclaration(AutoFixtureHelper.TypeSyntax)
                            .AddVariables(SyntaxFactory.VariableDeclarator(autoFixtureFieldName)
                                                       .WithInitializer(SyntaxFactory.EqualsValueClause(defaultExpression)));
                var field = SyntaxFactory.FieldDeclaration(variable)
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));

                if (isStatic)
                {
                    field = field.AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
                }

                classDeclaration = classDeclaration.AddMembers(field);
            }

            if (_frameworkSet.TestFramework.TestClassAttributes != null)
            {
                classDeclaration = classDeclaration.AddAttributeLists(_frameworkSet.TestFramework.TestClassAttributes.AsList());
            }

            return classDeclaration;
        }
    }
}