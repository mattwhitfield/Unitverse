namespace Unitverse.Core.Helpers
{
    public class GenerationStatistics : IGenerationStatistics
    {
        public long InterfacesMocked { get; set; }

        public long TypesConstructed { get; set; }

        public long ValuesGenerated { get; set; }

        public long TestClassesGenerated { get; set; }

        public long TestMethodsGenerated { get; set; }

        public long TestMethodsRegenerated { get; set; }
    }
}