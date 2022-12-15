namespace Unitverse.Core.Generation
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Options;

    public abstract class CompilationUnitStrategyBootstrapper : ICompilationUnitStrategyBootstrapper
    {
        public CompilationUnitStrategyBootstrapper(SemanticModel sourceModel, SemanticModel? targetModel, IGenerationItem generationItem, DocumentOptionSet? documentOptions, string targetNamespaceName)
        {
            SourceModel = sourceModel;
            TargetModel = targetModel;
            GenerationItem = generationItem;
            DocumentOptions = documentOptions;
            TargetNamespaceName = targetNamespaceName;
        }

        public SemanticModel SourceModel { get; }

        public SemanticModel? TargetModel { get; }

        public IGenerationItem GenerationItem { get; }

        public DocumentOptionSet? DocumentOptions { get; }

        public string TargetNamespaceName { get; }

        public async Task<ICompilationUnitStrategy> Initialize()
        {
            SyntaxNode? targetTree = null;
            if (TargetModel != null)
            {
                targetTree = await TargetModel.SyntaxTree.GetRootAsync();
            }

            return InitializeInternal(targetTree);
        }

        protected abstract ICompilationUnitStrategy InitializeInternal(SyntaxNode? targetTree);
    }
}
