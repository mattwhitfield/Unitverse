namespace Unitverse.Core.Generation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Options;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Models;

    public abstract class CompilationUnitStrategy : ICompilationUnitStrategy
    {
        public CompilationUnitStrategy(SemanticModel sourceModel, SemanticModel? targetModel, IGenerationItem generationItem, Solution? solution, DocumentOptionSet? documentOptions, string sourceNamespaceName, string targetNamespaceName)
        {
            SourceModel = sourceModel;
            TargetModel = targetModel;
            GenerationItem = generationItem;
            Solution = solution;
            DocumentOptions = documentOptions;
            SourceNamespaceName = sourceNamespaceName;
            TargetNamespaceName = targetNamespaceName;
        }

        public SemanticModel SourceModel { get; }

        public SemanticModel? TargetModel { get; }

        public IGenerationItem GenerationItem { get; }

        public Solution? Solution { get; }

        public DocumentOptionSet? DocumentOptions { get; }

        public string SourceNamespaceName { get; }

        public string TargetNamespaceName { get; }

        private HashSet<string> _usingsEmitted = new HashSet<string>();

        protected List<UsingDirectiveSyntax> AddedUsings { get; } = new List<UsingDirectiveSyntax>();

        public async Task Initialize()
        {
            SyntaxNode? targetTree = null;
            if (TargetModel != null)
            {
                targetTree = await TargetModel.SyntaxTree.GetRootAsync();
                foreach (var syntax in targetTree.DescendantNodes().OfType<UsingDirectiveSyntax>())
                {
                    _usingsEmitted.Add(syntax.NormalizeWhitespace().ToFullString());
                }
            }

            InitializeInternal(targetTree);
        }

        public void AddUsing(UsingDirectiveSyntax usingDirective)
        {
            AddedUsings.Add(usingDirective);
        }

        protected abstract void InitializeInternal(SyntaxNode? targetTree);

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
            foreach (var usingDirective in AddedUsings)
            {
                var fullString = usingDirective.NormalizeWhitespace().ToFullString();
                if (_usingsEmitted.Add(fullString))
                {
                    if (usingDirective.Name is IdentifierNameSyntax ins && ins.Identifier.ValueText.StartsWith("<global", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    AddUsingToTarget(usingDirective);
                }
            }
        }

        protected abstract void AddUsingToTarget(UsingDirectiveSyntax usingDirective);

        public abstract void AddTypeToTarget(TypeDeclarationSyntax targetType, TypeDeclarationSyntax? originalTargetType);

        public abstract CompilationUnitSyntax RenderCompilationUnit();

        public abstract SyntaxNode TargetRoot { get; }
    }
}
