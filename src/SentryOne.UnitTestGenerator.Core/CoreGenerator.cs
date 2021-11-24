namespace SentryOne.UnitTestGenerator.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;
    using SentryOne.UnitTestGenerator.Core.Frameworks;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Options;
    using SentryOne.UnitTestGenerator.Core.Resources;
    using SentryOne.UnitTestGenerator.Core.Strategies;
    using SentryOne.UnitTestGenerator.Core.Strategies.ClassDecoration;
    using SentryOne.UnitTestGenerator.Core.Strategies.ClassGeneration;
    using SentryOne.UnitTestGenerator.Core.Strategies.ClassLevelGeneration;
    using SentryOne.UnitTestGenerator.Core.Strategies.IndexerGeneration;
    using SentryOne.UnitTestGenerator.Core.Strategies.InterfaceGeneration;
    using SentryOne.UnitTestGenerator.Core.Strategies.MethodGeneration;
    using SentryOne.UnitTestGenerator.Core.Strategies.OperatorGeneration;
    using SentryOne.UnitTestGenerator.Core.Strategies.PropertyGeneration;

    public static class CoreGenerator
    {
        public static async Task<GenerationResult> Generate(SemanticModel sourceModel, SyntaxNode sourceSymbol, SemanticModel targetModel, bool withRegeneration, IUnitTestGeneratorOptions options, Func<string, string> nameSpaceTransform)
        {
            if (nameSpaceTransform == null)
            {
                throw new ArgumentNullException(nameof(nameSpaceTransform));
            }

            var sourceNameSpace = await sourceModel.GetNamespace().ConfigureAwait(true);
            var targetNameSpace = nameSpaceTransform(sourceNameSpace);

            var usingsEmitted = new HashSet<string>();

            NamespaceDeclarationSyntax originalTargetNamespace = null;

            CompilationUnitSyntax compilation;
            NamespaceDeclarationSyntax targetNamespace;
            if (targetModel != null)
            {
                var targetTree = await targetModel.SyntaxTree.GetRootAsync().ConfigureAwait(true);
                compilation = targetTree.AncestorsAndSelf().OfType<CompilationUnitSyntax>().FirstOrDefault() ?? SyntaxFactory.CompilationUnit();
                foreach (var syntax in targetTree.DescendantNodes().OfType<UsingDirectiveSyntax>())
                {
                    usingsEmitted.Add(syntax.NormalizeWhitespace().ToFullString());
                }

                originalTargetNamespace = targetNamespace = targetTree.DescendantNodesAndSelf().OfType<NamespaceDeclarationSyntax>().FirstOrDefault(x => string.Equals(x.Name.ToString(), targetNameSpace, StringComparison.OrdinalIgnoreCase)) ?? targetTree.DescendantNodesAndSelf().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
                if (targetNamespace == null)
                {
                    targetNamespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(targetNameSpace));
                }
            }
            else
            {
                compilation = SyntaxFactory.CompilationUnit();
                targetNamespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(targetNameSpace));
            }

            return Generate(sourceModel, sourceSymbol, withRegeneration, options, targetNamespace, usingsEmitted, compilation, originalTargetNamespace);
        }

        private static TypeDeclarationSyntax AddGeneratedItems<T>(ClassModel classModel, TypeDeclarationSyntax declaration, ItemGenerationStrategyFactory<T> factory, Func<ClassModel, IEnumerable<T>> selector, Func<T, bool> shouldGenerate, bool withRegeneration)
        {
            foreach (var property in selector(classModel))
            {
                if (shouldGenerate(property))
                {
                    foreach (var method in factory.CreateFor(property, classModel))
                    {
                        var methodName = method.Identifier.Text;
                        var existingMethod = declaration.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault(x => string.Equals(x.Identifier.Text, methodName, StringComparison.OrdinalIgnoreCase));

                        if (existingMethod != null)
                        {
                            if (!withRegeneration)
                            {
                                throw new InvalidOperationException("One or more of the generated methods ('" + methodName + "') already exists in the test class. In order to over-write existing tests, hold left shift while right-clicking and select the 'Regenerate' option.");
                            }

                            declaration = declaration.ReplaceNode(existingMethod, method);
                        }
                        else
                        {
                            declaration = declaration.AddMembers(method);
                        }
                    }
                }
            }

            return declaration;
        }

        private static CompilationUnitSyntax AddTargetNamespaceToCompilation(NamespaceDeclarationSyntax originalTargetNamespace, CompilationUnitSyntax compilation, NamespaceDeclarationSyntax targetNamespace)
        {
            if (originalTargetNamespace == null)
            {
                compilation = compilation.AddMembers(targetNamespace);
            }
            else
            {
                compilation = compilation.RemoveNode(originalTargetNamespace, SyntaxRemoveOptions.KeepNoTrivia).AddMembers(targetNamespace);
            }

            return compilation;
        }

        private static NamespaceDeclarationSyntax AddTypeParameterAliases(ClassModel classModel, HashSet<string> typeParametersEmitted, NamespaceDeclarationSyntax targetNamespace)
        {
            foreach (var parameter in classModel.Declaration.TypeParameterList?.Parameters ?? Enumerable.Empty<TypeParameterSyntax>())
            {
                var aliasedName = parameter.Identifier.ToString();
                if (targetNamespace.DescendantNodes().OfType<UsingDirectiveSyntax>().Any(node => string.Equals(node.Alias?.Name?.ToString(), aliasedName, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                if (typeParametersEmitted.Add(parameter.Identifier.ValueText))
                {
                    targetNamespace = targetNamespace.AddUsings(
                        SyntaxFactory.UsingDirective(SyntaxFactory.QualifiedName(SyntaxFactory.IdentifierName(Strings.UnitTestGenerator_AddUsingStatements_System), SyntaxFactory.IdentifierName("String")))
                            .WithAlias(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(parameter.Identifier))));
                }
            }

            return targetNamespace;
        }

        private static NamespaceDeclarationSyntax AddTypeToTargetNamespace(TypeDeclarationSyntax originalTargetType, NamespaceDeclarationSyntax targetNamespace, TypeDeclarationSyntax targetType)
        {
            if (originalTargetType != null)
            {
                targetNamespace = targetNamespace.RemoveNode(originalTargetType, SyntaxRemoveOptions.KeepNoTrivia);
            }
            else
            {
                var type = targetNamespace.DescendantNodes().OfType<TypeDeclarationSyntax>().FirstOrDefault(x => x.Identifier.ValueText == targetType.Identifier.ValueText);
                if (type != null)
                {
                    targetNamespace = targetNamespace.RemoveNode(type, SyntaxRemoveOptions.KeepNoTrivia);
                }
            }

            targetNamespace = targetNamespace.AddMembers(targetType);
            return targetNamespace;
        }

        private static NamespaceDeclarationSyntax AddUsingStatements(NamespaceDeclarationSyntax targetNamespace, HashSet<string> usingsEmitted, IFrameworkSet frameworkSet, List<ClassModel> classModels)
        {
            targetNamespace = EmitUsingStatements(targetNamespace, usingsEmitted, SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(Strings.UnitTestGenerator_AddUsingStatements_System)));
            targetNamespace = EmitUsingStatements(targetNamespace, usingsEmitted, frameworkSet.TestFramework.GetUsings());
            targetNamespace = EmitUsingStatements(targetNamespace, usingsEmitted, frameworkSet.AssertionFramework.GetUsings());
            if (frameworkSet.Context.MocksUsed)
            {
                targetNamespace = EmitUsingStatements(targetNamespace, usingsEmitted, frameworkSet.MockingFramework.GetUsings());
            }

            foreach (var emittedType in frameworkSet.Context.EmittedTypes)
            {
                if (emittedType?.ContainingNamespace != null)
                {
                    targetNamespace = EmitUsingStatements(targetNamespace, usingsEmitted, SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(emittedType.ContainingNamespace.ToDisplayString())));
                }
            }

            if (classModels.SelectMany(x => x.Methods).Any(x => x.IsAsync))
            {
                targetNamespace = EmitUsingStatements(targetNamespace, usingsEmitted, SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(typeof(Task).Namespace)));
            }

            return targetNamespace;
        }

        private static TypeDeclarationSyntax ApplyStrategies(bool withRegeneration, TypeDeclarationSyntax targetType, IFrameworkSet frameworkSet, ClassModel classModel)
        {
            targetType = new ClassDecorationStrategyFactory(frameworkSet).Apply(targetType, classModel);

            if (!classModel.IsSingleItem || classModel.Constructors.Any())
            {
                targetType = AddGeneratedItems(classModel, targetType, new ClassLevelGenerationStrategyFactory(frameworkSet), x => new[] { x }, x => x.Constructors.Any(c => c.ShouldGenerate), withRegeneration);
            }

            if (classModel.Interfaces.Count > 0)
            {
                targetType = AddGeneratedItems(classModel, targetType, new InterfaceGenerationStrategyFactory(frameworkSet), x => new[] { x }, x => x.ShouldGenerate, withRegeneration);
            }

            targetType = AddGeneratedItems(classModel, targetType, new MethodGenerationStrategyFactory(frameworkSet), x => x.Methods, x => x.ShouldGenerate, withRegeneration);
            targetType = AddGeneratedItems(classModel, targetType, new OperatorGenerationStrategyFactory(frameworkSet), x => x.Operators, x => x.ShouldGenerate, withRegeneration);
            targetType = AddGeneratedItems(classModel, targetType, new PropertyGenerationStrategyFactory(frameworkSet), x => x.Properties, x => x.ShouldGenerate, withRegeneration);
            targetType = AddGeneratedItems(classModel, targetType, new IndexerGenerationStrategyFactory(frameworkSet), x => x.Indexers, x => x.ShouldGenerate, withRegeneration);
            return targetType;
        }

        private static GenerationResult CreateGenerationResult(CompilationUnitSyntax compilation, List<ClassModel> classModels)
        {
            using (var workspace = new AdhocWorkspace())
            {
                compilation = (CompilationUnitSyntax)Formatter.Format(compilation, workspace);

                var generationResult = new GenerationResult(compilation.ToFullString());
                foreach (var asset in classModels.SelectMany(x => x.RequiredAssets).Distinct())
                {
                    generationResult.RequiredAssets.Add(asset);
                }

                return generationResult;
            }
        }

        private static NamespaceDeclarationSyntax EmitUsingStatements(NamespaceDeclarationSyntax namespaceDeclaration, ISet<string> emittedUsings, params UsingDirectiveSyntax[] usings)
        {
            return EmitUsingStatements(namespaceDeclaration, emittedUsings, usings?.AsEnumerable() ?? Enumerable.Empty<UsingDirectiveSyntax>());
        }

        private static NamespaceDeclarationSyntax EmitUsingStatements(NamespaceDeclarationSyntax namespaceDeclaration, ISet<string> emittedUsings, IEnumerable<UsingDirectiveSyntax> usings)
        {
            foreach (var usingDirective in usings)
            {
                var fullString = usingDirective.NormalizeWhitespace().ToFullString();
                if (emittedUsings.Add(fullString))
                {
                    if (usingDirective.Name is IdentifierNameSyntax ins && ins.Identifier.ValueText.StartsWith("<global", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    namespaceDeclaration = namespaceDeclaration.AddUsings(usingDirective);
                }
            }

            return namespaceDeclaration;
        }

        private static TypeDeclarationSyntax EnsureAllConstructorParametersHaveFields(IFrameworkSet frameworkSet, ClassModel classModel, TypeDeclarationSyntax targetType)
        {
            var setupMethod = frameworkSet.TestFramework.CreateSetupMethod(frameworkSet.GetTargetTypeName(classModel, true));

            BaseMethodDeclarationSyntax foundMethod = null, updatedMethod = null;
            if (setupMethod is MethodDeclarationSyntax methodSyntax)
            {
                updatedMethod = foundMethod = targetType.Members.OfType<MethodDeclarationSyntax>().FirstOrDefault(x => x.Identifier.Text == methodSyntax.Identifier.Text && x.ParameterList.Parameters.Count == 0);
            }
            else if (setupMethod is ConstructorDeclarationSyntax)
            {
                updatedMethod = foundMethod = targetType.Members.OfType<ConstructorDeclarationSyntax>().FirstOrDefault(x => x.ParameterList.Parameters.Count == 0);
            }

            if (foundMethod != null)
            {
                var parametersEmitted = new HashSet<string>();
                var allFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                var fields = new List<FieldDeclarationSyntax>();
                foreach (var parameterModel in classModel.Constructors.SelectMany(x => x.Parameters))
                {
                    allFields.Add(classModel.GetConstructorParameterFieldName(parameterModel));
                }

                // generate fields for each constructor parameter that doesn't have an existing field
                foreach (var parameterModel in classModel.Constructors.SelectMany(x => x.Parameters))
                {
                    if (!parametersEmitted.Add(parameterModel.Name))
                    {
                        continue;
                    }

                    var fieldName = classModel.GetConstructorParameterFieldName(parameterModel);

                    var fieldExists = targetType.Members.OfType<FieldDeclarationSyntax>().Any(x => x.Declaration.Variables.Any(v => v.Identifier.Text == fieldName));

                    if (!fieldExists)
                    {
                        var fieldTypeSyntax = parameterModel.TypeInfo.ToTypeSyntax(frameworkSet.Context);
                        ExpressionSyntax defaultExpression;

                        if (parameterModel.TypeInfo.Type.TypeKind == TypeKind.Interface)
                        {
                            fieldTypeSyntax = frameworkSet.MockingFramework.GetFieldType(fieldTypeSyntax);
                            defaultExpression = frameworkSet.MockingFramework.GetFieldInitializer(parameterModel.TypeInfo.ToTypeSyntax(frameworkSet.Context));
                        }
                        else
                        {
                            defaultExpression = AssignmentValueHelper.GetDefaultAssignmentValue(parameterModel.TypeInfo, classModel.SemanticModel, frameworkSet);
                        }

                        var variable = SyntaxFactory.VariableDeclaration(fieldTypeSyntax)
                            .AddVariables(SyntaxFactory.VariableDeclarator(fieldName));
                        var field = SyntaxFactory.FieldDeclaration(variable)
                            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));

                        fields.Add(field);

                        var statement = SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName(fieldName), defaultExpression));

                        var body = updatedMethod.Body ?? SyntaxFactory.Block();

                        SyntaxList<StatementSyntax> newStatements;
                        var index = body.Statements.LastIndexOf(x => x.DescendantNodes().OfType<AssignmentExpressionSyntax>().Any(a => a.Left is IdentifierNameSyntax identifierName && allFields.Contains(identifierName.Identifier.Text)));
                        if (index >= 0 && index < body.Statements.Count - 1)
                        {
                            newStatements = body.Statements.Insert(index + 1, statement);
                        }
                        else
                        {
                            newStatements = body.Statements.Add(statement);
                        }

                        updatedMethod = updatedMethod.WithBody(body.WithStatements(newStatements));
                    }
                }

                if (fields.Any())
                {
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

        private static GenerationResult Generate(SemanticModel sourceModel, SyntaxNode sourceSymbol, bool withRegeneration, IUnitTestGeneratorOptions options, NamespaceDeclarationSyntax targetNamespace, HashSet<string> usingsEmitted, CompilationUnitSyntax compilation, NamespaceDeclarationSyntax originalTargetNamespace)
        {
            var frameworkSet = FrameworkSetFactory.Create(options);
            var typeParametersEmitted = new HashSet<string>();
            var model = new TestableItemExtractor(sourceModel.SyntaxTree, sourceModel);
            var classModels = model.Extract(sourceSymbol).ToList();

            foreach (var c in classModels)
            {
                if (c.Declaration.Parent is NamespaceDeclarationSyntax namespaceDeclaration)
                {
                    targetNamespace = EmitUsingStatements(targetNamespace, usingsEmitted, SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(namespaceDeclaration.Name.ToString())));
                }
            }

            foreach (var classModel in classModels)
            {
                var targetType = GetOrCreateTargetType(sourceSymbol, targetNamespace, frameworkSet, classModel, out var originalTargetType);

                targetNamespace = AddTypeParameterAliases(classModel, typeParametersEmitted, targetNamespace);

                targetType = ApplyStrategies(withRegeneration, targetType, frameworkSet, classModel);

                targetNamespace = AddTypeToTargetNamespace(originalTargetType, targetNamespace, targetType);

                foreach (var parameter in frameworkSet.Context.GenericTypes)
                {
                    targetNamespace = targetNamespace.AddUsings(
                        SyntaxFactory.UsingDirective(SyntaxFactory.QualifiedName(SyntaxFactory.IdentifierName(Strings.UnitTestGenerator_AddUsingStatements_System), SyntaxFactory.IdentifierName("String")))
                            .WithAlias(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(parameter))));
                }
            }

            targetNamespace = AddUsingStatements(targetNamespace, usingsEmitted, frameworkSet, classModels);

            compilation = AddTargetNamespaceToCompilation(originalTargetNamespace, compilation, targetNamespace);

            var generationResult = CreateGenerationResult(compilation, classModels);

            return generationResult;
        }

        private static TypeDeclarationSyntax GetOrCreateTargetType(SyntaxNode sourceSymbol, SyntaxNode targetNamespace, IFrameworkSet frameworkSet, ClassModel classModel, out TypeDeclarationSyntax originalTargetType)
        {
            TypeDeclarationSyntax targetType = null;
            originalTargetType = null;

            if (targetNamespace != null && sourceSymbol != null)
            {
                var types = TestableItemExtractor.GetTypeDeclarations(targetNamespace);

                var targetClassName = frameworkSet.GetTargetTypeName(classModel, true);
                originalTargetType = targetType = types.FirstOrDefault(x => string.Equals(x.GetClassName(), targetClassName, StringComparison.OrdinalIgnoreCase));

                if (originalTargetType == null)
                {
                    targetClassName = frameworkSet.GetTargetTypeName(classModel, false);
                    originalTargetType = targetType = types.FirstOrDefault(x => string.Equals(x.GetClassName(), targetClassName, StringComparison.OrdinalIgnoreCase));
                }
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
    }
}