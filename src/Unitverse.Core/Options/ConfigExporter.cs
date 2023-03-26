namespace Unitverse.Core.Options
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public static class ConfigExporter
    {
        private static readonly Dictionary<string, string> Categories = new Dictionary<string, string>();

        static ConfigExporter()
        {
            AddToCategories(typeof(IGenerationOptions), "GenerationOptions");
            AddToCategories(typeof(INamingOptions), "NamingOptions");
            AddToCategories(typeof(IStrategyOptions), "StrategyOptions");
        }

        private static void AddToCategories(Type type, string name)
        {
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanRead))
            {
                Categories[property.Name] = name;
            }
        }

        public static void WriteTo(string targetFileName, IUnitTestGeneratorOptions options)
        {
            var objects = new[]
            {
                Tuple.Create<object, string>(options.GenerationOptions, "GenerationOptions"),
                Tuple.Create<object, string>(options.NamingOptions, "NamingOptions"),
                Tuple.Create<object, string>(options.StrategyOptions, "StrategyOptions"),
            };

            Write(targetFileName, objects, null);
        }

        public static void WriteTo(string targetFileName, IEnumerable<object> sources, IDictionary<string, string> mappings)
        {
            Write(targetFileName, sources.Select(x => Tuple.Create(x, x.GetType().Name)), mappings);
        }

        private static void Write(string targetFileName, IEnumerable<Tuple<object, string>> sources, IDictionary<string, string>? mappings)
        {
            if (string.IsNullOrWhiteSpace(targetFileName))
            {
                throw new ArgumentNullException(nameof(targetFileName));
            }

            if (sources is null)
            {
                throw new ArgumentNullException(nameof(sources));
            }

            var first = true;
            using (var writer = new StreamWriter(targetFileName, false, Encoding.UTF8))
            {
                foreach (var tuple in sources)
                {
                    if (!first)
                    {
                        writer.WriteLine();
                    }

                    first = false;

                    var source = tuple.Item1;
                    var type = source.GetType();
                    writer.WriteLine("[" + tuple.Item2 + "]");
                    foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanRead && x.CanWrite))
                    {
                        if (property.Name.Equals("Site", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        var propertyValue = property.GetValue(source).ToString();
                        var propertyName = property.Name;

                        writer.WriteLine(propertyName + "=" + propertyValue);
                    }
                }

                if (mappings != null && mappings.Any())
                {
                    writer.WriteLine();
                    writer.WriteLine("[Mappings]");

                    foreach (var pair in mappings.OrderBy(x => x.Key))
                    {
                        writer.WriteLine(pair.Key + "=" + pair.Value);
                    }
                }
            }
        }

        public static void WriteSettings(string fileName, Dictionary<string, string> settings, string sourceProjectName, string targetProjectName)
        {
            var categorisedSettings = new Dictionary<string, Dictionary<string, string>>();
            foreach (var setting in settings)
            {
                if (!Categories.TryGetValue(setting.Key, out var categoryName))
                {
                    categoryName = string.Empty;
                }

                if (!categorisedSettings.TryGetValue(categoryName, out var category))
                {
                    categorisedSettings[categoryName] = category = new Dictionary<string, string>();
                }

                category[setting.Key] = setting.Value;
            }

            var first = true;
            using (var writer = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                foreach (var category in categorisedSettings.OrderBy(x => x.Key))
                {
                    if (!first)
                    {
                        writer.WriteLine();
                    }

                    first = false;

                    if (!string.IsNullOrWhiteSpace(category.Key))
                    {
                        writer.WriteLine("[" + category.Key + "]");
                    }

                    foreach (var pair in category.Value.OrderBy(x => x.Key))
                    {
                        writer.WriteLine(pair.Key + "=" + pair.Value);
                    }
                }

                if (!string.IsNullOrWhiteSpace(sourceProjectName) && !string.IsNullOrWhiteSpace(targetProjectName))
                {
                    if (!first)
                    {
                        writer.WriteLine();
                    }

                    writer.WriteLine("[Mappings]");
                    writer.WriteLine(sourceProjectName + "=" + targetProjectName);
                }
            }
        }
    }
}
