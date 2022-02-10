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
        public TestableItemExtractor(SyntaxTree tree, SemanticModel semanticModel)
        {
            Tree = tree ?? throw new ArgumentNullException(nameof(tree));
            SemanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));
        }

        private SemanticModel SemanticModel { get; }

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

        public IEnumerable<ClassModel> Extract(SyntaxNode sourceSymbol, IUnitTestGeneratorOptions options)
        {
            var models = ExtractClassModels(Tree, options).ToList();
            if (sourceSymbol != null)
            {
                models.Each(x => x.SetShouldGenerateForSingleItem(sourceSymbol));
            }

            return models;
        }

        private static IList<Func<SyntaxTokenList, bool>> GetAllowedModifiers(TypeDeclarationSyntax syntax, IUnitTestGeneratorOptions options)
        {
            var functionList = new List<Func<SyntaxTokenList, bool>>();

            // allow public
            functionList.Add(list => list.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword)));

            // if we are allowing internals
            if (options.GenerationOptions.EmitTestsForInternals)
            {
                if (options.GenerationOptions.EmitSubclassForProtectedMethods)
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
            else if (options.GenerationOptions.EmitSubclassForProtectedMethods)
            {
                // not allowing internals - so just protected and not protected internal
                functionList.Add(list => list.Any(modifier => modifier.IsKind(SyntaxKind.ProtectedKeyword)) && !list.Any(modifier => modifier.IsKind(SyntaxKind.InternalKeyword)));
            }

            return functionList;
        }

        private void AddModels<TIn, TOut>(TypeDeclarationSyntax type, Func<TIn, SyntaxTokenList> modifiersSelector, Func<TIn, TOut> converter, IList<Func<SyntaxTokenList, bool>> allowedModifiers, ICollection<TOut> target)
        {
            foreach (var model in type.ChildNodes().OfType<TIn>().Where(x => allowedModifiers.Any(modifierFilter => modifierFilter(modifiersSelector(x)))).Select(converter))
            {
                target.Add(model);
            }
        }

        private ClassModel ExtractClassModel(TypeDeclarationSyntax syntax, IUnitTestGeneratorOptions options)
        {
            var allowedModifiers = GetAllowedModifiers(syntax, options);

            var model = new ClassModel(syntax, SemanticModel, false);

            AddModels<ConstructorDeclarationSyntax, IConstructorModel>(syntax, x => x.Modifiers, ExtractConstructorModel, allowedModifiers, model.Constructors);
            AddModels<OperatorDeclarationSyntax, IOperatorModel>(syntax, x => x.Modifiers, ExtractOperatorModel, allowedModifiers, model.Operators);
            AddModels<MethodDeclarationSyntax, IMethodModel>(syntax, x => x.Modifiers, ExtractMethodModel, allowedModifiers, model.Methods);
            AddModels<PropertyDeclarationSyntax, IPropertyModel>(syntax, x => x.Modifiers, ExtractPropertyModel, allowedModifiers, model.Properties);
            AddModels<IndexerDeclarationSyntax, IIndexerModel>(syntax, x => x.Modifiers, ExtractIndexerModel, allowedModifiers, model.Indexers);

            if (syntax is RecordDeclarationSyntax record)
            {
                if (record.ParameterList != null)
                {
                    var dummyContext = new GenerationContext();
                    foreach (var recordContstructor in model.TypeSymbol.Constructors.Where(x => x.IsImplicitlyDeclared && !model.Constructors.Any(c => c == x)))
                    {
                        var constructor = SyntaxFactory.ConstructorDeclaration(model.ClassName);
                        List<ParameterModel> parameters;

                        if (record.ParameterList != null)
                        {
                            constructor = constructor.WithParameterList(record.ParameterList);
                            parameters = ExtractParameters(record.ParameterList.Parameters);
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

                            model.Properties.Add(new PropertyModel(propertyName, property, parameter.TypeInfo, SemanticModel, allProperties.FirstOrDefault(x => x.Name == propertyName)));
                        }
                    }
                }
            }

            foreach (var methodModel in syntax.ChildNodes().OfType<MethodDeclarationSyntax>().Where(x => x.ExplicitInterfaceSpecifier != null).Select(ExtractMethodModel))
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

        private IEnumerable<ClassModel> ExtractClassModels(SyntaxTree tree, IUnitTestGeneratorOptions options)
        {
            var root = tree.GetRoot();

            var typeList = GetTypeDeclarations(root);
            var fileUsings = root.DescendantNodesAndSelf().OfType<UsingDirectiveSyntax>().ToList();

            foreach (var syntax in typeList)
            {
                var model = ExtractClassModel(syntax, options);

                fileUsings.ForEach(model.Usings.Add);

                yield return model;
            }
        }

        private ConstructorModel ExtractConstructorModel(ConstructorDeclarationSyntax constructor)
        {
            var name = constructor.Identifier.ValueText;

            var parameters = ExtractParameters(constructor.ParameterList.Parameters);

            return new ConstructorModel(name, parameters, constructor);
        }

        private IndexerModel ExtractIndexerModel(IndexerDeclarationSyntax indexer)
        {
            var parameters = ExtractParameters(indexer.ParameterList.Parameters);

            var typeInfo = SemanticModel.GetTypeInfo(indexer.Type);

            return new IndexerModel("this", parameters, typeInfo, indexer);
        }

        private MethodModel ExtractMethodModel(MethodDeclarationSyntax method)
        {
            var methodName = method.Identifier.ValueText;

            var parameters = ExtractParameters(method.ParameterList.Parameters);

            return new MethodModel(methodName, parameters, method, SemanticModel);
        }

        private OperatorModel ExtractOperatorModel(OperatorDeclarationSyntax operatorSyntax)
        {
            var methodName = operatorSyntax.OperatorToken.ValueText;

            var parameters = ExtractParameters(operatorSyntax.ParameterList.Parameters);

            return new OperatorModel(methodName, parameters, operatorSyntax, SemanticModel);
        }

        private List<ParameterModel> ExtractParameters(SeparatedSyntaxList<ParameterSyntax> parameterList)
        {
            var parameters = new List<ParameterModel>();

            foreach (var parameter in parameterList)
            {
                var typeModel = SemanticModel.GetDeclaredSymbol(parameter);
                var typeInfo = SemanticModel.GetTypeInfo(parameter.Type);

                parameters.Add(new ParameterModel(typeModel.Name, parameter, typeModel.ToDisplayString(), typeInfo));
            }

            return parameters;
        }

        private PropertyModel ExtractPropertyModel(PropertyDeclarationSyntax property)
        {
            var propertyName = property.Identifier.ValueText;

            var typeInfo = SemanticModel.GetTypeInfo(property.Type);

            return new PropertyModel(propertyName, property, typeInfo, SemanticModel, null);
        }
    }
}