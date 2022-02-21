namespace Unitverse.Core.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.SqlClient;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Markup;
    using System.Xml;
    using System.Xml.Linq;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Moq;
    using Moq.AutoMock;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Assets;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;
    using Xunit;
    using Assert = NUnit.Framework.Assert;
    using Expression = System.Linq.Expressions.Expression;

    [TestFixture]
    public class UnitTestGeneratorTests
    {
        // ReSharper disable once MemberCanBePrivate.Global - is the test case source
        public static IEnumerable<object[]> TestClassResourceNames
        {
            // ReSharper disable once UnusedMember.Global - is the test case source
            get
            {
                var set = TestClasses.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, true);
                var entryKeys = new List<string>();
                foreach (DictionaryEntry entry in set)
                {
                    entryKeys.Add(entry.Key.ToString());
                }

                var frameworks = new[] { TestFrameworkTypes.MsTest, TestFrameworkTypes.NUnit3, TestFrameworkTypes.XUnit };
                var mocks = new[] { MockingFrameworkType.Moq, MockingFrameworkType.NSubstitute, MockingFrameworkType.FakeItEasy, MockingFrameworkType.MoqAutoMock };

                foreach (var framework in frameworks)
                {
                    foreach (var mock in mocks)
                    {
                        foreach (var resourceName in entryKeys)
                        {
                            yield return new object[] { resourceName, framework, mock, true };
                            yield return new object[] { resourceName, framework, mock, false };
                        }
                    }
                }
            }
        }

        [TestCaseSource(nameof(TestClassResourceNames))]
        public static async Task AssertTestGeneration(string resourceName, TestFrameworkTypes testFrameworkTypes, MockingFrameworkType mockingFrameworkType, bool useFluentAssertions)
        {
            var classAsText = TestClasses.ResourceManager.GetString(resourceName, TestClasses.Culture);

            var generationOptions = new MutableGenerationOptions(new DefaultGenerationOptions());
            var namingOptions = new MutableNamingOptions(new DefaultNamingOptions());
            var strategyOptions = new MutableStrategyOptions(new DefaultStrategyOptions());

            generationOptions.FrameworkType = testFrameworkTypes;
            generationOptions.MockingFrameworkType = mockingFrameworkType;
            generationOptions.UseFluentAssertions = useFluentAssertions;

            var options = new UnitTestGeneratorOptions(generationOptions, namingOptions, strategyOptions, false);

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
                properties.ApplyTo(strategyOptions);
            }

            var tree = CSharpSyntaxTree.ParseText(classAsText, new CSharpParseOptions(LanguageVersion.Latest));

            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(INotifyPropertyChanged).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Expression).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Brush).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Stream).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Form).Assembly.Location),
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

            references.AddRange(GetReferences(mockingFrameworkType));
            references.AddRange(GetReferences(testFrameworkTypes));

            if (useFluentAssertions)
            {
                references.Add(MetadataReference.CreateFromFile(typeof(FluentActions).Assembly.Location));
                references.Add(MetadataReference.CreateFromFile(typeof(XDocument).Assembly.Location));
                references.Add(MetadataReference.CreateFromFile(typeof(XmlNode).Assembly.Location));
                references.Add(MetadataReference.CreateFromFile(typeof(IQueryAmbient).Assembly.Location));
            }

            var externalInitTree = CSharpSyntaxTree.ParseText("namespace System.Runtime.CompilerServices { internal static class IsExternalInit { } }", new CSharpParseOptions(LanguageVersion.Latest));

            var compilation = CSharpCompilation.Create(
                "MyTest",
                syntaxTrees: new[] { tree, externalInitTree },
                references: references);

            var semanticModel = compilation.GetSemanticModel(tree);

            
            var core = await CoreGenerator.Generate(semanticModel, null, null, false, options, x => "Tests", true, Substitute.For<IMessageLogger>()).ConfigureAwait(true);

            Assert.IsNotNull(core);
            Assert.That(!string.IsNullOrWhiteSpace(core.FileContent));
            Console.WriteLine(core.FileContent.Lines().Select((x, i) => ((i + 1).ToString("D4")) + ": " + x).Aggregate((x, y) => x + Environment.NewLine + y));

            var generatedTree = CSharpSyntaxTree.ParseText(core.FileContent, new CSharpParseOptions(LanguageVersion.Latest));

            var syntaxTrees = new List<SyntaxTree> { tree, externalInitTree, generatedTree };

            if (core.RequiredAssets.Any(x => x == TargetAsset.PropertyTester))
            {
                var testerAsset = AssetFactory.Create(TargetAsset.PropertyTester);
                var propertyTester = testerAsset.Content("Tests", testFrameworkTypes);
                syntaxTrees.Add(CSharpSyntaxTree.ParseText(propertyTester, new CSharpParseOptions(LanguageVersion.Latest)));
            }

            var validateCompilation = CSharpCompilation.Create(
                "MyTest",
                syntaxTrees: syntaxTrees,
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var stream = new MemoryStream();
            var result = validateCompilation.Emit(stream);
            var streamLength = stream.Length;
            stream.Dispose();

            if (!result.Success)
            {
                Assert.IsFalse(result.Diagnostics.Where(x => x.Location.SourceTree != generatedTree && x.Location.IsInSource && x.WarningLevel == 0).Any(), "Input has errors: " + string.Join(",", result.Diagnostics.Where(x => x.Location.SourceTree != generatedTree && x.Location.IsInSource && x.WarningLevel == 0).Select(x => x.Location.GetLineSpan().StartLinePosition.Line + ": " + x.GetMessage())));
            }

            Assert.IsTrue(result.Success, "Generated output has errors: " + string.Join(Environment.NewLine, result.Diagnostics.Where(x => x.Location.SourceTree == generatedTree).OrderBy(x => x.Location.GetLineSpan().StartLinePosition.Line).Select(x => x.Location.GetLineSpan().StartLinePosition.Line + ": " + x.GetMessage())));
            Assert.That(streamLength, Is.GreaterThan(0));
        }

        [Test]
        public void CanExtractSingleItemAsClassModel()
        {
            var tree = CSharpSyntaxTree.ParseText(TestClasses.SampleClassTestFile);

            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create(
                "MyTest",
                syntaxTrees: new[] { tree },
                references: new[] { mscorlib });

            var semanticModel = compilation.GetSemanticModel(tree);

            var root = tree.GetRoot();

            var methodSyntax = root.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault();

            var extractor = new TestableItemExtractor(tree, semanticModel);

            var classModels = extractor.Extract(methodSyntax, Substitute.For<IUnitTestGeneratorOptions>());
            var classModel = classModels.FirstOrDefault();

            Assert.That(classModel, Is.Not.Null);

            var methodModel = classModel.Methods.FirstOrDefault();

            Assert.That(methodModel, Is.Not.Null);

            Assert.That(methodModel.Name, Is.EqualTo("ThisIsAMethod"));
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

                case MockingFrameworkType.MoqAutoMock:
                    yield return MetadataReference.CreateFromFile(typeof(AutoMocker).Assembly.Location);
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