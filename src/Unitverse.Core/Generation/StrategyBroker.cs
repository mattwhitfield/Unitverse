namespace Unitverse.Core.Generation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;
    using Unitverse.Core.Strategies;
    using Unitverse.Core.Strategies.ClassDecoration;
    using Unitverse.Core.Strategies.ClassLevelGeneration;
    using Unitverse.Core.Strategies.IndexerGeneration;
    using Unitverse.Core.Strategies.InterfaceGeneration;
    using Unitverse.Core.Strategies.MethodGeneration;
    using Unitverse.Core.Strategies.OperatorGeneration;
    using Unitverse.Core.Strategies.PropertyGeneration;
    using Unitverse.Core.Templating;
    using Unitverse.Core.Templating.Model;
    using Unitverse.Core.Templating.Model.Implementation;

    internal static class StrategyBroker
    {
        public static TypeDeclarationSyntax ApplyStrategies(TypeDeclarationSyntax targetType, ModelGenerationContext generationContext)
        {
            targetType = new ClassDecorationStrategyFactory(generationContext.FrameworkSet).Apply(targetType, generationContext.Model);

            targetType = ApplyStandardStrategies(targetType, generationContext);

            targetType = ApplyTemplates(targetType, generationContext);

            return targetType;
        }

        private static TypeDeclarationSyntax ApplyTemplates(TypeDeclarationSyntax targetType, ModelGenerationContext generationContext)
        {
            var path = generationContext.FrameworkSet.Options.SourceProjectPath ?? generationContext.FrameworkSet.Options.SolutionPath;

            if (File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
            }

            if (Directory.Exists(path))
            {
                // scan for templates, if there are any then construct the template model and evaluate
                var templates = TemplateStore.LoadTemplatesFor(path, generationContext.MessageLogger);
                if (templates.Any())
                {
                    var context = new TemplatingContext(generationContext, templates);

                    targetType = AddGeneratedTemplates(context.ForConstructors(), targetType);
                    targetType = AddGeneratedTemplates(context.ForMethods(), targetType);
                    targetType = AddGeneratedTemplates(context.ForProperties(), targetType);
                }
            }

            return targetType;
        }

        private static TypeDeclarationSyntax ApplyStandardStrategies(TypeDeclarationSyntax targetType, ModelGenerationContext generationContext)
        {
            targetType = AddGeneratedItems(generationContext, targetType, new ClassLevelGenerationStrategyFactory(generationContext.FrameworkSet));

            if (generationContext.Model.Interfaces.Count > 0)
            {
                targetType = AddGeneratedItems(generationContext, targetType, new InterfaceGenerationStrategyFactory(generationContext.FrameworkSet));
            }

            targetType = AddGeneratedItems(generationContext, targetType, new MethodGenerationStrategyFactory(generationContext.FrameworkSet));
            targetType = AddGeneratedItems(generationContext, targetType, new OperatorGenerationStrategyFactory(generationContext.FrameworkSet));
            targetType = AddGeneratedItems(generationContext, targetType, new PropertyGenerationStrategyFactory(generationContext.FrameworkSet));
            targetType = AddGeneratedItems(generationContext, targetType, new IndexerGenerationStrategyFactory(generationContext.FrameworkSet));
            return targetType;
        }

        private static TypeDeclarationSyntax AddGeneratedTemplates(
            SpecificTemplatingContext templatingContext,
            TypeDeclarationSyntax declaration)
        {
            var anyMatched = false;
            foreach (var template in templatingContext.Templates.OrderBy(x => x.Priority))
            {
                if (anyMatched && template.IsExclusive)
                {
                    continue;
                }

                foreach (var model in templatingContext.Targets.Where(x => x.ShouldGenerate))
                {
                    if (template.AppliesTo(model, templatingContext.ClassModel))
                    {
                        var namingContext = model.CreateNamingContext(templatingContext);
                        var method = template.Create(templatingContext.ModelGenerationContext.FrameworkSet, model, templatingContext.ClassModel, namingContext);

                        declaration = EmitMethod(templatingContext.ModelGenerationContext, declaration, method);
                        if (template.StopMatching)
                        {
                            return declaration;
                        }

                        anyMatched = true;
                    }
                }
            }

            return declaration;
        }

        private static TypeDeclarationSyntax AddGeneratedItems<T>(
            ModelGenerationContext generationContext,
            TypeDeclarationSyntax declaration,
            ItemGenerationStrategyFactory<T> factory)
        {
            foreach (var member in factory.GetItems(generationContext.Model))
            {
                if (factory.ShouldGenerate(member))
                {
                    var namingContext = factory.DecorateNamingContext(generationContext.BaseNamingContext, generationContext.Model, member);
                    foreach (var methodHandler in factory.CreateFor(member, generationContext.Model, namingContext, generationContext.FrameworkSet.Options.StrategyOptions))
                    {
                        var method = methodHandler.Method;

                        declaration = EmitMethod(generationContext, declaration, method);
                    }
                }
            }

            return declaration;
        }

        private static TypeDeclarationSyntax EmitMethod(ModelGenerationContext generationContext, TypeDeclarationSyntax declaration, BaseMethodDeclarationSyntax method)
        {
            MethodDeclarationSyntax? existingMethod = null;
            string methodName = "ctor"; // will be replaced below if it's not a constructor

            if (method is MethodDeclarationSyntax typeMethod)
            {
                methodName = typeMethod.Identifier.Text;
                existingMethod = declaration.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault(x => string.Equals(x.Identifier.Text, methodName, StringComparison.OrdinalIgnoreCase));
            }

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

            return declaration;
        }
    }
}
