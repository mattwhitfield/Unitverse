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
    using Microsoft.CodeAnalysis.Options;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Generation;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public static class CoreGenerator
    {
        public static async Task<GenerationResult> Generate(SemanticModel sourceModel, SyntaxNode? sourceSymbol, SemanticModel? targetModel, Solution? solution, bool withRegeneration, IUnitTestGeneratorOptions options, Func<string, string> nameSpaceTransform, bool isSingleItemGeneration, IMessageLogger messageLogger)
        {
            if (nameSpaceTransform == null)
            {
                throw new ArgumentNullException(nameof(nameSpaceTransform));
            }

            var sourceNameSpace = await sourceModel.GetNamespace().ConfigureAwait(true);
            var targetNameSpace = nameSpaceTransform(sourceNameSpace ?? "Namespace");

            var usingsEmitted = new HashSet<string>();

            NamespaceDeclarationSyntax? originalTargetNamespace = null;

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

            DocumentOptionSet? documentOptions = null;
            if (solution != null)
            {
                var document = solution.GetDocument(sourceModel.SyntaxTree);
                if (document != null)
                {
                    documentOptions = await document.GetOptionsAsync();
                }
            }

            return Generate(sourceModel, sourceSymbol, documentOptions, withRegeneration, options, targetNamespace, usingsEmitted, compilation, originalTargetNamespace, isSingleItemGeneration, messageLogger);
        }

        private static CompilationUnitSyntax AddTargetNamespaceToCompilation(NamespaceDeclarationSyntax? originalTargetNamespace, CompilationUnitSyntax compilation, NamespaceDeclarationSyntax targetNamespace, IGenerationOptions generationOptions)
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
                var newCompilation = compilation.RemoveNode(originalTargetNamespace, SyntaxRemoveOptions.KeepNoTrivia);
                compilation = newCompilation ?? compilation;

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
                    var newNamespace = targetNamespace.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia);
                    if (newNamespace != null)
                    {
                        targetNamespace = newNamespace;
                    }
                }
            }

            compilation = compilation.WithUsings(SyntaxFactory.List(usings));
        }

        private static NamespaceDeclarationSyntax AddTypeParameterAliases(ClassModel classModel, IGenerationContext context, NamespaceDeclarationSyntax targetNamespace)
        {
            foreach (var parameter in classModel.Declaration.TypeParameterList?.Parameters ?? Enumerable.Empty<TypeParameterSyntax>())
            {
                NameSyntax nameSyntax = SyntaxFactory.QualifiedName(SyntaxFactory.IdentifierName("System"), SyntaxFactory.IdentifierName("String"));
                ITypeSymbol? derivedType = null;
                var constraint = classModel.Declaration.ConstraintClauses.FirstOrDefault(x => x.Name.Identifier.ValueText == parameter.Identifier.ValueText);

                if (constraint != null)
                {
                    var typeConstraints = constraint.Constraints.OfType<TypeConstraintSyntax>().Select(x => x.Type).Select(x => classModel.SemanticModel.GetTypeInfo(x)) ?? Enumerable.Empty<TypeInfo>();
                    ITypeSymbol[] constrainableTypes = typeConstraints.Select(x => x.Type).WhereNotNull().Where(x => !(x is IErrorTypeSymbol)).ToArray();
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

        private static NamespaceDeclarationSyntax AddTypeToTargetNamespace(TypeDeclarationSyntax? originalTargetType, NamespaceDeclarationSyntax targetNamespace, TypeDeclarationSyntax targetType)
        {
            if (originalTargetType != null)
            {
                var newTargetNamespace = targetNamespace.RemoveNode(originalTargetType, SyntaxRemoveOptions.KeepNoTrivia);
                targetNamespace = newTargetNamespace ?? targetNamespace;
            }
            else
            {
                var type = targetNamespace.DescendantNodes().OfType<TypeDeclarationSyntax>().FirstOrDefault(x => x.Identifier.ValueText == targetType.Identifier.ValueText);
                if (type != null)
                {
                    var newTargetNamespace = targetNamespace.RemoveNode(type, SyntaxRemoveOptions.KeepNoTrivia);
                    targetNamespace = newTargetNamespace ?? targetNamespace;
                }
            }

            targetNamespace = targetNamespace.AddMembers(targetType);
            return targetNamespace;
        }

        private static NamespaceDeclarationSyntax AddUsingStatements(NamespaceDeclarationSyntax targetNamespace, HashSet<string> usingsEmitted, IFrameworkSet frameworkSet, List<ClassModel> classModels)
        {
            targetNamespace = EmitUsingStatements(targetNamespace, usingsEmitted, Helpers.Generate.UsingDirective("System"));
            targetNamespace = EmitUsingStatements(targetNamespace, usingsEmitted, frameworkSet.TestFramework.GetUsings());
            targetNamespace = EmitUsingStatements(targetNamespace, usingsEmitted, frameworkSet.AssertionFramework.GetUsings());
            if (frameworkSet.Context.MocksUsed)
            {
                targetNamespace = EmitUsingStatements(targetNamespace, usingsEmitted, frameworkSet.MockingFramework.GetUsings());
            }

            if (frameworkSet.Options.GenerationOptions.UseAutoFixture)
            {
                targetNamespace = EmitUsingStatements(targetNamespace, usingsEmitted, Helpers.Generate.UsingDirective("AutoFixture"));

                if (frameworkSet.Options.GenerationOptions.CanUseAutoFixtureForMocking())
                {
                    switch (frameworkSet.Options.GenerationOptions.MockingFrameworkType)
                    {
                        case MockingFrameworkType.NSubstitute:
                            targetNamespace = EmitUsingStatements(targetNamespace, usingsEmitted, Helpers.Generate.UsingDirective("AutoFixture.AutoNSubstitute"));
                            break;
                        case MockingFrameworkType.MoqAutoMock:
                        case MockingFrameworkType.Moq:
                            targetNamespace = EmitUsingStatements(targetNamespace, usingsEmitted, Helpers.Generate.UsingDirective("AutoFixture.AutoMoq"));
                            break;
                        case MockingFrameworkType.FakeItEasy:
                            targetNamespace = EmitUsingStatements(targetNamespace, usingsEmitted, Helpers.Generate.UsingDirective("AutoFixture.AutoFakeItEasy"));
                            break;
                    }
                }
            }

            foreach (var emittedType in frameworkSet.Context.EmittedTypes)
            {
                if (emittedType?.ContainingNamespace != null)
                {
                    targetNamespace = EmitUsingStatements(targetNamespace, usingsEmitted, Helpers.Generate.UsingDirective(emittedType.ContainingNamespace.ToDisplayString()));
                }
            }

            if (classModels.SelectMany(x => x.Methods).Any(x => x.IsAsync))
            {
                targetNamespace = EmitUsingStatements(targetNamespace, usingsEmitted, Helpers.Generate.UsingDirective(typeof(Task).Namespace));
            }

            if (!string.IsNullOrWhiteSpace(frameworkSet.Options.GenerationOptions.TestTypeBaseClass) &&
                !string.IsNullOrWhiteSpace(frameworkSet.Options.GenerationOptions.TestTypeBaseClassNamespace))
            {
                targetNamespace = EmitUsingStatements(targetNamespace, usingsEmitted, Helpers.Generate.UsingDirective(frameworkSet.Options.GenerationOptions.TestTypeBaseClassNamespace));
            }

            return targetNamespace;
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

        private static GenerationResult Generate(SemanticModel sourceModel, SyntaxNode? sourceSymbol, DocumentOptionSet? documentOptions, bool withRegeneration, IUnitTestGeneratorOptions options, NamespaceDeclarationSyntax targetNamespace, HashSet<string> usingsEmitted, CompilationUnitSyntax compilation, NamespaceDeclarationSyntax? originalTargetNamespace, bool isSingleItemGeneration, IMessageLogger messageLogger)
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
                    targetNamespace = EmitUsingStatements(targetNamespace, usingsEmitted, Helpers.Generate.UsingDirective(namespaceDeclaration.Name.ToString()));
                }
            }

            bool anyMethodsEmitted = false;

            foreach (var classModel in classModels)
            {
                targetNamespace = AddTypeParameterAliases(classModel, frameworkSet.Context, targetNamespace);

                var context = new ModelGenerationContext(classModel, frameworkSet, withRegeneration, options.GenerationOptions.PartialGenerationAllowed, new NamingContext(classModel.ClassName));

                EmissionMarker.MarkEmittedItems(context);

                var targetType = TypeDeclarationFactory.GetOrCreateTargetType(targetNamespace, frameworkSet, classModel, out var originalTargetType);

                targetType = StrategyBroker.ApplyStrategies(targetType, context);

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
                    targetNamespace = targetNamespace.AddUsings(Helpers.Generate.UsingDirective("System.String").WithAlias(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(parameter))));
                }
            }

            targetNamespace = AddUsingStatements(targetNamespace, usingsEmitted, frameworkSet, classModels);

            compilation = AddTargetNamespaceToCompilation(originalTargetNamespace, compilation, targetNamespace, options.GenerationOptions);

            var generationResult = GenerationResultFactory.CreateGenerationResult(compilation, documentOptions, classModels, anyMethodsEmitted, frameworkSet.Context);

            return generationResult;
        }
    }
}