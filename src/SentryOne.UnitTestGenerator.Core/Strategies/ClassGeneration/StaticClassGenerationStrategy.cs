namespace SentryOne.UnitTestGenerator.Core.Strategies.ClassGeneration
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;

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

            var classDeclaration = SyntaxFactory.ClassDeclaration(_frameworkSet.GetTargetTypeName(model, true));

            model.TargetInstance = model.TypeSyntax;

            classDeclaration = classDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            if (_frameworkSet.TestFramework.SupportsStaticTestClasses)
            {
                classDeclaration = classDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
            }

            if (!string.IsNullOrWhiteSpace(_frameworkSet.TestFramework.TestClassAttribute))
            {
                var testFixtureAtt = Generate.Attribute(_frameworkSet.TestFramework.TestClassAttribute);
                var list = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(testFixtureAtt));
                classDeclaration = classDeclaration.AddAttributeLists(list);
            }

            return classDeclaration;
        }
    }
}