namespace Unitverse.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using Unitverse.Core.Strategies;
    using Unitverse.Core.Strategies.ClassDecoration;
    using Unitverse.Core.Strategies.ClassGeneration;
    using Unitverse.Core.Strategies.ClassLevelGeneration;
    using Unitverse.Core.Strategies.IndexerGeneration;
    using Unitverse.Core.Strategies.InterfaceGeneration;
    using Unitverse.Core.Strategies.MethodGeneration;
    using Unitverse.Core.Strategies.OperatorGeneration;
    using Unitverse.Core.Strategies.PropertyGeneration;
    using Unitverse.Core.Strategies.ValueGeneration;

    public static class CoreGenerator
    {
        public static async Task<GenerationResult> Generate(SemanticModel sourceModel, SyntaxNode sourceSymbol, SemanticModel targetModel, bool withRegeneration, IUnitTestGeneratorOptions options, Func<string, string> nameSpaceTransform, bool isSingleItemGeneration, IMessageLogger messageLogger)
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

            return Generate(sourceModel, sourceSymbol, withRegeneration, options, targetNamespace, usingsEmitted, compilation, originalTargetNamespace, isSingleItemGeneration, messageLogger);
        }

        private static void MarkEmittedItems<T>(ModelGenerationContext generationContext, ItemGenerationStrategyFactory<T> factory, Func<ClassModel, IEnumerable<T>> selector)
            where T : ITestableModel
        {
            var namingContext = new NamingContext(generationContext.Model.ClassName);

            foreach (var member in selector(generationContext.Model))
            {
                member.MarkedForGeneration =
                    member.ShouldGenerate &&
                    factory.CreateFor(member, generationContext.Model, namingContext, generationContext.FrameworkSet.Options.StrategyOptions).Any();
            }
        }

        private static TypeDeclarationSyntax AddGeneratedItems<T>(ModelGenerationContext generationContext, TypeDeclarationSyntax declaration, ItemGenerationStrategyFactory<T> factory, Func<ClassModel, IEnumerable<T>> selector, Func<T, bool> shouldGenerate, Func<NamingContext, T, NamingContext> nameDecorator)
        {
            var namingContext = new NamingContext(generationContext.Model.ClassName);

            foreach (var member in selector(generationContext.Model))
            {
                if (shouldGenerate(member))
                {
                    namingContext = nameDecorator(namingContext, member);
                    foreach (var method in factory.CreateFor(member, generationContext.Model, namingContext, generationContext.FrameworkSet.Options.StrategyOptions))
                    {
                        var methodName = method.Identifier.Text;
                        var existingMethod = declaration.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault(x => string.Equals(x.Identifier.Text, methodName, StringComparison.OrdinalIgnoreCase));

                        if (existingMethod != null)
                        {
                            if (!generationContext.WithRegeneration && !generationContext.PartialGenerationAllowed)
                            {
                                throw new InvalidOperationException("One or more of the generated methods ('" + methodName + "') already exists in the test class. In order to over-write existing tests, hold left shift while right-clicking and select the 'Regenerate tests' option. Alternatively, you can enable the 'Partial Generation' option in the Visual Studio Options dialogue, which will only add new tests to existing test classes.");
                            }
                            else if (generationContext.WithRegeneration)
                            {
                                generationContext.FrameworkSet.Context.TestMethodsRegenerated++;
                                declaration = declaration.ReplaceNode(existingMethod, method);
                                generationContext.MethodsEmitted++;
                            }
                        }
                        else
                        {
                            generationContext.FrameworkSet.Context.TestMethodsGenerated++;

                            declaration = declaration.AddMembers(method);
                            generationContext.MethodsEmitted++;
                        }
                    }
                }
            }

            return declaration;
        }

        private static CompilationUnitSyntax AddTargetNamespaceToCompilation(NamespaceDeclarationSyntax originalTargetNamespace, CompilationUnitSyntax compilation, NamespaceDeclarationSyntax targetNamespace, IGenerationOptions generationOptions)
        {
            if (originalTargetNamespace == null)
            {
                if (generationOptions.EmitUsingsOutsideNamespace)
                {
                    var usings = targetNamespace.Usings.ToList();
                    MoveUsingsToCompilation(ref compilation, ref targetNamespace, usings);
                }
            }
            else
            {
                compilation = compilation.RemoveNode(originalTargetNamespace, SyntaxRemoveOptions.KeepNoTrivia);

                if (generationOptions.EmitUsingsOutsideNamespace)
                {
                    var namespaceUsings = originalTargetNamespace.Usings.Select(x => x.NormalizeWhitespace().ToFullString());
                    var compilationUsings = compilation.Usings.Select(x => x.NormalizeWhitespace().ToFullString());
                    var existingUsings = new HashSet<string>(namespaceUsings.Concat(compilationUsings), StringComparer.Ordinal);

                    // find all the usings that are in targetNamespace, but not declared in either the originalTargetNamespace or the compilation
                    var usings = targetNamespace.Usings.Where(x => !existingUsings.Contains(x.NormalizeWhitespace().ToFullString())).ToList();

                    MoveUsingsToCompilation(ref compilation, ref targetNamespace, compilation.Usings.Concat(usings).ToList());
                }
            }

            return compilation.AddMembers(targetNamespace);
        }

        private static void MoveUsingsToCompilation(ref CompilationUnitSyntax compilation, ref NamespaceDeclarationSyntax targetNamespace, IEnumerable<UsingDirectiveSyntax> usings)
        {
            foreach (var usingStatement in usings)
            {
                var fullString = usingStatement.NormalizeWhitespace().ToFullString();
                var node = targetNamespace.Usings.FirstOrDefault(x => x.NormalizeWhitespace().ToFullString() == fullString);
                if (node != null)
                {
                    targetNamespace = targetNamespace.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia);
                }
            }

            compilation = compilation.WithUsings(SyntaxFactory.List(usings));
        }

        private static NamespaceDeclarationSyntax AddTypeParameterAliases(ClassModel classModel, IGenerationContext context, NamespaceDeclarationSyntax targetNamespace)
        {
            foreach (var parameter in classModel.Declaration.TypeParameterList?.Parameters ?? Enumerable.Empty<TypeParameterSyntax>())
            {
                NameSyntax nameSyntax = SyntaxFactory.QualifiedName(SyntaxFactory.IdentifierName("System"), SyntaxFactory.IdentifierName("String"));
                ITypeSymbol derivedType = null;
                var constraint = classModel.Declaration.ConstraintClauses.FirstOrDefault(x => x.Name.Identifier.ValueText == parameter.Identifier.ValueText);

                if (constraint != null)
                {
                    var typeConstraints = constraint.Constraints.OfType<TypeConstraintSyntax>().Select(x => x.Type).Select(x => classModel.SemanticModel.GetTypeInfo(x));
                    var constrainableTypes = typeConstraints.Select(x => x.Type).Where(x => x != null && !(x is IErrorTypeSymbol)).ToArray();
                    if (constrainableTypes.Any())
                    {
                        derivedType = TypeHelper.FindDerivedNonAbstractType(constrainableTypes);
                        if (derivedType != null)
                        {
                            nameSyntax = SyntaxFactory.IdentifierName(derivedType.ToFullName());
                        }
                    }
                }

                var aliasedName = parameter.Identifier.ToString();
                if (targetNamespace.DescendantNodes().OfType<UsingDirectiveSyntax>().Any(node => string.Equals(node.Alias?.Name?.ToString(), aliasedName, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                if (!context.GenericTypes.ContainsKey(parameter.Identifier.ValueText))
                {
                    context.GenericTypes[parameter.Identifier.ValueText] = derivedType;
                    targetNamespace = targetNamespace.AddUsings(
                        SyntaxFactory.UsingDirective(nameSyntax)
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
            targetNamespace = EmitUsingStatements(targetNamespace, usingsEmitted, SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")));
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

        private static void MarkEmittedItems(ModelGenerationContext context)
        {
            MarkEmittedItems(context, new MethodGenerationStrategyFactory(context.FrameworkSet), x => x.Methods);
            MarkEmittedItems(context, new OperatorGenerationStrategyFactory(context.FrameworkSet), x => x.Operators);
            MarkEmittedItems(context, new PropertyGenerationStrategyFactory(context.FrameworkSet), x => x.Properties);
            MarkEmittedItems(context, new IndexerGenerationStrategyFactory(context.FrameworkSet), x => x.Indexers);
            ValueGenerationStrategyFactory.ResetSeed();
        }

        private static TypeDeclarationSyntax ApplyStrategies(TypeDeclarationSyntax targetType, ModelGenerationContext generationContext)
        {
            targetType = new ClassDecorationStrategyFactory(generationContext.FrameworkSet).Apply(targetType, generationContext.Model);

            targetType = AddGeneratedItems(generationContext, targetType, new ClassLevelGenerationStrategyFactory(generationContext.FrameworkSet), x => new[] { x }, x => x.Constructors.Any(c => c.ShouldGenerate) || (!x.Constructors.Any() && x.Properties.Any(p => p.HasInit)), (c, x) => c);

            if (generationContext.Model.Interfaces.Count > 0)
            {
                targetType = AddGeneratedItems(generationContext, targetType, new InterfaceGenerationStrategyFactory(generationContext.FrameworkSet), x => new[] { x }, x => x.ShouldGenerate, (c, x) => c);
            }

            targetType = AddGeneratedItems(generationContext, targetType, new MethodGenerationStrategyFactory(generationContext.FrameworkSet), x => x.Methods, x => x.ShouldGenerate, (c, x) => c.WithMemberName(generationContext.Model.GetMethodUniqueName(x), x.Name));
            targetType = AddGeneratedItems(generationContext, targetType, new OperatorGenerationStrategyFactory(generationContext.FrameworkSet), x => x.Operators, x => x.ShouldGenerate, (c, x) => c.WithMemberName(generationContext.Model.GetOperatorUniqueName(x), x.Name));
            targetType = AddGeneratedItems(generationContext, targetType, new PropertyGenerationStrategyFactory(generationContext.FrameworkSet), x => x.Properties, x => x.ShouldGenerate, (c, x) => c.WithMemberName(x.Name));
            targetType = AddGeneratedItems(generationContext, targetType, new IndexerGenerationStrategyFactory(generationContext.FrameworkSet), x => x.Indexers, x => x.ShouldGenerate, (c, x) => c.WithMemberName(generationContext.Model.GetIndexerName(x)));
            return targetType;
        }

        private static GenerationResult CreateGenerationResult(CompilationUnitSyntax compilation, List<ClassModel> classModels, bool anyMethodsEmitted, IGenerationStatistics generationStatistics)
        {
            using (var workspace = new AdhocWorkspace())
            {
                compilation = (CompilationUnitSyntax)Formatter.Format(compilation, workspace);

                var generationResult = new GenerationResult(compilation.ToFullString(), anyMethodsEmitted, generationStatistics);
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
            var setupMethod = frameworkSet.TestFramework.CreateSetupMethod(frameworkSet.GetTargetTypeName(classModel));

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
                    allFields.Add(classModel.GetConstructorParameterFieldName(parameterModel, frameworkSet.NamingProvider));
                }

                // generate fields for each constructor parameter that doesn't have an existing field
                foreach (var parameterModel in classModel.Constructors.SelectMany(x => x.Parameters))
                {
                    if (!parametersEmitted.Add(parameterModel.Name))
                    {
                        continue;
                    }

                    var fieldName = classModel.GetConstructorParameterFieldName(parameterModel, frameworkSet.NamingProvider);

                    var fieldExists = targetType.Members.OfType<FieldDeclarationSyntax>().Any(x => x.Declaration.Variables.Any(v => v.Identifier.Text == fieldName));

                    if (!fieldExists)
                    {
                        var fieldTypeSyntax = parameterModel.TypeInfo.ToTypeSyntax(frameworkSet.Context);
                        ExpressionSyntax defaultExpression;

                        if (parameterModel.TypeInfo.Type.TypeKind == TypeKind.Interface)
                        {
                            frameworkSet.Context.InterfacesMocked++;
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

        private static GenerationResult Generate(SemanticModel sourceModel, SyntaxNode sourceSymbol, bool withRegeneration, IUnitTestGeneratorOptions options, NamespaceDeclarationSyntax targetNamespace, HashSet<string> usingsEmitted, CompilationUnitSyntax compilation, NamespaceDeclarationSyntax originalTargetNamespace, bool isSingleItemGeneration, IMessageLogger messageLogger)
        {
            var frameworkSet = FrameworkSetFactory.Create(options);

            var model = new TestableItemExtractor(sourceModel.SyntaxTree, sourceModel);
            var classModels = model.Extract(sourceSymbol, options).ToList();

            foreach (var c in classModels)
            {
                frameworkSet.Context.TestClassesGenerated++;

                frameworkSet.EvaluateTargetModel(c);

                c.SetTargetInstance(frameworkSet.NamingProvider.TargetFieldName.Resolve(new NamingContext(c.ClassName)), frameworkSet);
                if (c.Declaration.Parent is NamespaceDeclarationSyntax namespaceDeclaration)
                {
                    targetNamespace = EmitUsingStatements(targetNamespace, usingsEmitted, SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(namespaceDeclaration.Name.ToString())));
                }
            }

            bool anyMethodsEmitted = false;

            foreach (var classModel in classModels)
            {
                targetNamespace = AddTypeParameterAliases(classModel, frameworkSet.Context, targetNamespace);

                var context = new ModelGenerationContext(classModel, frameworkSet, withRegeneration, options.GenerationOptions.PartialGenerationAllowed);
                MarkEmittedItems(context);

                var targetType = GetOrCreateTargetType(targetNamespace, frameworkSet, classModel, out var originalTargetType);

                targetType = ApplyStrategies(targetType, context);

                anyMethodsEmitted |= context.MethodsEmitted > 0;
                if (context.MethodsEmitted == 0 && options.GenerationOptions.PartialGenerationAllowed)
                {
                    if (isSingleItemGeneration)
                    {
                        throw new InvalidOperationException("No new methods were added to the test class for the type '" + classModel.ClassName + "'. If you want to re-generate existing tests, please hold left shift while opening the context menu and select the 'Regenerate tests' option.");
                    }
                    else
                    {
                        messageLogger.LogMessage("No new methods were added to the test class for the type '" + classModel.ClassName + "'.");
                    }
                }

                targetNamespace = AddTypeToTargetNamespace(originalTargetType, targetNamespace, targetType);

                foreach (var parameter in frameworkSet.Context.GenericTypesVisited)
                {
                    targetNamespace = targetNamespace.AddUsings(
                        SyntaxFactory.UsingDirective(SyntaxFactory.QualifiedName(SyntaxFactory.IdentifierName("System"), SyntaxFactory.IdentifierName("String")))
                            .WithAlias(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(parameter))));
                }
            }

            targetNamespace = AddUsingStatements(targetNamespace, usingsEmitted, frameworkSet, classModels);

            compilation = AddTargetNamespaceToCompilation(originalTargetNamespace, compilation, targetNamespace, options.GenerationOptions);

            var generationResult = CreateGenerationResult(compilation, classModels, anyMethodsEmitted, frameworkSet.Context);

            return generationResult;
        }

        private static TypeDeclarationSyntax GetOrCreateTargetType(SyntaxNode targetNamespace, IFrameworkSet frameworkSet, ClassModel classModel, out TypeDeclarationSyntax originalTargetType)
        {
            TypeDeclarationSyntax targetType = null;
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
    }
}