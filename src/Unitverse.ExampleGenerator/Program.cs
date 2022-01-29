using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Unitverse.Core.Options;
using Unitverse.Core.Helpers;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Threading.Tasks;
using Unitverse.Core;
using FluentAssertions;
using System.Xml.Linq;
using System.Xml;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Markup;
using NSubstitute;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using FakeItEasy;
using Xunit;
using NUnit.Framework;

namespace Unitverse.ExampleGenerator
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                var currentPath = new FileInfo(typeof(Program).Assembly.Location).Directory;
                while (!string.Equals(currentPath.Name, "src", StringComparison.OrdinalIgnoreCase))
                {
                    currentPath = currentPath.Parent;
                }
                currentPath = currentPath.Parent.GetDirectories().First(x => string.Equals(x.Name, "docs", StringComparison.OrdinalIgnoreCase));

                var set = Examples.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, true);
                var entryKeys = new List<Tuple<string, string>>();
                foreach (DictionaryEntry entry in set)
                {
                    var exampleName = entry.Key.ToString();
                    var classAsText = Examples.ResourceManager.GetString(exampleName, Examples.Culture);
                    var description = GetDescription(classAsText);

                    await WriteExample(currentPath, exampleName, description, classAsText);

                    entryKeys.Add(Tuple.Create(entry.Key.ToString(), description));
                }

                var file = new FileInfo(Path.Combine(currentPath.FullName, "examples.md"));
                using (var writer = new StreamWriter(file.FullName, false, Encoding.UTF8))
                {
                    writer.WriteLine("# Examples");
                    writer.WriteLine("This section contains examples of the output that Unitverse outputs, refreshed every build. Each example aims to demonstrate a particular scenario which is described in the following table.");
                    writer.WriteLine();
                    writer.WriteLine("| Example | Description |");
                    writer.WriteLine("| --- | --- |");
                    foreach (var pair in entryKeys)
                    {
                        writer.WriteLine("| [" + pair.Item1 + "](examples/" + pair.Item1 + ".md) | " + pair.Item2 + " |");
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return -1;
            }
        }

        private static string GetDescription(string classAsText)
        {
            var description = classAsText.Lines().FirstOrDefault(x => x.StartsWith("// $"));
            if (description == null)
            {
                return "No description available.";
            }

            return description.Substring("// $".Length).Trim();
        }

        static async Task WriteExample(DirectoryInfo docsFolder, string exampleName, string description, string classAsText)
        {
            var generationOptions = new MutableGenerationOptions(new DefaultGenerationOptions());
            var namingOptions = new MutableNamingOptions(new DefaultNamingOptions());

            var options = new UnitTestGeneratorOptions(generationOptions, namingOptions, false);

            var lines = classAsText.Lines().Where(x => x.StartsWith("// #", StringComparison.Ordinal)).Select(x => x.Substring(4).Trim()).ToList();
            if (lines.Any())
            {
                var properties = new Dictionary<string, string>();
                foreach (var line in lines)
                {
                    var pair = line.Split('=');
                    if (pair.Count() == 2)
                    {
                        properties[pair[0].Trim()] = pair[1].Trim();
                    }
                }

                properties.ApplyTo(generationOptions);
                properties.ApplyTo(namingOptions);
            }

            var tree = CSharpSyntaxTree.ParseText(classAsText, new CSharpParseOptions(LanguageVersion.Latest));

            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Linq.Expressions.Expression).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Brush).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Stream).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(SqlConnection).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Window).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(UIElement).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DependencyObject).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ValueTask<>).Assembly.Location),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Core.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Threading.Tasks.dll")),
            };

            references.AddRange(GetReferences(options.GenerationOptions.FrameworkType));
            references.AddRange(GetReferences(options.GenerationOptions.MockingFrameworkType));

            if (options.GenerationOptions.UseFluentAssertions)
            {
                references.Add(MetadataReference.CreateFromFile(typeof(FluentActions).Assembly.Location));
                references.Add(MetadataReference.CreateFromFile(typeof(XDocument).Assembly.Location));
                references.Add(MetadataReference.CreateFromFile(typeof(XmlNode).Assembly.Location));
                references.Add(MetadataReference.CreateFromFile(typeof(IQueryAmbient).Assembly.Location));
            }

            var compilation = CSharpCompilation.Create(
                "MyTest",
                syntaxTrees: new[] { tree },
                references: references);

            var semanticModel = compilation.GetSemanticModel(tree);


            var core = await CoreGenerator.Generate(semanticModel, null, null, false, options, x => "Tests", true, Substitute.For<IMessageLogger>()).ConfigureAwait(true);

            var generatedTree = CSharpSyntaxTree.ParseText(core.FileContent, new CSharpParseOptions(LanguageVersion.Latest));

            var sourceContent = GetClassFrom(tree, false);
            var targetContent = GetClassFrom(generatedTree, true);

            var file = new FileInfo(Path.Combine(docsFolder.FullName, "examples", exampleName + ".md"));
            using (var writer = new StreamWriter(file.FullName, false, Encoding.UTF8))
            {
                writer.WriteLine("## " + exampleName);
                writer.WriteLine(description);
                writer.WriteLine();
                writer.WriteLine("### Source Type(s)");
                writer.WriteLine("``` csharp");
                writer.WriteLine(sourceContent);
                writer.WriteLine("```");
                writer.WriteLine();
                writer.WriteLine("### Generated Tests");
                writer.WriteLine("``` csharp");
                writer.WriteLine(targetContent);
                writer.WriteLine("```");
            }
        }

        private static string TrimBlankLines(string input)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder temp = new StringBuilder();
            var anyEmitted = false;
            foreach (var str in input.Lines())
            {
                if (string.IsNullOrWhiteSpace(str))
                {
                    temp.AppendLine(str);
                }
                else
                {
                    if (temp.Length > 0)
                    {
                        if (anyEmitted)
                        {
                            sb.Append(temp);
                        }
                        temp.Length = 0;
                    }
                    anyEmitted = true;
                    sb.AppendLine(str);
                }
            }
            return sb.ToString();
        }

        private static string GetClassFrom(SyntaxTree tree, bool lastOnly)
        {
            StringBuilder sb = new StringBuilder();

            var types = tree.GetRoot().DescendantNodesAndSelf().OfType<TypeDeclarationSyntax>();
            if (lastOnly)
            {
                types = new[] { types.Last() };
            }

            foreach (var type in types)
            {
                using (var workspace = new AdhocWorkspace())
                {
                    var compilation = Formatter.Format(type, workspace);

                    var trimmed = TrimBlankLines(compilation.ToFullString());
                    
                    if (sb.Length > 0)
                    {
                        sb.AppendLine();
                    }
                    sb.Append(trimmed);
                }
            }

            return sb.ToString();
        }

        private static IEnumerable<PortableExecutableReference> GetReferences(MockingFrameworkType mockingFrameworkType)
        {
            switch (mockingFrameworkType)
            {
                case MockingFrameworkType.NSubstitute:
                    yield return MetadataReference.CreateFromFile(typeof(Substitute).Assembly.Location);
                    break;

                case MockingFrameworkType.Moq:
                    yield return MetadataReference.CreateFromFile(typeof(Mock).Assembly.Location);
                    break;

                case MockingFrameworkType.FakeItEasy:
                    yield return MetadataReference.CreateFromFile(typeof(A).Assembly.Location);
                    break;
            }
        }

        private static IEnumerable<PortableExecutableReference> GetReferences(TestFrameworkTypes testFrameworkTypes)
        {
            if ((testFrameworkTypes & TestFrameworkTypes.XUnit) > 0)
            {
                yield return MetadataReference.CreateFromFile(typeof(FactAttribute).Assembly.Location);
                yield return MetadataReference.CreateFromFile(typeof(Xunit.Assert).Assembly.Location);
            }

            if ((testFrameworkTypes & (TestFrameworkTypes.NUnit3 | TestFrameworkTypes.NUnit2)) > 0)
            {
                yield return MetadataReference.CreateFromFile(typeof(TestFixtureAttribute).Assembly.Location);
            }

            if ((testFrameworkTypes & TestFrameworkTypes.MsTest) > 0)
            {
                yield return MetadataReference.CreateFromFile(typeof(Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute).Assembly.Location);
            }
        }
    }
}
