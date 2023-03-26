namespace Unitverse.Tests.Common
{
    using Unitverse.Core.Options;

    public class TestGeneratorOptions : UnitTestGeneratorOptions
    {
        public TestGeneratorOptions(IGenerationOptions generationOptions)
            : base(generationOptions, new DefaultNamingOptions(), new DefaultStrategyOptions(), false, null, null, string.Empty, string.Empty)
        {
        }

        public TestGeneratorOptions(IGenerationOptions generationOptions, INamingOptions namingOptions, IStrategyOptions strategyOptions)
            : base(generationOptions, namingOptions, strategyOptions, false, null, null, string.Empty, string.Empty)
        {
        }
    }
}
