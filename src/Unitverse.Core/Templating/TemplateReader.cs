namespace Unitverse.Core.Templating
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using SequelFilter;

    public static class TemplateReader
    {
        public static ITemplate ReadFrom(string fileName)
        {
            var lines = File.ReadAllLines(fileName);
            ParseContent(lines, out var headers, out var content);

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidOperationException($"While reading '{fileName}': No test body found");
            }

            string? testMethodName = null, target = null, description = null;
            int priority = 10;
            bool stopMatching = false, isExclusive = false, isAsync = false, isStatic = false;
            var includes = new List<string>();
            var excludes = new List<string>();

            foreach (var header in headers)
            {
                if (string.Equals(header.name, TemplateHeaders.TestMethodName, StringComparison.OrdinalIgnoreCase))
                {
                    testMethodName = header.value;
                }
                else if (string.Equals(header.name, TemplateHeaders.Target, StringComparison.OrdinalIgnoreCase))
                {
                    target = header.value;
                }
                else if (string.Equals(header.name, TemplateHeaders.Description, StringComparison.OrdinalIgnoreCase))
                {
                    description = header.value;
                }
                else if (string.Equals(header.name, TemplateHeaders.Include, StringComparison.OrdinalIgnoreCase))
                {
                    includes.Add(header.value);
                }
                else if (string.Equals(header.name, TemplateHeaders.Exclude, StringComparison.OrdinalIgnoreCase))
                {
                    excludes.Add(header.value);
                }
                else if (string.Equals(header.name, TemplateHeaders.Priority, StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(header.value, out var parsedValue))
                    {
                        priority = parsedValue;
                    }
                    else
                    {
                        throw new InvalidOperationException($"While reading '{fileName}': Could not parse {TemplateHeaders.Priority} value ('{header.value}') as an integer");
                    }
                }
                else if (string.Equals(header.name, TemplateHeaders.StopMatching, StringComparison.OrdinalIgnoreCase))
                {
                    stopMatching = IsTrue(header.value);
                }
                else if (string.Equals(header.name, TemplateHeaders.IsExclusive, StringComparison.OrdinalIgnoreCase))
                {
                    isExclusive = IsTrue(header.value);
                }
                else if (string.Equals(header.name, TemplateHeaders.IsAsync, StringComparison.OrdinalIgnoreCase))
                {
                    isAsync = IsTrue(header.value);
                }
                else if (string.Equals(header.name, TemplateHeaders.IsStatic, StringComparison.OrdinalIgnoreCase))
                {
                    isStatic = IsTrue(header.value);
                }
            }

            if (string.IsNullOrWhiteSpace(testMethodName))
            {
                throw new InvalidOperationException($"While reading '{fileName}': No '{TemplateHeaders.TestMethodName}' header found");
            }

            if (string.IsNullOrWhiteSpace(target))
            {
                throw new InvalidOperationException($"While reading '{fileName}': No '{TemplateHeaders.Target}' header found");
            }

            if (!includes.Any())
            {
                throw new InvalidOperationException($"While reading '{fileName}': No {TemplateHeaders.Include} values specified");
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
                    throw new InvalidOperationException($"While reading '{fileName}': Could not parse {TemplateHeaders.Include} value - {ex.Message}");
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
                    throw new InvalidOperationException($"While reading '{fileName}': Could not parse {TemplateHeaders.Exclude} value - {ex.Message}");
                }
            }

#pragma warning disable CS8604 // Possible null reference argument - content, testMethodName and target checked by string.IsNullOrWhiteSpace above
            return new Template(content, testMethodName, target, compiledIncludes, compiledExcludes, isExclusive, stopMatching, priority, isAsync, isStatic, description ?? string.Empty);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        private static bool IsTrue(string value)
        {
            return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "1", StringComparison.OrdinalIgnoreCase);
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
}
