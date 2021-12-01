using Unitverse.Core.Options;

namespace Unitverse.Core.Tests.Helpers
{
    public class DetectedGenerationOptions : IGenerationOptions
    {
        private readonly IGenerationOptions _baseOptions;
        private readonly bool? _usefluentAssertions;
        private readonly TestFrameworkTypes? _testFramework;
        private readonly MockingFrameworkType? _mockingFramework;

        public DetectedGenerationOptions(IGenerationOptions baseOptions, bool? usefluentAssertions, TestFrameworkTypes? testFramework, MockingFrameworkType? mockingFramework)
        {
            _baseOptions = baseOptions ?? throw new System.ArgumentNullException(nameof(baseOptions));
            _usefluentAssertions = usefluentAssertions;
            _testFramework = testFramework;
            _mockingFramework = mockingFramework;
        }

        public TestFrameworkTypes FrameworkType => _testFramework ?? _baseOptions.FrameworkType;

        public MockingFrameworkType MockingFrameworkType => _mockingFramework ?? _baseOptions.MockingFrameworkType;

        public bool UseFluentAssertions => _usefluentAssertions ?? _baseOptions.UseFluentAssertions;

        public bool AutoDetectFrameworkTypes => _baseOptions.AutoDetectFrameworkTypes;

        public bool AllowGenerationWithoutTargetProject => _baseOptions.AllowGenerationWithoutTargetProject;

        public string TestProjectNaming => _baseOptions.TestProjectNaming;

        public string TestFileNaming => _baseOptions.TestFileNaming;

        public string TestTypeNaming => _baseOptions.TestTypeNaming;
    }
}
