namespace SentryOne.UnitTestGenerator.Core.Options
{
    using System;

    public class UnitTestGeneratorOptions : IUnitTestGeneratorOptions
    {
        public UnitTestGeneratorOptions(IGenerationOptions generationOptions)
        {
            GenerationOptions = generationOptions ?? throw new ArgumentNullException(nameof(generationOptions));
        }

        public IGenerationOptions GenerationOptions { get; }
    }
}