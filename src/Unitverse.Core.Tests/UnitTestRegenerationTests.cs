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
    using System.Text;
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
    using Unitverse.Tests.Common;
    using Xunit;
    using Assert = NUnit.Framework.Assert;
    using Expression = System.Linq.Expressions.Expression;

    [TestFixture]
    public class UnitTestRegenerationTests
    {
        // ReSharper disable once MemberCanBePrivate.Global - is the test case source
        public static IEnumerable<object[]> TestClassResourceNames
        {
            // ReSharper disable once UnusedMember.Global - is the test case source
            get
            {
                var set = RegenerationTestClasses.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, true);
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
#if VS2019
                            if (resourceName.Contains("FileScoped", StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }
#endif
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
            // Get the source and split it
            var source = RegenerationTestClasses.ResourceManager.GetString(resourceName, TestClasses.Culture);
            StringBuilder first = new StringBuilder(), second = new StringBuilder();
            var current = first;

            foreach (var line in source.Lines())
            {
                if (line.Trim().All(x => x == '-') && line.Trim().Length > 0)
                {
                    current = second;
                }
                else
                {
                    current.AppendLine(line);
                }
            }

            var classAsText = first.ToString();
            var updatedClassAsText = second.ToString();

            // Extract the options from the first part
            var options = UnitTestGeneratorTests.ExtractOptions(testFrameworkTypes, mockingFrameworkType, useFluentAssertions, false, false, classAsText, true);

            // Compile the first
            UnitTestGeneratorTests.Compile(testFrameworkTypes, mockingFrameworkType, useFluentAssertions, options.GenerationOptions.UseAutoFixture, options.GenerationOptions.UseAutoFixtureForMocking, classAsText, out var tree, out var secondTree, out var references, out var externalInitTree, out var semanticModel);
            var generationItem = new TestGenerationItem(null, options, x => "Tests");
            var core = await CoreGenerator.Generate(generationItem, semanticModel, null, null, false, true, Substitute.For<IMessageLogger>()).ConfigureAwait(true);

            Assert.IsNotNull(core);
            Assert.That(!string.IsNullOrWhiteSpace(core.FileContent));
            Console.WriteLine(core.FileContent.Lines().Select((x, i) => ((i + 1).ToString("D4")) + ": " + x).Aggregate((x, y) => x + Environment.NewLine + y));

            // Check the first generated tree
            var generatedTree = CSharpSyntaxTree.ParseText(core.FileContent, new CSharpParseOptions(LanguageVersion.Latest));
            var syntaxTrees = new List<SyntaxTree> { tree, externalInitTree, generatedTree };
            if (secondTree != null)
            {
                syntaxTrees.Add(secondTree);
            }

            var propertyTesterEmitted = false;
            if (core.RequiredAssets.Any(x => x == TargetAsset.PropertyTester))
            {
                var testerAsset = AssetFactory.Create(TargetAsset.PropertyTester);
                var propertyTester = testerAsset.Content("Tests", testFrameworkTypes);
                syntaxTrees.Add(CSharpSyntaxTree.ParseText(propertyTester, new CSharpParseOptions(LanguageVersion.Latest)));
                propertyTesterEmitted = true;
            }
            var targetCompilation = CSharpCompilation.Create(
                "MyTest",
                syntaxTrees: syntaxTrees,
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            // Compile the second, using the output from the first compile
            UnitTestGeneratorTests.Compile(testFrameworkTypes, mockingFrameworkType, useFluentAssertions, false, false, updatedClassAsText, out var updatedTree, out _, out _, out _, out var updatedModel);

            var core2 = await CoreGenerator.Generate(generationItem, updatedModel, targetCompilation.GetSemanticModel(generatedTree), null, false, true, Substitute.For<IMessageLogger>()).ConfigureAwait(true);

            // Check the second generated tree
            Assert.IsNotNull(core2);
            Assert.That(!string.IsNullOrWhiteSpace(core2.FileContent));
            Console.WriteLine(core2.FileContent.Lines().Select((x, i) => ((i + 1).ToString("D4")) + ": " + x).Aggregate((x, y) => x + Environment.NewLine + y));

            var generatedTree2 = CSharpSyntaxTree.ParseText(core2.FileContent, new CSharpParseOptions(LanguageVersion.Latest));

            if (core2.RequiredAssets.Any(x => x == TargetAsset.PropertyTester) && !propertyTesterEmitted)
            {
                var testerAsset = AssetFactory.Create(TargetAsset.PropertyTester);
                var propertyTester = testerAsset.Content("Tests", testFrameworkTypes);
                syntaxTrees.Add(CSharpSyntaxTree.ParseText(propertyTester, new CSharpParseOptions(LanguageVersion.Latest)));
            }

            var validateCompilation2 = CSharpCompilation.Create(
                "MyTest",
                syntaxTrees: syntaxTrees,
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            // emit the final output and check it
            var stream = new MemoryStream();
            var result = validateCompilation2.Emit(stream);
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
    }
}