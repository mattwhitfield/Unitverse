namespace SentryOne.UnitTestGenerator.Core.Models
{
    using System.Collections.Generic;
    using SentryOne.UnitTestGenerator.Core.Assets;

    public class GenerationResult
    {
        public GenerationResult(string fileContent)
        {
            FileContent = fileContent;
        }

        public string FileContent { get; }

        public IList<TargetAsset> RequiredAssets { get; } = new List<TargetAsset>();

        public IList<IReferencedAssembly> AssemblyReferences { get; } = new List<IReferencedAssembly>();
    }
}