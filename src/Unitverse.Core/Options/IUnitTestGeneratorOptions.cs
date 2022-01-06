namespace Unitverse.Core.Options
{
    public interface IUnitTestGeneratorOptions
    {
        bool StatisticsCollectionEnabled { get; }

        IGenerationOptions GenerationOptions { get; }

        INamingOptions NamingOptions { get; }
    }
}