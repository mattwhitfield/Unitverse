namespace SentryOne.UnitTestGenerator.Core.Options
{
    using System;

    public class UnitTestGeneratorOptions : IUnitTestGeneratorOptions
    {
        public UnitTestGeneratorOptions(IGenerationOptions generationOptions, IVersioningOptions versioningOptions)
        {
            GenerationOptions = generationOptions ?? throw new ArgumentNullException(nameof(generationOptions));
            VersioningOptions = versioningOptions ?? throw new ArgumentNullException(nameof(versioningOptions));
        }

        public IGenerationOptions GenerationOptions { get; }

        public IVersioningOptions VersioningOptions { get; }
    }
}