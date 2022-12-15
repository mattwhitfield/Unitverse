namespace Unitverse.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Generation;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public static class CoreGenerator
    {
        public static async Task<GenerationResult> Generate(IGenerationItem generationItem, SemanticModel sourceModel, SemanticModel? targetModel, Solution? solution, bool withRegeneration, bool isSingleItemGeneration, IMessageLogger messageLogger)
        {
            // create the strategy used to build compilation units (file scoped or block scoped namespaces)
            var strategy = await CompilationUnitStrategyFactory.CreateAsync(sourceModel, targetModel, generationItem, solution).ConfigureAwait(true);

            // create the framework set that we'll use to generate for this item
            var frameworkSet = FrameworkSetFactory.Create(strategy.GenerationItem.Options);

            // extract the class models that we'll use for generation
            var classModels = GetClassModels(strategy);

            // prepare the models ready to be emitted
            PrepareModelsForEmission(strategy, frameworkSet, classModels);

            // generate the tests for each class model
            var anyMethodsEmitted = false;
            foreach (var classModel in classModels)
            {
                anyMethodsEmitted |= GenerateModel(classModel, strategy, frameworkSet, withRegeneration, isSingleItemGeneration, messageLogger);
            }

            // add the using statements we need for our chosen frameworks
            FrameworkDependencyHelper.AddUsingStatements(strategy, frameworkSet, classModels);

            // now render the compilation unit
            var compilation = strategy.RenderCompilationUnit();

            // and return the result
            return GenerationResultFactory.CreateGenerationResult(compilation, strategy.DocumentOptions, classModels, anyMethodsEmitted, frameworkSet.Context);
        }

        private static bool GenerateModel(ClassModel classModel, ICompilationUnitStrategy strategy, IFrameworkSet frameworkSet, bool withRegeneration, bool isSingleItemGeneration, IMessageLogger messageLogger)
        {
            // add aliases for any generic type parameters from the source type
            strategy.AddTypeParameterAliases(classModel, frameworkSet.Context);

            // create a context for generation
            var context = new ModelGenerationContext(classModel, frameworkSet, withRegeneration, new NamingContext(classModel.ClassName));

            // mark the items that we actually want to emit based on the applicable config
            EmissionMarker.MarkEmittedItems(context);

            // get the target type into which we want to apply strategies and apply those strategies
            var targetType = TypeDeclarationFactory.GetOrCreateTargetType(strategy.TargetRoot, frameworkSet, classModel, out var originalTargetType);
            targetType = StrategyBroker.ApplyStrategies(targetType, context);

            // check to see if we emitted any methods or should raise an error
            var anyMethodsEmitted = CheckForEmittedMethods(isSingleItemGeneration, strategy.GenerationItem.Options, classModel, context, messageLogger);

            // add the type that was generated to the target
            strategy.AddTypeToTarget(targetType, originalTargetType);

            // add using statements to alias any generic types that we visited in the course of generation
            foreach (var parameter in frameworkSet.Context.GenericTypesVisited)
            {
                strategy.AddUsing(Helpers.Generate.UsingDirective("System.String").WithAlias(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(parameter))));
            }

            return anyMethodsEmitted;
        }

        private static bool CheckForEmittedMethods(bool isSingleItemGeneration, IUnitTestGeneratorOptions options, ClassModel classModel, ModelGenerationContext context, IMessageLogger messageLogger)
        {
            var anyMethodsEmitted = context.MethodsEmitted > 0;
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

            return anyMethodsEmitted;
        }

        private static void PrepareModelsForEmission(ICompilationUnitStrategy strategy, IFrameworkSet frameworkSet, IEnumerable<ClassModel> classModels)
        {
            foreach (var classModel in classModels)
            {
                frameworkSet.Context.TestClassesGenerated++;

                frameworkSet.EvaluateTargetModel(classModel);

                classModel.SetTargetInstance(frameworkSet.NamingProvider.TargetFieldName.Resolve(new NamingContext(classModel.ClassName)), frameworkSet);

                if (classModel.Declaration.Parent is NamespaceDeclarationSyntax namespaceDeclaration)
                {
                    strategy.AddUsing(Helpers.Generate.UsingDirective(namespaceDeclaration.Name.ToString()));
                }
#if VS2022
                else
                {
                    var fileScopedNamespaceDeclaration = classModel.Declaration.AncestorsAndSelf().OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault();
                    if (fileScopedNamespaceDeclaration != null)
                    {
                        strategy.AddUsing(Helpers.Generate.UsingDirective(fileScopedNamespaceDeclaration.Name.ToString()));
                    }
                }
#endif
            }
        }

        private static List<ClassModel> GetClassModels(ICompilationUnitStrategy strategy)
        {
            var model = new TestableItemExtractor(strategy.SourceModel.SyntaxTree, strategy.SourceModel);
            return model.Extract(strategy.GenerationItem.SourceNode, strategy.GenerationItem.Options).ToList();
        }
    }
}