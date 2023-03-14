namespace Unitverse.Core.Templating
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using SequelFilter;

    public static class TemplateReader
    {
        private const string TestMethodName = "TestMethodName";
        private const string Target = "Target";
        private const string Include = "Include";
        private const string Exclude = "Exclude";
        private const string IsAsync = "IsAsync";
        private const string IsStatic = "IsStatic";
        private const string Description = "Description";

        private const string IsExclusive = "IsExclusive"; // can only be matched if no other templates have been matched for the current item
        private const string StopMatching = "StopMatching"; // should stop looking for templates that apply to the current item after this is matched
        private const string Priority = "Priority"; // numeric priority - 1 comes first

        public static ITemplate ReadFrom(string fileName)
        {
            return ReadFrom(File.ReadAllLines(fileName), fileName);
        }

        public static ITemplate ReadFrom(string[] lines, string fileName)
        {
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
                if (string.Equals(header.name, TestMethodName, StringComparison.OrdinalIgnoreCase))
                {
                    testMethodName = header.value;
                }
                else if (string.Equals(header.name, Target, StringComparison.OrdinalIgnoreCase))
                {
                    target = header.value;
                }
                else if (string.Equals(header.name, Description, StringComparison.OrdinalIgnoreCase))
                {
                    description = header.value;
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
                    stopMatching = IsTrue(header.value);
                }
                else if (string.Equals(header.name, IsExclusive, StringComparison.OrdinalIgnoreCase))
                {
                    isExclusive = IsTrue(header.value);
                }
                else if (string.Equals(header.name, IsAsync, StringComparison.OrdinalIgnoreCase))
                {
                    isAsync = IsTrue(header.value);
                }
                else if (string.Equals(header.name, IsStatic, StringComparison.OrdinalIgnoreCase))
                {
                    isStatic = IsTrue(header.value);
                }
            }

            if (string.IsNullOrWhiteSpace(testMethodName))
            {
                throw new InvalidOperationException($"While reading '{fileName}': No '{TestMethodName}' header found");
            }

            if (string.IsNullOrWhiteSpace(target))
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
