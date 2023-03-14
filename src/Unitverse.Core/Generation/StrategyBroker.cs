﻿namespace Unitverse.Core.Generation
{
    using System;
    using System.Collections.Generic;
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

    internal static class StrategyBroker
    {
        public static TypeDeclarationSyntax ApplyStrategies(TypeDeclarationSyntax targetType, ModelGenerationContext generationContext)
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

            //// TODO - scan for templates, if there are any then construct the template model and evaluate
            //// TODO - add option to see if templating is enabled, if not - return
            ////if (generationContext.FrameworkSet.Options.GenerationOptions.UseTemplating)
            ////{
            ////    var templates = TemplateStore.LoadTemplatesFor(sourceFolder);
            ////    if (templates.Any())
            ////    { }
            ////}

            return targetType;
        }

        private static TypeDeclarationSyntax AddGeneratedItems<T>(ModelGenerationContext generationContext, TypeDeclarationSyntax declaration, ItemGenerationStrategyFactory<T> factory, Func<ClassModel, IEnumerable<T>> selector, Func<T, bool> shouldGenerate, Func<NamingContext, T, NamingContext> nameDecorator)
        {
            foreach (var member in selector(generationContext.Model))
            {
                if (shouldGenerate(member))
                {
                    var namingContext = nameDecorator(generationContext.BaseNamingContext, member);
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
