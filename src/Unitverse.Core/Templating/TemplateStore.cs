namespace Unitverse.Core.Templating
{
    using SequelFilter;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;
    using Unitverse.Core.Templating.Model;

    public static class TemplateStore
    {
        private static readonly Dictionary<string, ITemplate> _cache = new Dictionary<string, ITemplate>(StringComparer.OrdinalIgnoreCase);

        public static IList<ITemplate> LoadTemplatesFor(string folder, IMessageLogger messageLogger)
        {
            var output = new List<ITemplate>();
            var directory = new DirectoryInfo(folder);

            while (directory != null)
            {
                var templateFolders = directory.GetDirectories(".unitverseTemplates", SearchOption.TopDirectoryOnly);
                var templateFolder = templateFolders.FirstOrDefault();
                if (templateFolder != null)
                {
                    output.AddRange(ReadTemplates(templateFolder));
                }

                directory = directory.Parent;
            }

            return output;
        }

        private static string GetCacheKey(FileInfo fileInfo)
        {
            return fileInfo.FullName + "." + fileInfo.LastWriteTimeUtc.ToString("O");
        }

        private static IEnumerable<ITemplate> ReadTemplates(DirectoryInfo directoryInfo, IMessageLogger messageLogger)
        {
            foreach (var file in directoryInfo.GetFiles("*.template", SearchOption.TopDirectoryOnly))
            {
                var cacheKey = GetCacheKey(file);
                if (_cache.TryGetValue(cacheKey, out var template))
                {
                    yield return template;
                }
                else
                {
                    try
                    {
                        template = TemplateReader.ReadFrom(file.FullName);
                    }
                    catch (Exception ex)
                    {
                        messageLogger.LogMessage(ex.Message);
                    }

                    if (template != null)
                    {
                        _cache[cacheKey] = template;
                        yield return template;
                    }
                }
            }
        }
    }

    public static class TemplateReader
    {
        private const string TestMethodName = "TestMethodName";
        private const string Target = "Target";
        private const string Include = "Include";
        private const string Exclude = "Exclude";

        // TODO
        private const string IsExclusive = "IsExclusive"; // can only be matched if no other templates have been matched for the current item
        private const string StopMatching = "StopMatching"; // should stop looking for templates that apply to the current item after this is matched
        private const string Priority = "Priority"; // numeric priority - 1 comes first

        public static ITemplate ReadFrom(string fileName)
        {
            var lines = File.ReadAllLines(fileName);

            ParseContent(lines, out var headers, out var content);

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidOperationException($"While reading '{fileName}': No test body found");
            }

            string testMethodName = null, target = null;
            int priority = 10;
            bool stopMatching = false, isExclusive = false;
            var includes = new List<string>();
            var excludes = new List<string>();

            foreach (var header in headers)
            {
                if (string.Equals(header.name, TestMethodName, StringComparison.OrdinalIgnoreCase))
                {
                    testMethodName = header.value;
                }
                else if (string.Equals(header.name, Target, StringComparison.OrdinalIgnoreCase))
                {
                    target = header.value;
                }
                else if (string.Equals(header.name, Include, StringComparison.OrdinalIgnoreCase))
                {
                    includes.Add(header.value);
                }
                else if (string.Equals(header.name, Exclude, StringComparison.OrdinalIgnoreCase))
                {
                    excludes.Add(header.value);
                }
                else if (string.Equals(header.name, Priority, StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(header.value, out var parsedValue))
                    {
                        priority = parsedValue;
                    }
                    else
                    {
                        throw new InvalidOperationException($"While reading '{fileName}': Could not parse {Priority} value ('{header.value}') as an integer");
                    }
                }
                else if (string.Equals(header.name, StopMatching, StringComparison.OrdinalIgnoreCase))
                {
                    stopMatching = string.Equals(header.value, "true", StringComparison.OrdinalIgnoreCase);
                }
                else if (string.Equals(header.name, IsExclusive, StringComparison.OrdinalIgnoreCase))
                {
                    isExclusive = string.Equals(header.value, "true", StringComparison.OrdinalIgnoreCase);
                }
            }

            if (string.IsNullOrWhiteSpace(testMethodName))
            {
                throw new InvalidOperationException($"While reading '{fileName}': No '{TestMethodName}' header found");
            }

            if (string.IsNullOrWhiteSpace(testMethodName))
            {
                throw new InvalidOperationException($"While reading '{fileName}': No '{Target}' header found");
            }

            var compiledIncludes = new List<ExecutableExpression>();
            var compiledExcludes = new List<ExecutableExpression>();

            foreach (var include in includes)
            {
                try
                {
                    compiledIncludes.Add(SequelFilterParser.Parse(include));
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"While reading '{fileName}': Could not parse {Include} value - {ex.Message}");
                }
            }

            foreach (var exclude in excludes)
            {
                try
                {
                    compiledExcludes.Add(SequelFilterParser.Parse(exclude));
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"While reading '{fileName}': Could not parse {Exclude} value - {ex.Message}");
                }
            }


        }

        private static void ParseContent(string[] lines, out List<(string name, string value)> headers, out string content)
        {
            headers = new List<(string name, string value)>();
            var contentBuilder = new StringBuilder();

            var pastSplit = false;
            foreach (var line in lines)
            {
                var isBlank = string.IsNullOrWhiteSpace(line);
                pastSplit |= isBlank;
                if (pastSplit)
                {
                    if (isBlank && contentBuilder.Length == 0)
                    {
                        continue;
                    }

                    contentBuilder.AppendLine(line);
                }
                else if (!isBlank)
                {
                    var splitIndex = line.IndexOf(':');
                    if (splitIndex > 0 && splitIndex < line.Length - 1)
                    {
                        headers.Add((line.Substring(0, splitIndex).Trim(), line.Substring(splitIndex + 1).Trim()));
                    }
                }
            }

            content = contentBuilder.ToString();
        }
    }

    public interface ITemplate
    {
        bool AppliesTo(IConstructor testableModel);

        IEnumerable<SectionedMethodHandler> Create(IProperty property, ClassModel model, NamingContext namingContext);
    }
}
