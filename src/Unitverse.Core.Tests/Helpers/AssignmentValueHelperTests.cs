namespace Unitverse.Core.Tests.Helpers
{
    using Unitverse.Core.Helpers;
    using System;
    using NUnit.Framework;
    using NSubstitute;
    using Microsoft.CodeAnalysis;
    using Unitverse.Core.Frameworks;
    using System.Collections.Generic;
    using Unitverse.Core.Options;
    using Microsoft.CodeAnalysis.Formatting;
    using System.Linq;
    using System.Text.RegularExpressions;

    [TestFixture]
    public static class AssignmentValueHelperTests
    {
        private static IEnumerable<object[]> TestCases
        {
            get
            {
                yield return new object[] { typeof(Func<>), new[] { typeof(string) }, "() => \"TestValue[0-9]+\"" };
                yield return new object[] { typeof(Func<,>), new[] { typeof(string), typeof(string) }, "x => \"TestValue[0-9]+\"" };
                yield return new object[] { typeof(Func<,,>), new[] { typeof(string), typeof(string), typeof(string) }, "\\(x, y\\) => \"TestValue[0-9]+\"" };
                yield return new object[] { typeof(Func<,,,>), new[] { typeof(string), typeof(string), typeof(string), typeof(string) }, "\\(x, y, z\\) => \"TestValue[0-9]+\"" };
                yield return new object[] { typeof(Func<,,,,>), new[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) }, "\\(a, b, c, d\\) => \"TestValue[0-9]+\"" };
                yield return new object[] { typeof(Func<,,,,,>), new[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) }, "\\(a, b, c, d, e\\) => \"TestValue[0-9]+\"" };
                yield return new object[] { typeof(Action), new Type[] { }, "() => \\{ \\}" };
                yield return new object[] { typeof(Action<>), new[] { typeof(string) }, "x => \\{ \\}" };
                yield return new object[] { typeof(Action<,>), new[] { typeof(string), typeof(string) }, "\\(x, y\\) => \\{ \\}" };
                yield return new object[] { typeof(Action<,,>), new[] { typeof(string), typeof(string), typeof(string) }, "\\(x, y, z\\) => \\{ \\}" };
                yield return new object[] { typeof(Action<,,,>), new[] { typeof(string), typeof(string), typeof(string), typeof(string) }, "\\(a, b, c, d\\) => \\{ \\}" };
                yield return new object[] { typeof(Action<,,,,>), new[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) }, "\\(a, b, c, d, e\\) => \\{ \\}" };
            }
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public static void CanCallGetDefaultAssignmentValue(Type type, Type[] typeArguments, string expectedOutputMatch)
        {
            var model = ClassModelProvider.Instance.SemanticModel;

            var typeSymbol = model.Compilation.GetTypeByMetadataName(type.FullName);
            if (typeArguments.Length > 0)
            {
                typeSymbol = typeSymbol.Construct(typeArguments.Select(x => model.Compilation.GetTypeByMetadataName(x.FullName)).ToArray());
            }

            var generationOptions = Substitute.For<IGenerationOptions>();
            generationOptions.FrameworkType.Returns(TestFrameworkTypes.NUnit3);
            generationOptions.MockingFrameworkType.Returns(MockingFrameworkType.NSubstitute);
            generationOptions.TestTypeNaming.Returns("{0}Tests");
            var options = new UnitTestGeneratorOptions(generationOptions, Substitute.For<INamingOptions>(), new DefaultStrategyOptions(), false, new Dictionary<string, string>());
            var frameworkSet = FrameworkSetFactory.Create(options);

            var visitedTypes = new HashSet<string>();
            var result = AssignmentValueHelper.GetDefaultAssignmentValue(typeSymbol, model, visitedTypes, frameworkSet);

            using (var workspace = new AdhocWorkspace())
            {
                var expressionText = Formatter.Format(result, workspace).ToString();
                Assert.That(Regex.IsMatch(expressionText, expectedOutputMatch), "Expected '" + expressionText + "' to match regex '" + expectedOutputMatch + "'.");
            }
        }
    }
}