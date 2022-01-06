namespace Unitverse.Core.Helpers
{
    public interface IGenerationStatistics
    {
        long InterfacesMocked { get; set; }

        long TypesConstructed { get; set; }

        long ValuesGenerated { get; set; }

        long TestClassesGenerated { get; set; }

        long TestMethodsGenerated { get; set; }

        long TestMethodsRegenerated { get; set; }
    }
}