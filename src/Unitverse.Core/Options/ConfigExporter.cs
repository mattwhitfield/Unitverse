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
        public static void WriteTo(string targetFileName, IUnitTestGeneratorOptions options)
        {
            var objects = new[]
            {
                Tuple.Create<object, string>(options.GenerationOptions, "GenerationOptions"),
                Tuple.Create<object, string>(options.NamingOptions, "NamingOptions"),
                Tuple.Create<object, string>(options.StrategyOptions, "StrategyOptions"),
            };

            Write(targetFileName, objects);
        }

        public static void WriteTo(string targetFileName, IEnumerable<object> sources)
        {
            Write(targetFileName, sources.Select(x => Tuple.Create(x, x.GetType().Name)));
        }

        private static void Write(string targetFileName, IEnumerable<Tuple<object, string>> sources)
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
            }
        }
    }
}
