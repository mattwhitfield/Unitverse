namespace SentryOne.UnitTestGenerator.Specs.Strategies
{
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Helpers;
    using SentryOne.UnitTestGenerator.Core.Options;
    using TechTalk.SpecFlow;

    [Binding]
    public class BaseSteps
    {
        private readonly BaseContext _context;
        
        public BaseSteps(BaseContext context)
        {
            _context = context;
        }

        [Given(@"I have a class defined as")]
        public void GivenIHaveAClassThatImplements(string classAsText)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(classAsText);
            var compilation = CSharpCompilation.Create("MyTest", new[] { syntaxTree }, SemanticModelHelper.References.Value);
            var model = compilation.GetSemanticModel(syntaxTree);
            _context.SemanticModel = model;

            var extractor = new TestableItemExtractor(syntaxTree, model);
            _context.ClassModel = extractor.Extract(syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().First()).First();
        }

        [Given(@"I set my test framework to '(.*)'")]
        public void GivenISetMyGenerationFrameworkTo(TestFrameworkTypes frameworkType)
        {
            _context.TargetFramework = frameworkType;
        }

        [Given(@"I set my mock framework to '(.*)'")]
        public void GivenISetMyGenerationMockFrameworkTo(MockingFrameworkType mockType)
        {
            _context.MockFramework = mockType;
        }
    }
}