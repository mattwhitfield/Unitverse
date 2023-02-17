namespace Unitverse.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Models;
    using Unitverse.Core.Options;

    public class TestableItemExtractor
    {
        private readonly SemanticModel _semanticModel;

        public TestableItemExtractor(SyntaxTree tree, SemanticModel semanticModel)
        {
            Tree = tree ?? throw new ArgumentNullException(nameof(tree));
            _semanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));
        }

        private SyntaxTree Tree { get; }

        public static IList<TypeDeclarationSyntax> GetTypeDeclarations(SyntaxNode root)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            var typeList = new List<TypeDeclarationSyntax>();
            typeList.AddRange(root.DescendantNodes().OfType<ClassDeclarationSyntax>().Where(x => !x.Ancestors().OfType<TypeDeclarationSyntax>().Any()));
            typeList.AddRange(root.DescendantNodes().OfType<StructDeclarationSyntax>().Where(x => !x.Ancestors().OfType<TypeDeclarationSyntax>().Any()));
            typeList.AddRange(root.DescendantNodes().OfType<RecordDeclarationSyntax>().Where(x => !x.Ancestors().OfType<TypeDeclarationSyntax>().Any()));
            return typeList;
        }

        public IEnumerable<ClassModel> Extract(SyntaxNode? sourceSymbol, IUnitTestGeneratorOptions options)
        {
            var models = ExtractClassModels(Tree, _semanticModel, options).ToList();
            if (sourceSymbol != null)
            {
                models.Each(x => x.SetShouldGenerateForSingleItem(sourceSymbol));
            }

            // only emit classes that are directly selected for generation, or have a member that is selected
            return models.Where(x => x.ShouldGenerateOrContainsItemThatShouldGenerate());
        }

        private static IList<Func<SyntaxTokenList, bool>> GetAllowedModifiers(IUnitTestGeneratorOptions options, TypeDeclarationSyntax typeDeclarationSyntax)
        {
            var functionList = new List<Func<SyntaxTokenList, bool>>();

            // allow public
            functionList.Add(list => list.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword)));

            var canEmitForProtectedMethods = options.GenerationOptions.EmitSubclassForProtectedMethods && !typeDeclarationSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.SealedKeyword));

            // if we are allowing internals
            if (options.GenerationOptions.EmitTestsForInternals)
            {
                if (canEmitForProtectedMethods)
                {
                    // then we are good for protected, internal & protected internal
                    functionList.Add(list => list.Any(modifier => modifier.IsKind(SyntaxKind.ProtectedKeyword)));
                    functionList.Add(list => list.Any(modifier => modifier.IsKind(SyntaxKind.InternalKeyword)));
                }
                else
                {
                    // otherwise we are just good for internal
                    functionList.Add(list => list.Any(modifier => modifier.IsKind(SyntaxKind.InternalKeyword)) && !list.Any(modifier => modifier.IsKind(SyntaxKind.ProtectedKeyword)));
                }
            }
            else if (canEmitForProtectedMethods)
            {
                // not allowing internals - so just protected and not protected internal
                functionList.Add(list => list.Any(modifier => modifier.IsKind(SyntaxKind.ProtectedKeyword)) && !list.Any(modifier => modifier.IsKind(SyntaxKind.InternalKeyword)));
            }

            return functionList;
        }

        private void AddModels<TIn, TOut>(TypeDeclarationSyntax type, SemanticModel semanticModel, Func<TIn, SyntaxTokenList> modifiersSelector, Func<TIn, SemanticModel, TOut> converter, IList<Func<SyntaxTokenList, bool>> allowedModifiers, ICollection<TOut> target)
        {
            foreach (var model in type.ChildNodes().OfType<TIn>().Where(x => allowedModifiers.Any(modifierFilter => modifierFilter(modifiersSelector(x)))).Select(x => converter(x, semanticModel)))
            {
                target.Add(model);
            }
        }

        private ClassModel ExtractClassModel(TypeDeclarationSyntax syntax, SemanticModel semanticModel, IUnitTestGeneratorOptions options)
        {
            var allowedModifiers = GetAllowedModifiers(options, syntax);

            var model = new ClassModel(syntax, semanticModel, false);

            CollectRelatedPartialTypeConstructors(syntax, semanticModel, allowedModifiers, model);

            AddModels<ConstructorDeclarationSyntax, IConstructorModel>(syntax, semanticModel, x => x.Modifiers, ExtractConstructorModel, allowedModifiers, model.Constructors);
            AddModels<OperatorDeclarationSyntax, IOperatorModel>(syntax, semanticModel, x => x.Modifiers, ExtractOperatorModel, allowedModifiers, model.Operators);
            AddModels<MethodDeclarationSyntax, IMethodModel>(syntax, semanticModel, x => x.Modifiers, ExtractMethodModel, allowedModifiers, model.Methods);
            AddModels<PropertyDeclarationSyntax, IPropertyModel>(syntax, semanticModel, x => x.Modifiers, ExtractPropertyModel, allowedModifiers, model.Properties);
            AddModels<IndexerDeclarationSyntax, IIndexerModel>(syntax, semanticModel, x => x.Modifiers, ExtractIndexerModel, allowedModifiers, model.Indexers);

            if (syntax is RecordDeclarationSyntax record)
            {
                if (record.ParameterList != null)
                {
                    var dummyContext = new GenerationContext(options.GenerationOptions, new NamingProvider(options.NamingOptions));
                    foreach (var recordContstructor in model.TypeSymbol.Constructors.Where(x => x.IsImplicitlyDeclared && !model.Constructors.Any(c => c == x)))
                    {
                        var constructor = SyntaxFactory.ConstructorDeclaration(model.ClassName);
                        List<ParameterModel> parameters;

                        if (record.ParameterList != null)
                        {
                            constructor = constructor.WithParameterList(record.ParameterList);
                            parameters = ExtractParameters(record.ParameterList.Parameters, semanticModel);
                        }
                        else
                        {
                            continue;
                        }

                        model.Constructors.Add(new ConstructorModel(model.ClassName, parameters, constructor));

                        var allProperties = model.TypeSymbol.GetMembers().OfType<IPropertySymbol>();

                        foreach (var parameter in parameters)
                        {
                            if (model.Properties.Any(x => x.Name == parameter.Name))
                            {
                                continue;
                            }

                            var propertyName = parameter.Name;

                            var property = SyntaxFactory.PropertyDeclaration(parameter.TypeInfo.ToTypeSyntax(dummyContext), SyntaxFactory.Identifier(propertyName))
                                                        .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                                                        .WithAccessorList(SyntaxFactory.AccessorList(SyntaxFactory.SingletonList(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration))));

                            model.Properties.Add(new PropertyModel(propertyName, property, parameter.TypeInfo, semanticModel, allProperties.FirstOrDefault(x => x.Name == propertyName)));
                        }
                    }
                }
            }

            foreach (var methodModel in syntax.ChildNodes().OfType<MethodDeclarationSyntax>().Where(x => x.ExplicitInterfaceSpecifier != null).Select(x => ExtractMethodModel(x, semanticModel)))
            {
                model.Methods.Add(methodModel);
            }

            model.DefaultConstructor = model.Constructors.OrderByDescending(x => x.Parameters.Count).FirstOrDefault();

            // populate interface models
            foreach (var declaredSymbolInterface in model.TypeSymbol.Interfaces)
            {
                model.Interfaces.Add(new InterfaceModel(declaredSymbolInterface));
            }

            return model;
        }

        private void CollectRelatedPartialTypeConstructors(TypeDeclarationSyntax syntax, SemanticModel semanticModel, IList<Func<SyntaxTokenList, bool>> allowedModifiers, ClassModel model)
        {
            if (!syntax.Modifiers.Any(x => x.Kind() == SyntaxKind.PartialKeyword))
            {
                return;
            }

            if (model.TypeSymbol.Locations.Length <= 1)
            {
                return;
            }

            var currentLocation = syntax.GetLocation();
            foreach (var location in model.TypeSymbol.Locations.Where(x => x.SourceTree != currentLocation.SourceTree))
            {
                var node = location.SourceTree?.GetRoot();

                if (node == null)
                {
                    continue;
                }

                var relatedModel = semanticModel.Compilation.GetSemanticModel(node.SyntaxTree);

                if (relatedModel == null)
                {
                    continue;
                }

                // this is a related partial
                foreach (var child in node.DescendantNodesAndSelf().OfType<TypeDeclarationSyntax>())
                {
                    ConstructorModel ExtractRelatedPartialConstructorModel(ConstructorDeclarationSyntax constructor, SemanticModel semanticModel)
                    {
                        var constructorModel = ExtractConstructorModel(constructor, semanticModel);
                        constructorModel.IsFromRelatedPartial = true;
                        constructorModel.ShouldGenerate = false;

                        return constructorModel;
                    }

                    AddModels<ConstructorDeclarationSyntax, IConstructorModel>(child, relatedModel, x => x.Modifiers, ExtractRelatedPartialConstructorModel, allowedModifiers, model.Constructors);
                }
            }
        }

        private IEnumerable<ClassModel> ExtractClassModels(SyntaxTree tree, SemanticModel semanticModel, IUnitTestGeneratorOptions options)
        {
            var root = tree.GetRoot();

            var typeList = GetTypeDeclarations(root);
            var fileUsings = root.DescendantNodesAndSelf().OfType<UsingDirectiveSyntax>().ToList();

            foreach (var syntax in typeList)
            {
                var model = ExtractClassModel(syntax, semanticModel, options);

                fileUsings.ForEach(model.Usings.Add);

                yield return model;
            }
        }

        private ConstructorModel ExtractConstructorModel(ConstructorDeclarationSyntax constructor, SemanticModel semanticModel)
        {
            var name = constructor.Identifier.ValueText;

            var parameters = ExtractParameters(constructor.ParameterList.Parameters, semanticModel);

            return new ConstructorModel(name, parameters, constructor);
        }

        private IndexerModel ExtractIndexerModel(IndexerDeclarationSyntax indexer, SemanticModel semanticModel)
        {
            var parameters = ExtractParameters(indexer.ParameterList.Parameters, semanticModel);

            var typeInfo = semanticModel.GetTypeInfo(indexer.Type);

            return new IndexerModel("this", parameters, typeInfo, indexer);
        }

        private MethodModel ExtractMethodModel(MethodDeclarationSyntax method, SemanticModel semanticModel)
        {
            var methodName = method.Identifier.ValueText;

            var parameters = ExtractParameters(method.ParameterList.Parameters, semanticModel);

            return new MethodModel(methodName, parameters, method, semanticModel);
        }

        private OperatorModel ExtractOperatorModel(OperatorDeclarationSyntax operatorSyntax, SemanticModel semanticModel)
        {
            var methodName = operatorSyntax.OperatorToken.ValueText;

            var parameters = ExtractParameters(operatorSyntax.ParameterList.Parameters, semanticModel);

            return new OperatorModel(methodName, parameters, operatorSyntax, semanticModel);
        }

        private List<ParameterModel> ExtractParameters(SeparatedSyntaxList<ParameterSyntax> parameterList, SemanticModel semanticModel)
        {
            var parameters = new List<ParameterModel>();

            foreach (var parameter in parameterList)
            {
                var typeModel = semanticModel.GetDeclaredSymbol(parameter);

                if (typeModel != null && parameter.Type != null)
                {
                    var typeInfo = semanticModel.GetTypeInfo(parameter.Type);

                    parameters.Add(new ParameterModel(typeModel.Name, parameter, typeModel.ToDisplayString(), typeInfo));
                }
            }

            return parameters;
        }

        private PropertyModel ExtractPropertyModel(PropertyDeclarationSyntax property, SemanticModel semanticModel)
        {
            var propertyName = property.Identifier.ValueText;

            var typeInfo = semanticModel.GetTypeInfo(property.Type);

            return new PropertyModel(propertyName, property, typeInfo, semanticModel, null);
        }
    }
}