namespace SentryOne.UnitTestGenerator.Specs
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
        public bool CreateProjectAutomatically { get; } = true;
        public bool AddReferencesAutomatically { get; } = true;
        public bool AllowGenerationWithoutTargetProject { get; } = true;
        public string TestProjectNaming { get; } = "{0}.Tests";
        public string TestFileNaming { get; } = "{0}Tests";
        public string TestTypeNaming { get; } = "{0}Tests";
    }
}