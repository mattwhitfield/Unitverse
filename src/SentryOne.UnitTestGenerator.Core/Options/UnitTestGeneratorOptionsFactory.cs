namespace SentryOne.UnitTestGenerator.Core.Options
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EditorConfig.Core;

    public static class UnitTestGeneratorOptionsFactory
    {
        public static IUnitTestGeneratorOptions Create(string solutionFilePath, IGenerationOptions generationOptions, IVersioningOptions versioningOptions)
        {
            if (generationOptions == null)
            {
                throw new ArgumentNullException(nameof(generationOptions));
            }

            if (versioningOptions == null)
            {
                throw new ArgumentNullException(nameof(versioningOptions));
            }

            var mutableGenerationOptions = new MutableGenerationOptions(generationOptions);
            var mutableVersioningOptions = new MutableVersioningOptions(versioningOptions);

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
                properties.ApplyTo(mutableVersioningOptions);
            }

            return new UnitTestGeneratorOptions(mutableGenerationOptions, mutableVersioningOptions);
        }
    }
}
