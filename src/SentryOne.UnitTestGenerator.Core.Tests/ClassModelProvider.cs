namespace SentryOne.UnitTestGenerator.Core.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Models;

    internal static class ClassModelProvider
    {
        private static readonly object Lock = new object();

        private static ClassModel _instance;
        private static ClassModel _genericInstance;

        public static ClassModel Instance
        {
            get
            {
                lock (Lock)
                {
                    return _instance ?? (_instance = CreateModel());
                }
            }
        }

        public static ClassModel GenericInstance
        {
            get
            {
                lock (Lock)
                {
                    return _genericInstance ?? (_genericInstance = CreateGenericModel());
                }
            }
        }

        private static ClassModel CreateModel()
        {
            return CreateModel(TestClasses.PocoInitialization);
        }

        private static ClassModel CreateGenericModel()
        {
            return CreateModel(TestClasses.TypeGenericDisambiguation);
        }

        private static ClassModel CreateModel(string classAsText)
        {
            var tree = CSharpSyntaxTree.ParseText(classAsText, new CSharpParseOptions(LanguageVersion.Latest));

            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Stream).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Drawing.Brushes).Assembly.Location),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Core.dll")),
            };

            var compilation = CSharpCompilation.Create(
                "MyTest",
                syntaxTrees: new[] { tree },
                references: references);

            var semanticModel = compilation.GetSemanticModel(tree);

            var model = new TestableItemExtractor(semanticModel.SyntaxTree, semanticModel);
            return model.Extract(null).First();
        }

        public static void Consume<T>(this IEnumerable<T> enumerable)
        {
            enumerable.ToList().ForEach(_ => { });
        }
    }
}
