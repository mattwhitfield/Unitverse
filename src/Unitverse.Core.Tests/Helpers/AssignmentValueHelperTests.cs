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
    using System.Threading.Tasks;

    [TestFixture]
    public static class AssignmentValueHelperTests
    {
        private static IEnumerable<object[]> TestCases
        {
            get
            {
                yield return new object[] { typeof(Func<Task>), "\\(\\) => Task.CompletedTask" };
                yield return new object[] { typeof(Func<string, Task>), "x => Task.CompletedTask" };
                yield return new object[] { typeof(Func<string, string, Task>), "\\(x, y\\) => Task.CompletedTask" };
                yield return new object[] { typeof(Func<string, string, string, Task>), "\\(x, y, z\\) => Task.CompletedTask" };
                yield return new object[] { typeof(Func<string, string, string, string, Task>), "\\(a, b, c, d\\) => Task.CompletedTask" };
                yield return new object[] { typeof(Func<string, string, string, string, string, Task>), "\\(a, b, c, d, e\\) => Task.CompletedTask" };
                yield return new object[] { typeof(Func<Task<int>>), "\\(\\) => Task.FromResult\\([0-9]+\\)" };
                yield return new object[] { typeof(Func<string, Task<int>>), "x => Task.FromResult\\([0-9]+\\)" };
                yield return new object[] { typeof(Func<string, string, Task<int>>), "\\(x, y\\) => Task.FromResult\\([0-9]+\\)" };
                yield return new object[] { typeof(Func<string, string, string, Task<int>>), "\\(x, y, z\\) => Task.FromResult\\([0-9]+\\)" };
                yield return new object[] { typeof(Func<string, string, string, string, Task<int>>), "\\(a, b, c, d\\) => Task.FromResult\\([0-9]+\\)" };
                yield return new object[] { typeof(Func<string, string, string, string, string, Task<int>>), "\\(a, b, c, d, e\\) => Task.FromResult\\([0-9]+\\)" };
                yield return new object[] { typeof(Func<string>), "\\(\\) => \"TestValue[0-9]+\"" };
                yield return new object[] { typeof(Func<string, string>), "x => \"TestValue[0-9]+\"" };
                yield return new object[] { typeof(Func<string, string, string>), "\\(x, y\\) => \"TestValue[0-9]+\"" };
                yield return new object[] { typeof(Func<string, string, string, string>), "\\(x, y, z\\) => \"TestValue[0-9]+\"" };
                yield return new object[] { typeof(Func<string, string, string, string, string>), "\\(a, b, c, d\\) => \"TestValue[0-9]+\"" };
                yield return new object[] { typeof(Func<string, string, string, string, string, string>), "\\(a, b, c, d, e\\) => \"TestValue[0-9]+\"" };
                yield return new object[] { typeof(Action), "\\(\\) => \\{ \\}" };
                yield return new object[] { typeof(Action<string>), "x => \\{ \\}" };
                yield return new object[] { typeof(Action<string, string>), "\\(x, y\\) => \\{ \\}" };
                yield return new object[] { typeof(Action<string, string, string>), "\\(x, y, z\\) => \\{ \\}" };
                yield return new object[] { typeof(Action<string, string, string, string>), "\\(a, b, c, d\\) => \\{ \\}" };
                yield return new object[] { typeof(Action<string, string, string, string, string>), "\\(a, b, c, d, e\\) => \\{ \\}" };
            }
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public static void CanCallGetDefaultAssignmentValue(Type type, string expectedOutputMatch)
        {
            var model = ClassModelProvider.Instance.SemanticModel;

            var typeSymbol = GetType(model, type);

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

        private static ITypeSymbol GetType(SemanticModel model, Type t)
        {
            var typeSymbol = model.Compilation.GetTypeByMetadataName(t.Namespace + "." + t.Name);
            if (t.GenericTypeArguments.Length > 0)
            {
                var typeArgs = t.GenericTypeArguments.Select(x => GetType(model, x)).ToArray();
                typeSymbol = typeSymbol.Construct(typeArgs);
            }
            return typeSymbol;
        }
    }
}