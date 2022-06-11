namespace Unitverse.Specs
{
    using System.Collections.Generic;
    using Unitverse.Core.Options;

    // ENHANCE - add fluent assertions to specs
    public class GenerationOptions : IGenerationOptions
    {
        public GenerationOptions(TestFrameworkTypes testFramework, MockingFrameworkType mockFramework)
        {
            FrameworkType = testFramework;
            MockingFrameworkType = mockFramework;
        }

        public static IUnitTestGeneratorOptions Get(TestFrameworkTypes testFramework, MockingFrameworkType mockFramework)
        {
            return new UnitTestGeneratorOptions(new GenerationOptions(testFramework, mockFramework), new DefaultNamingOptions(), new DefaultStrategyOptions(), false, new Dictionary<string, string>());
        }

        public TestFrameworkTypes FrameworkType { get; }
        public MockingFrameworkType MockingFrameworkType { get; }
        public bool CreateProjectAutomatically { get; } = true;
        public bool AddReferencesAutomatically { get; } = true;
        public bool AllowGenerationWithoutTargetProject { get; } = true;
        public string TestProjectNaming { get; } = "{0}.Tests";
        public string TestFileNaming { get; } = "{0}Tests";
        public string TestTypeNaming { get; } = "{0}Tests";

        public bool UseFluentAssertions { get; }

        public bool UseAutoFixture { get; }

        public bool AutoDetectFrameworkTypes { get; }

        public bool EmitUsingsOutsideNamespace { get; }

        public bool PartialGenerationAllowed { get; } = false;

        public bool EmitTestsForInternals { get; } = false;

        public bool AutomaticallyConfigureMocks { get; } = true;

        public bool EmitSubclassForProtectedMethods { get; } = true;

        public string ArrangeComment => "Arrange";

        public string ActComment => "Act";

        public string AssertComment => "Assert";

        public UserInterfaceModes UserInterfaceMode => UserInterfaceModes.OnlyWhenControlPressed;

        public bool RememberManuallySelectedTargetProjectByDefault => true;

        public FallbackTargetFindingMethod FallbackTargetFinding => FallbackTargetFindingMethod.None;

        public bool PrefixFieldReferencesWithThis => false;

        public bool EmitXmlDocumentation => false;
    }
}