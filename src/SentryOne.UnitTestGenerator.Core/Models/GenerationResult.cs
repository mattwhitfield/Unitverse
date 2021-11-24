namespace Unitverse.Core.Models
{
    using System.Collections.Generic;
    using Unitverse.Core.Assets;

    public class GenerationResult
    {
        public GenerationResult(string fileContent)
        {
            FileContent = fileContent;
        }

        public string FileContent { get; }

        public IList<TargetAsset> RequiredAssets { get; } = new List<TargetAsset>();
    }
}