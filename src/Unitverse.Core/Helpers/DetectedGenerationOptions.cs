namespace Unitverse.Core.Helpers
{
    using Unitverse.Core.Options;

    public class DetectedGenerationOptions : IGenerationOptions
    {
        private readonly IGenerationOptions _baseOptions;
        private readonly bool? _usefluentAssertions;
        private readonly bool? _useAutoFixture;
        private readonly bool? _useAutoFixtureForMocking;
        private readonly TestFrameworkTypes? _testFramework;
        private readonly MockingFrameworkType? _mockingFramework;

        public DetectedGenerationOptions(IGenerationOptions baseOptions, bool? usefluentAssertions, bool? useAutoFixture, bool? useAutoFixtureForMocking, TestFrameworkTypes? testFramework, MockingFrameworkType? mockingFramework)
        {
            _baseOptions = baseOptions ?? throw new System.ArgumentNullException(nameof(baseOptions));
            _usefluentAssertions = usefluentAssertions;
            _useAutoFixture = useAutoFixture;
            _useAutoFixtureForMocking = useAutoFixtureForMocking;
            _testFramework = testFramework;
            _mockingFramework = mockingFramework;
        }

        public TestFrameworkTypes FrameworkType => _testFramework ?? _baseOptions.FrameworkType;

        public MockingFrameworkType MockingFrameworkType => _mockingFramework ?? _baseOptions.MockingFrameworkType;

        public bool UseFluentAssertions => _usefluentAssertions ?? _baseOptions.UseFluentAssertions;

        public bool UseAutoFixture => _useAutoFixture ?? _baseOptions.UseAutoFixture;

        public bool UseAutoFixtureForMocking => _useAutoFixtureForMocking ?? _baseOptions.UseAutoFixtureForMocking;

        public bool AutoDetectFrameworkTypes => _baseOptions.AutoDetectFrameworkTypes;

        public bool AllowGenerationWithoutTargetProject => _baseOptions.AllowGenerationWithoutTargetProject;

        public string TestProjectNaming => _baseOptions.TestProjectNaming;

        public string TestFileNaming => _baseOptions.TestFileNaming;

        public string TestTypeNaming => _baseOptions.TestTypeNaming;

        public bool EmitUsingsOutsideNamespace => _baseOptions.EmitUsingsOutsideNamespace;

        public bool PartialGenerationAllowed => _baseOptions.PartialGenerationAllowed;

        public bool EmitTestsForInternals => _baseOptions.EmitTestsForInternals;

        public bool AutomaticallyConfigureMocks => _baseOptions.AutomaticallyConfigureMocks;

        public bool EmitSubclassForProtectedMethods => _baseOptions.EmitSubclassForProtectedMethods;

        public string ArrangeComment => _baseOptions.ArrangeComment;

        public string ActComment => _baseOptions.ActComment;

        public string AssertComment => _baseOptions.AssertComment;

        public UserInterfaceModes UserInterfaceMode => _baseOptions.UserInterfaceMode;

        public bool RememberManuallySelectedTargetProjectByDefault => _baseOptions.RememberManuallySelectedTargetProjectByDefault;

        public FallbackTargetFindingMethod FallbackTargetFinding => _baseOptions.FallbackTargetFinding;

        public bool PrefixFieldReferencesWithThis => _baseOptions.PrefixFieldReferencesWithThis;

        public bool EmitXmlDocumentation => _baseOptions.EmitXmlDocumentation;

        public bool UseMockBehaviorStrict => _baseOptions.UseMockBehaviorStrict;

        public bool CreateTargetAssets => _baseOptions.CreateTargetAssets;

        public string TestTypeBaseClass => _baseOptions.TestTypeBaseClass;

        public string TestTypeBaseClassNamespace => _baseOptions.TestTypeBaseClassNamespace;
    }
}
