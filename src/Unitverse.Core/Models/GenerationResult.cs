namespace Unitverse.Core.Models
{
    using System.Collections.Generic;
    using Unitverse.Core.Assets;

    public class GenerationResult
    {
        public GenerationResult(string fileContent, bool anyMethodsEmitted)
        {
            FileContent = fileContent;
            AnyMethodsEmitted = anyMethodsEmitted;
        }

        public string FileContent { get; }

        public bool AnyMethodsEmitted { get; }

        public IList<TargetAsset> RequiredAssets { get; } = new List<TargetAsset>();
    }
}