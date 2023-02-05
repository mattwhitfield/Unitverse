namespace Unitverse.Core.Generation
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
    using Unitverse.Core.Strategies.ClassGeneration;

    internal static class TypeDeclarationFactory
    {
        public static TypeDeclarationSyntax GetOrCreateTargetType(SyntaxNode targetNamespace, IFrameworkSet frameworkSet, ClassModel classModel, out TypeDeclarationSyntax? originalTargetType)
        {
            TypeDeclarationSyntax? targetType = null;
            originalTargetType = null;

            if (targetNamespace != null)
            {
                var types = TestableItemExtractor.GetTypeDeclarations(targetNamespace);

                var targetClassName = frameworkSet.GetTargetTypeName(classModel);
                originalTargetType = targetType = types.FirstOrDefault(x => string.Equals(x.GetClassName(), targetClassName, StringComparison.OrdinalIgnoreCase));
            }

            if (targetType == null)
            {
                targetType = new ClassGenerationStrategyFactory(frameworkSet).CreateFor(classModel);
            }
            else
            {
                targetType = EnsureAllConstructorParametersHaveFields(frameworkSet, classModel, targetType);
            }

            return targetType;
        }

        private static TypeDeclarationSyntax EnsureAllConstructorParametersHaveFields(IFrameworkSet frameworkSet, ClassModel classModel, TypeDeclarationSyntax targetType)
        {
            var setupMethod = frameworkSet.CreateSetupMethod(frameworkSet.GetTargetTypeName(classModel), classModel.ClassName);

            BaseMethodDeclarationSyntax? foundMethod = null;
            if (setupMethod.Method is MethodDeclarationSyntax methodSyntax)
            {
                foundMethod = targetType.Members.OfType<MethodDeclarationSyntax>().FirstOrDefault(x => x.Identifier.Text == methodSyntax.Identifier.Text && x.ParameterList.Parameters.Count == 0);
            }
            else if (setupMethod.Method is ConstructorDeclarationSyntax)
            {
                foundMethod = targetType.Members.OfType<ConstructorDeclarationSyntax>().FirstOrDefault(x => x.ParameterList.Parameters.Count == 0);
            }

            if (foundMethod != null)
            {
                var updatedMethod = foundMethod;

                var parametersEmitted = new HashSet<string>();
                var allFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                var fields = new List<FieldDeclarationSyntax>();
                foreach (var parameterModel in classModel.Constructors.SelectMany(x => x.Parameters))
                {
                    allFields.Add(classModel.GetConstructorParameterFieldName(parameterModel, frameworkSet));
                }

                var autoFixtureFieldName = frameworkSet.NamingProvider.AutoFixtureFieldName.Resolve(new NamingContext(classModel.ClassName));
                var autoFixtureFieldExists = targetType.Members.OfType<FieldDeclarationSyntax>().Any(x => x.Declaration.Variables.Any(v => v.Identifier.Text == autoFixtureFieldName));

                if (!autoFixtureFieldExists)
                {
                    var defaultExpression = AutoFixtureHelper.GetCreationExpression(frameworkSet.Options.GenerationOptions);
                    updatedMethod = UpdateMethod(updatedMethod, allFields, fields, autoFixtureFieldName, AutoFixtureHelper.TypeSyntax, defaultExpression);
                }

                // generate fields for each constructor parameter that doesn't have an existing field
                foreach (var parameterModel in classModel.Constructors.SelectMany(x => x.Parameters))
                {
                    if (!parametersEmitted.Add(parameterModel.Name))
                    {
                        continue;
                    }

                    var fieldName = classModel.GetConstructorParameterFieldName(parameterModel, frameworkSet);

                    var fieldExists = targetType.Members.OfType<FieldDeclarationSyntax>().Any(x => x.Declaration.Variables.Any(v => v.Identifier.Text == fieldName));

                    if (!fieldExists)
                    {
                        var fieldTypeSyntax = parameterModel.TypeInfo.ToTypeSyntax(frameworkSet.Context);
                        ExpressionSyntax defaultExpression;

                        if (parameterModel.TypeInfo.IsInterface() && !parameterModel.TypeInfo.IsWellKnownSequenceInterface())
                        {
                            frameworkSet.Context.InterfacesMocked++;
                            fieldTypeSyntax = frameworkSet.MockingFramework.GetFieldType(fieldTypeSyntax);
                            defaultExpression = frameworkSet.MockingFramework.GetFieldInitializer(parameterModel.TypeInfo.ToTypeSyntax(frameworkSet.Context));
                        }
                        else
                        {
                            defaultExpression = AssignmentValueHelper.GetDefaultAssignmentValue(parameterModel.TypeInfo, classModel.SemanticModel, frameworkSet);
                        }

                        updatedMethod = UpdateMethod(updatedMethod, allFields, fields, fieldName, fieldTypeSyntax, defaultExpression);
                    }
                }

                if (fields.Any())
                {
                    if (frameworkSet.Options.GenerationOptions.UseAutoFixture && !frameworkSet.Options.GenerationOptions.UseFieldForAutoFixture)
                    {
                        var fixtureAssignment = foundMethod.Body?.Statements.OfType<LocalDeclarationStatementSyntax>().FirstOrDefault(x => x.Declaration.Variables.Any(v => v.Identifier.Text == "fixture"));
                        if (fixtureAssignment == null)
                        {
                            updatedMethod = UpdateMethod(updatedMethod, allFields, AutoFixtureHelper.VariableDeclaration(frameworkSet.Options.GenerationOptions), true);
                        }
                    }

                    targetType = targetType.ReplaceNode(foundMethod, updatedMethod);
                    var existingField = targetType.Members.OfType<FieldDeclarationSyntax>().LastOrDefault();
                    if (existingField != null)
                    {
                        targetType = targetType.InsertNodesAfter(existingField, fields);
                    }
                    else
                    {
                        targetType = targetType.AddMembers(fields.OfType<MemberDeclarationSyntax>().ToArray());
                    }
                }
            }

            return targetType;
        }

        private static BaseMethodDeclarationSyntax UpdateMethod(BaseMethodDeclarationSyntax updatedMethod, HashSet<string> allFields, List<FieldDeclarationSyntax> fields, string fieldName, TypeSyntax fieldTypeSyntax, ExpressionSyntax defaultExpression)
        {
            var variable = SyntaxFactory.VariableDeclaration(fieldTypeSyntax)
                                        .AddVariables(SyntaxFactory.VariableDeclarator(fieldName));
            var field = SyntaxFactory.FieldDeclaration(variable)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));

            fields.Add(field);

            var statement = Helpers.Generate.Statement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName(fieldName), defaultExpression));

            return UpdateMethod(updatedMethod, allFields, statement, true);
        }

        private static BaseMethodDeclarationSyntax UpdateMethod(BaseMethodDeclarationSyntax updatedMethod, HashSet<string> allFields, StatementSyntax statement, bool first = false)
        {
            var body = updatedMethod.Body ?? SyntaxFactory.Block();

            SyntaxList<StatementSyntax> newStatements;
            if (first)
            {
                if (body.Statements.Count > 0)
                {
                    newStatements = body.Statements.Insert(0, statement);
                }
                else
                {
                    newStatements = body.Statements.Add(statement);
                }
            }
            else
            {
                var index = body.Statements.LastIndexOf(x => x.DescendantNodes().OfType<AssignmentExpressionSyntax>().Any(a => a.Left is IdentifierNameSyntax identifierName && allFields.Contains(identifierName.Identifier.Text)));
                if (index >= 0 && index < body.Statements.Count - 1)
                {
                    newStatements = body.Statements.Insert(index + 1, statement);
                }
                else
                {
                    newStatements = body.Statements.Add(statement);
                }
            }

            return updatedMethod.WithBody(body.WithStatements(newStatements));
        }
    }
}
