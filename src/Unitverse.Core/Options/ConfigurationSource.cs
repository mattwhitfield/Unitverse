namespace Unitverse.Core.Options
{
    public class ConfigurationSource
    {
        public ConfigurationSource(ConfigurationSourceType sourceType, string? fileName = null)
        {
            SourceType = sourceType;
            FileName = fileName;
        }

        public ConfigurationSourceType SourceType { get; }

        public string? FileName { get; }
    }
}
