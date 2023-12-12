namespace Unitverse.Core.Strategies.ClassGeneration
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

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

            if (model.Declaration.Modifiers.Any(x => string.Equals(x.Text, "static", StringComparison.OrdinalIgnoreCase) ||
                                                     string.Equals(x.Text, "abstract", StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            if (_frameworkSet.Options.GenerationOptions.EmitSubclassForProtectedMethods)
            {
                return model.Declaration.Modifiers.Any(m => m.IsKind(SyntaxKind.SealedKeyword)) || (model.Methods.All(x => !(x.MarkedForGeneration && x.Node.Modifiers.Any(m => m.IsKind(SyntaxKind.ProtectedKeyword)))) && model.Properties.All(x => !(x.MarkedForGeneration && x.Node.Modifiers.Any(m => m.IsKind(SyntaxKind.ProtectedKeyword)))));
            }

            return true;
        }

        public ClassDeclarationSyntax Create(ClassModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var targetTypeName = _frameworkSet.GetTargetTypeName(model);
            var classDeclaration = SyntaxFactory.ClassDeclaration(targetTypeName);

            classDeclaration = classDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            classDeclaration = _frameworkSet.ApplyTestClassAttribute(classDeclaration);

            var variableDeclaration = SyntaxFactory.VariableDeclaration(model.TypeSyntax)
                .AddVariables(SyntaxFactory.VariableDeclarator(model.TargetFieldName));

            var fieldDeclaration = SyntaxFactory.FieldDeclaration(variableDeclaration)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
            classDeclaration = classDeclaration.AddMembers(fieldDeclaration);

            if (_frameworkSet.Options.GenerationOptions.UseFieldForAutoFixture)
            {
                var autoFixtureFieldName = _frameworkSet.NamingProvider.AutoFixtureFieldName.Resolve(new NamingContext(model.ClassName));
                var variable = SyntaxFactory.VariableDeclaration(AutoFixtureHelper.TypeSyntax)
                            .AddVariables(SyntaxFactory.VariableDeclarator(autoFixtureFieldName));
                var field = SyntaxFactory.FieldDeclaration(variable)
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
                classDeclaration = classDeclaration.AddMembers(field);
            }

            var setupMethod = Generate.SetupMethod(model, targetTypeName, _frameworkSet, ref classDeclaration);

            var creationExpression = model.GetObjectCreationExpression(_frameworkSet, true);

            var assignment = SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                model.TargetInstance,
                creationExpression);

            setupMethod.Emit(Generate.Statement(assignment));

            classDeclaration = classDeclaration.AddMembers(setupMethod.Method);

            return classDeclaration;
        }
    }
}