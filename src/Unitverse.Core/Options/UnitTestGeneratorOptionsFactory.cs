namespace Unitverse.Core.Options
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using EditorConfig.Core;

    public static class UnitTestGeneratorOptionsFactory
    {
        public static IUnitTestGeneratorOptions Create(string projectFilePath, IUnitTestGeneratorOptions options)
        {
            return Create(projectFilePath, options.GenerationOptions, options.NamingOptions, options.StrategyOptions, options.StatisticsCollectionEnabled);
        }

        public static IUnitTestGeneratorOptions Create(string projectFilePath, IGenerationOptions generationOptions, INamingOptions namingOptions, IStrategyOptions strategyOptions, bool statisticsGenerationEnabled)
        {
            var mutableGenerationOptions = new MutableGenerationOptions(generationOptions);
            var mutableNamingOptions = new MutableNamingOptions(namingOptions);
            var mutableStrategyOptions = new MutableStrategyOptions(strategyOptions);

            var generationOptionsMutators = EditorConfigFieldMapper.CreateMutatorSet<MutableGenerationOptions>();
            var namingOptionsMutators = EditorConfigFieldMapper.CreateMutatorSet<MutableNamingOptions>();
            var strategyOptionsMutators = EditorConfigFieldMapper.CreateMutatorSet<MutableStrategyOptions>();

            var membersSetByFilename = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(projectFilePath))
            {
                var allFiles = new EditorConfigParser(CoreConstants.ConfigFileName).GetConfigurationFilesTillRoot(projectFilePath);

                foreach (var file in allFiles)
                {
                    foreach (var section in file.Sections)
                    {
                        foreach (var pair in section)
                        {
                            string? memberName;
                            var applied = Apply(mutableGenerationOptions, pair, generationOptionsMutators, out memberName) ||
                                          Apply(mutableNamingOptions, pair, namingOptionsMutators, out memberName) ||
                                          Apply(mutableStrategyOptions, pair, strategyOptionsMutators, out memberName);

                            if (applied && memberName != null)
                            {
                                // record
                                membersSetByFilename[memberName] = Path.Combine(file.Directory, CoreConstants.ConfigFileName);
                            }
                        }
                    }
                }
            }

            return new UnitTestGeneratorOptions(mutableGenerationOptions, mutableNamingOptions, mutableStrategyOptions, statisticsGenerationEnabled, membersSetByFilename);
        }

        private static bool Apply(object instance, KeyValuePair<string, string> valuePair, Dictionary<string, TypeMemberSetter> mutatorSet, out string? memberName)
        {
            var cleanFieldName = valuePair.Key.Replace("_", string.Empty).Replace("-", string.Empty);
            if (mutatorSet.TryGetValue(cleanFieldName, out var mutator))
            {
                memberName = cleanFieldName;
                return mutator(instance, valuePair.Value);
            }

            memberName = null;
            return false;
        }
    }
}
