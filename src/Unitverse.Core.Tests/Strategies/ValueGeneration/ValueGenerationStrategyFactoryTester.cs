namespace Unitverse.Core.Tests.Strategies.ValueGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Formatting;
    using Microsoft.CodeAnalysis.Scripting;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Frameworks;
    using Unitverse.Core.Options;
    using Unitverse.Core.Strategies.ValueGeneration;

    [TestFixture]
    public static class ValueGenerationStrategyFactoryTester
    {
        [Test]
        [TestCase("System.String")]
        [TestCase("System.Int16")]
        [TestCase("System.Int32")]
        [TestCase("System.Int64")]
        [TestCase("System.Decimal")]
        [TestCase("System.UInt16")]
        [TestCase("System.UInt32")]
        [TestCase("System.UInt64")]
        [TestCase("System.Byte")]
        [TestCase("System.SByte")]
        [TestCase("System.Double")]
        [TestCase("System.Single")]
        [TestCase("System.Boolean")]
        [TestCase("System.Guid")]
        [TestCase("System.DateTime")]
        [TestCase("System.DateTimeOffset")]
        [TestCase("System.TimeSpan")]
        [TestCase("System.Globalization.CultureInfo")]
        [TestCase("System.Byte[]")]
        [TestCase("System.IO.Stream")]
        [TestCase("System.Threading.CancellationToken")]
        [TestCase("System.Drawing.Brush, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        [TestCase("System.Drawing.Color, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        [TestCase("System.Int32[]")] // System.Array
        [TestCase("Microsoft.Win32.RegistryValueKind")] // System.Enum
        public static async Task CanGenerate(string typeName)
        {
            var model = ClassModelProvider.Instance.SemanticModel;

            ITypeSymbol info;
#pragma warning disable CA1062 // it's a test case parameter
            if (typeName.EndsWith("[]", StringComparison.Ordinal))
            {
                info = model.Compilation.CreateArrayTypeSymbol(model.Compilation.GetTypeByMetadataName(typeName.Substring(0, typeName.Length - 2)));
            }
            else
            {
                var metadataName = typeName.IndexOf(',') > 0 ? typeName.Substring(0, typeName.IndexOf(',')) : typeName;
                info = model.Compilation.GetTypeByMetadataName(metadataName);
            }

            var generationOptions = Substitute.For<IGenerationOptions>();
            generationOptions.FrameworkType.Returns(TestFrameworkTypes.NUnit3);
            generationOptions.MockingFrameworkType.Returns(MockingFrameworkType.NSubstitute);
            generationOptions.TestTypeNaming.Returns("{0}Tests");
            var options = new UnitTestGeneratorOptions(generationOptions, Substitute.For<INamingOptions>(), new DefaultStrategyOptions(), false, new Dictionary<string, string>());
            var frameworkSet = FrameworkSetFactory.Create(options);
            var expression = ValueGenerationStrategyFactory.GenerateFor(info, model, new HashSet<string>(StringComparer.OrdinalIgnoreCase),  frameworkSet);

            string expressionText = string.Empty;
            try
            {
                using (var workspace = new AdhocWorkspace())
                {
                    expressionText = Formatter.Format(expression, workspace).ToString();
                    Console.WriteLine(expressionText);
                    var scriptOptions = ScriptOptions.Default
                        .AddReferences(typeof(System.Drawing.Brushes).Assembly)
                        .AddImports("System")
                        .AddImports("System.IO")
                        .AddImports("System.Drawing")
                        .AddImports("Microsoft.Win32")
                        .AddImports("System.Globalization")
                        .AddImports("System.Threading")
                        .AddImports("System.Collections");
                    var result = await CSharpScript.EvaluateAsync<object>(expressionText, scriptOptions).ConfigureAwait(false);

                    var expectedType = Type.GetType(typeName);
                    Assert.That(result, Is.InstanceOf(expectedType));
                }
            }
            catch (CompilationErrorException e)
            {
                Assert.Fail("Expression compilation failed - " + expressionText + ": " + e.Message);
            }
        }
    }
}