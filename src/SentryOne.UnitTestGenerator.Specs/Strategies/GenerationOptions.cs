namespace SentryOne.UnitTestGenerator.Specs.Strategies
{
    using SentryOne.UnitTestGenerator.Core.Options;

    public class GenerationOptions : IGenerationOptions
    {
        public GenerationOptions(TestFrameworkTypes testFramework, MockingFrameworkType mockFramework)
        {
            FrameworkType = testFramework;
            MockingFrameworkType = mockFramework;
        }

        public TestFrameworkTypes FrameworkType { get; }
        public MockingFrameworkType MockingFrameworkType { get; }
        public bool CreateProjectAutomatically { get; set; } = true;
        public bool AddReferencesAutomatically { get; set; } = true;
        public string TestProjectNaming { get; set; } = "{0}.Tests";
        public string TestFileNaming { get; set; } = "{0}Tests";
        public string TestTypeNaming { get; set; } = "{0}Tests";
    }
}