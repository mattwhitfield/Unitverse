namespace Unitverse.Core.Models
{
    using System;
    using System.Collections.Generic;
    using Unitverse.Core.Assets;
    using Unitverse.Core.Helpers;

    public class GenerationResult : IGenerationStatistics
    {
        public GenerationResult(string fileContent, bool anyMethodsEmitted, IGenerationStatistics sourceStatistics)
        {
            if (sourceStatistics is null)
            {
                throw new ArgumentNullException(nameof(sourceStatistics));
            }

            FileContent = ContentCleaner.Clean(fileContent);
            AnyMethodsEmitted = anyMethodsEmitted;
            InterfacesMocked = sourceStatistics.InterfacesMocked;
            TypesConstructed = sourceStatistics.TypesConstructed;
            ValuesGenerated = sourceStatistics.ValuesGenerated;
            TestClassesGenerated = sourceStatistics.TestClassesGenerated;
            TestMethodsGenerated = sourceStatistics.TestMethodsGenerated;
            TestMethodsRegenerated = sourceStatistics.TestMethodsRegenerated;
        }

        public string FileContent { get; }

        public bool AnyMethodsEmitted { get; }

        public IList<TargetAsset> RequiredAssets { get; } = new List<TargetAsset>();

        public long InterfacesMocked { get; set; }

        public long TypesConstructed { get; set; }

        public long ValuesGenerated { get; set; }

        public long TestClassesGenerated { get; set; }

        public long TestMethodsGenerated { get; set; }

        public long TestMethodsRegenerated { get; set; }
    }
}