namespace Unitverse.Core.Options
{
    using System;

    public class UnitTestGeneratorOptions : IUnitTestGeneratorOptions
    {
        public UnitTestGeneratorOptions(IGenerationOptions generationOptions, INamingOptions namingOptions)
        {
            GenerationOptions = generationOptions ?? throw new ArgumentNullException(nameof(generationOptions));
            NamingOptions = namingOptions ?? throw new ArgumentNullException(nameof(namingOptions));
        }

        public IGenerationOptions GenerationOptions { get; }

        public INamingOptions NamingOptions { get; }
    }
}