namespace Unitverse.Tests.Common
{
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;

    public static class DefaultGenerationContext
    {
        public static GenerationContext Create()
        {
            return new GenerationContext(new DefaultGenerationOptions(), new NamingProvider(new DefaultNamingOptions()));
        }
    }
}
