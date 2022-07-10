namespace Unitverse.Core.Helpers
{
    using Unitverse.Core.Options;

    internal static class GenerationOptionsExtensions
    {
        public static bool CanUseAutoFixtureForMocking(this IGenerationOptions options)
        {
            return options.UseAutoFixture && options.UseAutoFixtureForMocking && options.MockingFrameworkType != MockingFrameworkType.JustMock;
        }
    }
}
