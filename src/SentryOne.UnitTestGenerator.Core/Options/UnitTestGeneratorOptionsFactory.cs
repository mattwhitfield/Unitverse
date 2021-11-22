namespace SentryOne.UnitTestGenerator.Core.Options
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EditorConfig.Core;

    public static class UnitTestGeneratorOptionsFactory
    {
        public static IUnitTestGeneratorOptions Create(string solutionFilePath, IGenerationOptions generationOptions)
        {
            if (generationOptions == null)
            {
                throw new ArgumentNullException(nameof(generationOptions));
            }

            var mutableGenerationOptions = new MutableGenerationOptions(generationOptions);

            if (!string.IsNullOrWhiteSpace(solutionFilePath))
            {
                var allFiles = new EditorConfigParser(".unitTestGeneratorConfig").GetConfigurationFilesTillRoot(solutionFilePath);
                var allProperties = allFiles.SelectMany(x => x.Sections).SelectMany(x => x);
                var properties = new Dictionary<string, string>();
                foreach (var pair in allProperties)
                {
                    properties[pair.Key] = pair.Value;
                }

                properties.ApplyTo(mutableGenerationOptions);
            }

            return new UnitTestGeneratorOptions(mutableGenerationOptions);
        }
    }
}
