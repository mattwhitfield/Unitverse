﻿namespace Unitverse.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Unitverse.Core.Assets;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;

    public class ClassModel
    {
        public ClassModel(TypeDeclarationSyntax declaration, SemanticModel semanticModel, bool isSingleItem)
        {
            Declaration = declaration ?? throw new ArgumentNullException(nameof(declaration));
            SemanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));
            IsSingleItem = isSingleItem;
            TargetFieldName = "_testClass";
            TypeSymbol = SemanticModel.GetDeclaredSymbol(declaration);

            TypeSyntax = SyntaxFactory.ParseTypeName(TypeSymbol.ToDisplayString(new SymbolDisplayFormat(
                SymbolDisplayGlobalNamespaceStyle.Omitted,
                SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
                SymbolDisplayGenericsOptions.IncludeTypeParameters,
                miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes)));

            DependencyMap = ConstructorFieldAssignmentExtractor.ExtractMapFrom(declaration, semanticModel);

            foreach (var interfaceImpl in TypeSymbol.AllInterfaces)
            {
                foreach (var interfaceMember in interfaceImpl.GetMembers())
                {
                    var implementation = TypeSymbol.FindImplementationForInterfaceMember(interfaceMember);
                    if (implementation != null)
                    {
                        if (!_implementedInterfaceSymbols.TryGetValue(implementation, out var list))
                        {
                            _implementedInterfaceSymbols[implementation] = list = new List<ISymbol>();
                        }

                        list.Add(interfaceMember);
                    }
                }
            }
        }

        public ClassDependencyMap DependencyMap { get; }

        public INamedTypeSymbol TypeSymbol { get; }

        public string ClassName => Declaration.GetClassName();

        public IList<IConstructorModel> Constructors { get; } = new List<IConstructorModel>();

        public TypeDeclarationSyntax Declaration { get; }

        public IConstructorModel DefaultConstructor { get; set; }

        public IList<IIndexerModel> Indexers { get; } = new List<IIndexerModel>();

        public bool ShouldGenerate { get; set; } = true;

        public bool IsSingleItem { get; }

        public string TargetFieldName { get; private set; }

        public bool IsStatic => Declaration.Modifiers.Any(x => string.Equals(x.ValueText, "static", StringComparison.OrdinalIgnoreCase));

        public IList<IMethodModel> Methods { get; } = new List<IMethodModel>();

        public IList<IOperatorModel> Operators { get; } = new List<IOperatorModel>();

        public IList<IPropertyModel> Properties { get; } = new List<IPropertyModel>();

        public IList<TargetAsset> RequiredAssets { get; } = new List<TargetAsset>();

        public IList<IInterfaceModel> Interfaces { get; } = new List<IInterfaceModel>();

        public SemanticModel SemanticModel { get; }

        public ExpressionSyntax TargetInstance { get; set; } = SyntaxFactory.IdentifierName("_testClass");

        public TypeSyntax TypeSyntax { get; set; }

        public IList<UsingDirectiveSyntax> Usings { get; } = new List<UsingDirectiveSyntax>();

        private IDictionary<ISymbol, IList<ISymbol>> _implementedInterfaceSymbols { get; } = new Dictionary<ISymbol, IList<ISymbol>>();

        public IList<ISymbol> GetImplementedInterfaceSymbolsFor(ISymbol symbol)
        {
            if (symbol != null && _implementedInterfaceSymbols.TryGetValue(symbol, out var list))
            {
                return list;
            }

            return new List<ISymbol>();
        }

        public void SetShouldGenerateForSingleItem(SyntaxNode syntaxNode)
        {
            ShouldGenerate = TypeSyntax == syntaxNode;
            Methods.Each(x => x.SetShouldGenerateForSingleItem(syntaxNode));
            Operators.Each(x => x.SetShouldGenerateForSingleItem(syntaxNode));
            Properties.Each(x => x.SetShouldGenerateForSingleItem(syntaxNode));
            Constructors.Each(x => x.SetShouldGenerateForSingleItem(syntaxNode));
            Indexers.Each(x => x.SetShouldGenerateForSingleItem(syntaxNode));
        }

        public string GetConstructorParameterFieldName(ParameterModel parameter, INamingProvider namingProvider)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (namingProvider is null)
            {
                throw new ArgumentNullException(nameof(namingProvider));
            }

            var namingContext = new NamingContext(ClassName).WithParameterName(parameter.Name);

            var baseFieldName = namingProvider.DependencyFieldName.Resolve(namingContext);

            if (Constructors.SelectMany(x => x.Parameters).Where(x => string.Equals(x.Name, parameter.Name, StringComparison.OrdinalIgnoreCase)).Select(x => x.Type).Distinct().Count() < 2)
            {
                return baseFieldName;
            }

            return baseFieldName + parameter.TypeInfo.Type.Name;
        }

        public string GetIndexerName(IIndexerModel indexer)
        {
            if (indexer == null)
            {
                throw new ArgumentNullException(nameof(indexer));
            }

            if (Indexers.Count < 2)
            {
                return "Indexer";
            }

            return "IndexerFor" + indexer.Parameters.Select(x => x.TypeInfo.Type.GetLastNamePart().ToPascalCase()).Aggregate((x, y) => x + "And" + y);
        }

        public ExpressionSyntax GetConstructorFieldReference(ParameterModel model, IFrameworkSet frameworkSet)
        {
            var identifierName = SyntaxFactory.IdentifierName(GetConstructorParameterFieldName(model, frameworkSet.NamingProvider));
            if (model.TypeInfo.Type.TypeKind == TypeKind.Interface)
            {
                return frameworkSet.MockingFramework.GetFieldReference(identifierName);
            }

            return identifierName;
        }

        public ExpressionSyntax GetObjectCreationExpression(IFrameworkSet frameworkSet)
        {
            if (frameworkSet == null)
            {
                throw new ArgumentNullException(nameof(frameworkSet));
            }

            var targetConstructor = Constructors.OrderByDescending(x => x.Parameters.Count).FirstOrDefault();

            var objectCreation = SyntaxFactory.ObjectCreationExpression(TypeSyntax);

            if (targetConstructor != null && targetConstructor.Parameters.Count > 0)
            {
                return objectCreation.WithArgumentList(Generate.Arguments(targetConstructor.Parameters.Select(x => GetConstructorFieldReference(x, frameworkSet))));
            }

            if (targetConstructor != null || !Declaration.ChildNodes().OfType<ConstructorDeclarationSyntax>().Any())
            {
                return objectCreation.WithArgumentList(SyntaxFactory.ArgumentList());
            }

            return AssignmentValueHelper.GetDefaultAssignmentValue(TypeSymbol, SemanticModel, frameworkSet);
        }

        public string GetMethodUniqueName(IMethodModel method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (Methods.Count(x => x.OriginalName == method.OriginalName) == 1)
            {
                return method.OriginalName;
            }

            var parameters = new List<string>();

            var hasEquallyNamedOverload = Methods.Any(x => x != method && x.Parameters.Count == method.Parameters.Count && x.Parameters.Select(p => p.Name.ToPascalCase()).SequenceEqual(method.Parameters.Select(p => p.Name.ToPascalCase())));
            var hasEquallyTypedOverload = Methods.Any(x => x != method && x.Parameters.Count == method.Parameters.Count && x.Parameters.Select(p => p.Type.ToPascalCase()).SequenceEqual(method.Parameters.Select(p => p.Type.ToPascalCase())));

            if (hasEquallyTypedOverload && (method.Node?.TypeParameterList?.Parameters.Count ?? 0) > 0)
            {
                parameters.AddRange(method.Node.TypeParameterList.Parameters.Select(x => x.Identifier.ValueText));
            }

            for (int i = 0; i < method.Parameters.Count; i++)
            {
                var hasEquallyNamedParameter = Methods.Any(x => x != method && x.Parameters.Count == method.Parameters.Count && string.Equals(x.Parameters[i].Name, method.Parameters[i].Name, StringComparison.OrdinalIgnoreCase));
                if (hasEquallyNamedOverload && hasEquallyNamedParameter)
                {
                    parameters.Add(method.Parameters[i].TypeInfo.Type.ToIdentifierName().ToPascalCase());
                }
                else
                {
                    parameters.Add(method.Parameters[i].Name.ToPascalCase());
                }
            }

            var baseName = method.OriginalName;
            if (method.Node?.ExplicitInterfaceSpecifier != null)
            {
                baseName += "For" + Generate.CleanName(method.Node.ExplicitInterfaceSpecifier.Name.ToString());
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}With{1}", baseName, parameters.Any() ? parameters.Select(x => x.ToPascalCase()).Aggregate((x, y) => x + "And" + y) : "NoParameters");
        }

        internal void SetTargetInstance(string fieldName)
        {
            TargetFieldName = fieldName;
            TargetInstance = SyntaxFactory.IdentifierName(TargetFieldName);
        }
    }
}