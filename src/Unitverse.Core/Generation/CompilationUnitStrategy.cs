namespace Unitverse.Core.Generation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Options;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;

#if VS2022
    using BaseNamespace = Microsoft.CodeAnalysis.CSharp.Syntax.BaseNamespaceDeclarationSyntax;
#else
    using BaseNamespace = Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax;
#endif

    public abstract class CompilationUnitStrategy : ICompilationUnitStrategy
    {
        public CompilationUnitStrategy(SemanticModel sourceModel, IGenerationItem generationItem, DocumentOptionSet? documentOptions, CompilationUnitSyntax compilation, BaseNamespace targetNamespace, BaseNamespace? originalTargetNamespace)
        {
            SourceModel = sourceModel;
            GenerationItem = generationItem;
            DocumentOptions = documentOptions;
            Compilation = compilation;
            TargetNamespace = targetNamespace;
            OriginalTargetNamespace = originalTargetNamespace;
        }

        public SemanticModel SourceModel { get; }

        public IGenerationItem GenerationItem { get; }

        public DocumentOptionSet? DocumentOptions { get; }

        protected CompilationUnitSyntax Compilation { get; set; }

        protected BaseNamespace TargetNamespace { get; set; }

        protected BaseNamespace? OriginalTargetNamespace { get; set; }

        private List<UsingDirectiveSyntax> _addedUsings = new List<UsingDirectiveSyntax>();

        public void AddUsing(UsingDirectiveSyntax usingDirective)
        {
            _addedUsings.Add(usingDirective);
        }

        protected void UpdateOriginalTargetNamespace()
        {
            // find the node represented by 'OriginalTargetNamespace' in the current 'Compilation'
            if (OriginalTargetNamespace == null)
            {
                return;
            }

            if (Compilation.DescendantNodes().Any(x => x == OriginalTargetNamespace))
            {
                return;
            }

            var searchName = OriginalTargetNamespace.Name.ToString();
            OriginalTargetNamespace = Compilation.DescendantNodes().OfType<BaseNamespace>().FirstOrDefault(x => x.Name.ToString() == searchName);
        }

        protected TypeDeclarationSyntax? FindTypeNode(SyntaxNode parent, TypeDeclarationSyntax? searchNode)
        {
            if (searchNode == null)
            {
                return null;
            }

            return parent.DescendantNodes().OfType<TypeDeclarationSyntax>().FirstOrDefault(x => x.Identifier.ValueText == searchNode.Identifier.ValueText);
        }

        public void AddTypeParameterAliases(ClassModel classModel, IGenerationContext context)
        {
            foreach (var parameter in classModel.Declaration.TypeParameterList?.Parameters ?? Enumerable.Empty<TypeParameterSyntax>())
            {
                var aliasedName = parameter.Identifier.ToString();
                if (TargetRoot.DescendantNodes().OfType<UsingDirectiveSyntax>().Any(node => string.Equals(node.Alias?.Name?.ToString(), aliasedName, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

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

                if (!context.GenericTypes.ContainsKey(parameter.Identifier.ValueText))
                {
                    context.GenericTypes[parameter.Identifier.ValueText] = derivedType;
                    AddUsing(SyntaxFactory.UsingDirective(nameSyntax)
                            .WithAlias(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(parameter.Identifier))));
                }
            }
        }

        protected void EmitUsingStatements()
        {
            bool systemUsingsFirst = GenerationItem.Options.GenerationOptions.PlaceSystemUsingDirectivesFirst;

            _addedUsings.AddRange(Compilation.Usings);
            _addedUsings.AddRange(TargetNamespace.Usings);

            var categorizedUsings = new CategorizedUsings(_addedUsings, systemUsingsFirst);
            var resolvedUsings = categorizedUsings.GetResolvedUsingDirectives();

            AddUsingsToTarget(resolvedUsings);
        }

        protected void AddUsingsToTarget(IEnumerable<UsingDirectiveSyntax> usingDirectives)
        {
            if (GenerationItem.Options.GenerationOptions.EmitUsingsOutsideNamespace)
            {
                Compilation = Compilation.WithUsings(SyntaxFactory.List(usingDirectives));
                TargetNamespace = TargetNamespace.WithUsings(SyntaxFactory.List<UsingDirectiveSyntax>());
            }
            else
            {
                Compilation = Compilation.WithUsings(SyntaxFactory.List<UsingDirectiveSyntax>());
                TargetNamespace = TargetNamespace.WithUsings(SyntaxFactory.List(usingDirectives));
            }
        }

        public abstract void AddTypeToTarget(TypeDeclarationSyntax targetType, TypeDeclarationSyntax? originalTargetType);

        public abstract CompilationUnitSyntax RenderCompilationUnit();

        public abstract SyntaxNode TargetRoot { get; }
    }
}
