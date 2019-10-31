namespace SentryOne.UnitTestGenerator.Specs.Strategies
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SentryOne.UnitTestGenerator.Core.Models;
    using SentryOne.UnitTestGenerator.Core.Options;

    public class BaseContext
    {
        public BaseContext()
        {
            TargetFramework = TestFrameworkTypes.NUnit3;
            MockFramework = MockingFrameworkType.NSubstitute;
        }

        public ClassModel ClassModel { get; set; }
        public SemanticModel SemanticModel { get; set; }
        public MethodDeclarationSyntax CurrentMethod { get; set; }
        public TestFrameworkTypes TargetFramework { get; set; }
        public MockingFrameworkType MockFramework { get; set; }
    }
}