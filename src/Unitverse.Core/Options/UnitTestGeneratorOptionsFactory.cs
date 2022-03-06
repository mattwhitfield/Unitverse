namespace Unitverse.Core.Options
{
    using System.Collections.Generic;
    using System.Linq;
    using EditorConfig.Core;

    public static class UnitTestGeneratorOptionsFactory
    {
        public static IUnitTestGeneratorOptions Create(string projectFilePath, IGenerationOptions generationOptions, INamingOptions namingOptions, IStrategyOptions strategyOptions, bool statisticsGenerationEnabled)
        {
            var mutableGenerationOptions = new MutableGenerationOptions(generationOptions);
            var mutableNamingOptions = new MutableNamingOptions(namingOptions);
            var mutableStrategyOptions = new MutableStrategyOptions(strategyOptions);

            if (!string.IsNullOrWhiteSpace(projectFilePath))
            {
                var allFiles = new EditorConfigParser(CoreConstants.ConfigFileName).GetConfigurationFilesTillRoot(projectFilePath);
                var allProperties = allFiles.SelectMany(x => x.Sections).SelectMany(x => x);
                var properties = new Dictionary<string, string>();
                foreach (var pair in allProperties)
                {
                    properties[pair.Key] = pair.Value;
                }

                properties.ApplyTo(mutableGenerationOptions);
                properties.ApplyTo(mutableNamingOptions);
                properties.ApplyTo(mutableStrategyOptions);
            }

            return new UnitTestGeneratorOptions(mutableGenerationOptions, mutableNamingOptions, mutableStrategyOptions, statisticsGenerationEnabled);
        }
    }
}
