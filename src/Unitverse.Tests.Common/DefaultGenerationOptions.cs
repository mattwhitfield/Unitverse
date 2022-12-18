namespace Unitverse.Tests.Common
{
    using System.ComponentModel;
    using Unitverse.Core.Options;

    public class DefaultGenerationOptions : IGenerationOptions
    {
        public TestFrameworkTypes FrameworkType { get; set; } = TestFrameworkTypes.XUnit;

        public MockingFrameworkType MockingFrameworkType { get; set; } = MockingFrameworkType.NSubstitute;

        public bool UseFluentAssertions { get; set; } = true;

        public bool UseAutoFixture { get; set; } = false;

        public bool UseAutoFixtureForMocking { get; set; } = false;

        public bool AutoDetectFrameworkTypes { get; set; } = false;

        public bool AllowGenerationWithoutTargetProject { get; set; } = false;

        public string TestProjectNaming { get; set; } = "{0}.Tests";

        public string TestFileNaming { get; set; } = "{0}Tests";

        public string TestTypeNaming { get; set; } = "{0}Tests";

        public bool EmitUsingsOutsideNamespace { get; set; } = false;

        public bool PartialGenerationAllowed { get; set; } = false;

        public bool EmitTestsForInternals { get; set; } = false;

        public bool AutomaticallyConfigureMocks { get; set; } = true;

        public bool EmitSubclassForProtectedMethods { get; set; } = true;

        public string ArrangeComment { get; set; } = "Arrange";

        public string ActComment { get; set; } = "Act";

        public string AssertComment { get; set; } = "Assert";

        public UserInterfaceModes UserInterfaceMode { get; set; } = UserInterfaceModes.OnlyWhenControlPressed;

        public bool RememberManuallySelectedTargetProjectByDefault { get; set; } = true;

        public FallbackTargetFindingMethod FallbackTargetFinding { get; set; } = FallbackTargetFindingMethod.None;

        public bool PrefixFieldReferencesWithThis { get; set; } = false;

        public bool EmitXmlDocumentation { get; set; } = false;

        public bool UseMockBehaviorStrict { get; set; } = false;

        public bool CreateTargetAssets { get; set; } = true;

        public string TestTypeBaseClass { get; set; } = string.Empty;

        public string TestTypeBaseClassNamespace { get; set; } = string.Empty;
#if VS2022

        public bool GenerateFileScopedNamespaces { get; set; } = false;
#endif

        public bool PlaceSystemUsingDirectivesFirst { get; set; } = false;
    }
}