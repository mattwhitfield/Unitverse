namespace SentryOne.UnitTestGenerator.Core.Strategies.ClassGeneration
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;

    public class StandardClassGenerationStrategy : IClassGenerationStrategy
    {
        private readonly IFrameworkSet _frameworkSet;

        public StandardClassGenerationStrategy(IFrameworkSet frameworkSet)
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

            return !model.Declaration.Modifiers.Any(x => string.Equals(x.Text, "static", StringComparison.OrdinalIgnoreCase) ||
                                                         string.Equals(x.Text, "abstract", StringComparison.OrdinalIgnoreCase));
        }

        public ClassDeclarationSyntax Create(ClassModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var targetTypeName = _frameworkSet.GetTargetTypeName(model, true);
            var classDeclaration = SyntaxFactory.ClassDeclaration(targetTypeName);

            classDeclaration = classDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            if (!string.IsNullOrWhiteSpace(_frameworkSet.TestFramework.TestClassAttribute))
            {
                var testFixtureAtt = Generate.Attribute(_frameworkSet.TestFramework.TestClassAttribute);
                var list = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(testFixtureAtt));
                classDeclaration = classDeclaration.AddAttributeLists(list);
            }

            var variableDeclaration = SyntaxFactory.VariableDeclaration(model.TypeSyntax)
                .AddVariables(SyntaxFactory.VariableDeclarator("_testClass"));

            var fieldDeclaration = SyntaxFactory.FieldDeclaration(variableDeclaration)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
            classDeclaration = classDeclaration.AddMembers(fieldDeclaration);

            var setupMethod = Generate.SetupMethod(model, targetTypeName, _frameworkSet, ref classDeclaration);

            var creationExpression = model.GetObjectCreationExpression(_frameworkSet);

            var assignment = SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                model.TargetInstance,
                creationExpression);

            setupMethod = setupMethod.AddBodyStatements(SyntaxFactory.ExpressionStatement(assignment));

            classDeclaration = classDeclaration.AddMembers(setupMethod);

            return classDeclaration;
        }
    }
}