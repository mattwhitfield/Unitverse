namespace SentryOne.UnitTestGenerator.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Models;

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
            return typeList;
        }

        public IEnumerable<ClassModel> Extract(SyntaxNode sourceSymbol)
        {
            var models = ExtractClassModels(Tree).ToList();
            if (sourceSymbol != null)
            {
                models.Each(x => x.SetShouldGenerateForSingleItem(sourceSymbol));
            }

            return models;
        }

        private static HashSet<SyntaxKind> GetAllowedModifiers(TypeDeclarationSyntax syntax)
        {
            var allowedModifiers = new HashSet<SyntaxKind> { SyntaxKind.PublicKeyword };
            if (syntax.Modifiers.Any(x => x.IsKind(SyntaxKind.AbstractKeyword)))
            {
                allowedModifiers.Add(SyntaxKind.ProtectedKeyword);
            }

            return allowedModifiers;
        }

        private void AddModels<TIn, TOut>(TypeDeclarationSyntax type, Func<TIn, SyntaxTokenList> modifiersSelector, Func<TIn, TOut> converter, ICollection<SyntaxKind> allowedModifiers, ICollection<TOut> target)
        {
            foreach (var model in type.ChildNodes().OfType<TIn>().Where(x => modifiersSelector(x).Any(m => allowedModifiers.Contains(m.Kind()))).Select(converter))
            {
                target.Add(model);
            }
        }

        private ClassModel ExtractClassModel(TypeDeclarationSyntax syntax)
        {
            var allowedModifiers = GetAllowedModifiers(syntax);

            var model = new ClassModel(syntax, SemanticModel, false);

            AddModels<ConstructorDeclarationSyntax, IConstructorModel>(syntax, x => x.Modifiers, ExtractConstructorModel, allowedModifiers, model.Constructors);
            AddModels<OperatorDeclarationSyntax, IOperatorModel>(syntax, x => x.Modifiers, ExtractOperatorModel, allowedModifiers, model.Operators);
            AddModels<MethodDeclarationSyntax, IMethodModel>(syntax, x => x.Modifiers, ExtractMethodModel, allowedModifiers, model.Methods);
            AddModels<PropertyDeclarationSyntax, IPropertyModel>(syntax, x => x.Modifiers, ExtractPropertyModel, allowedModifiers, model.Properties);
            AddModels<IndexerDeclarationSyntax, IIndexerModel>(syntax, x => x.Modifiers, ExtractIndexerModel, allowedModifiers, model.Indexers);

            foreach (var methodModel in syntax.ChildNodes().OfType<MethodDeclarationSyntax>().Where(x => x.ExplicitInterfaceSpecifier != null).Select(ExtractMethodModel))
            {
                model.Methods.Add(methodModel);
            }

            model.DefaultConstructor = model.Constructors.OrderByDescending(x => x.Parameters.Count).FirstOrDefault();

            // populate interface models
            if (syntax is ClassDeclarationSyntax classDeclaration)
            {
                var declaredSymbol = SemanticModel.GetDeclaredSymbol(classDeclaration);
                foreach (var declaredSymbolInterface in declaredSymbol.Interfaces)
                {
                    model.Interfaces.Add(new InterfaceModel(declaredSymbolInterface));
                }
            }

            return model;
        }

        private IEnumerable<ClassModel> ExtractClassModels(SyntaxTree tree)
        {
            var root = tree.GetRoot();

            var typeList = GetTypeDeclarations(root);
            var fileUsings = root.DescendantNodesAndSelf().OfType<UsingDirectiveSyntax>().ToList();

            foreach (var syntax in typeList)
            {
                var model = ExtractClassModel(syntax);

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

            return new PropertyModel(propertyName, property, typeInfo);
        }
    }
}