namespace Unitverse.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.SqlClient;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Forms;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class TestSemanticModelFactory
    {
        private static readonly Lazy<SemanticModel> LazyModel = new Lazy<SemanticModel>(CreateModel);

        private static readonly Lazy<SyntaxTree> LazyTree = new Lazy<SyntaxTree>(CreateTree);

        private static readonly Lazy<List<MetadataReference>> References = new Lazy<List<MetadataReference>>(CreateReferences);

        private static List<MetadataReference> CreateReferences()
        {
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            return new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(INotifyPropertyChanged).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Brush).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Stream).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Form).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(SqlConnection).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Window).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(UIElement).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DependencyObject).Assembly.Location),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Core.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Threading.Tasks.dll")),
            };
        }

        public static ClassDeclarationSyntax Class => GetNode<ClassDeclarationSyntax>();

        public static ConstructorDeclarationSyntax Constructor => GetNode<ConstructorDeclarationSyntax>();

        public static IndexerDeclarationSyntax Indexer => GetNode<IndexerDeclarationSyntax>();

        public static MethodDeclarationSyntax Method => GetNode<MethodDeclarationSyntax>();

        public static SemanticModel Model => LazyModel.Value;

        public static ParameterSyntax Parameter => GetNode<ParameterSyntax>();

        public static ParameterSyntax InterfaceParameter => GetNode<ParameterSyntax>(x => x.Identifier.Text == "interfaceParam");

        public static PropertyDeclarationSyntax Property => GetNode<PropertyDeclarationSyntax>();

        public static SyntaxTree Tree => LazyTree.Value;

        private static SemanticModel CreateModel()
        {
            var compilation = CSharpCompilation.Create("MyTest", new[] { Tree }, References.Value);

            return compilation.GetSemanticModel(Tree);
        }

        public static SemanticModel CreateSemanticModel(SyntaxTree tree)
        {
            var compilation = CSharpCompilation.Create("MyTest", new[] { tree }, References.Value);
            return compilation.GetSemanticModel(tree);
        }

        public static SyntaxTree CreateTree(string classAsText)
        {
            return CSharpSyntaxTree.ParseText(classAsText);
        }

        private static SyntaxTree CreateTree()
        {
            var classAsText = "namespace Test{ using System; using System.Threading.Tasks;  class ModelSource    {        public string Param { get; }        public ModelSource(string param)        {            Param = param;        }  public ModelSource(ICloneable interfaceParam) { Param = interfaceParam.ToString(); }        public int Method(string param)        {            return 1;        }   public async Task<int> AsyncMethod(string param)        {            return Task.FromResult(1);        }     public int this[int index]        {            get { return 123; }        }    }}";
            return CreateTree(classAsText);
        }

        private static T GetNode<T>()
        {
            return Tree.GetRoot().DescendantNodes().OfType<T>().First();
        }

        private static T GetNode<T>(Func<T, bool> filter)
        {
            return Tree.GetRoot().DescendantNodes().OfType<T>().First(filter);
        }
    }
}