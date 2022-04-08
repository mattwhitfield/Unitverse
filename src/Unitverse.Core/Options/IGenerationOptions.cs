namespace Unitverse.Core.Options
{
    public interface IGenerationOptions
    {
        TestFrameworkTypes FrameworkType { get; }

        MockingFrameworkType MockingFrameworkType { get; }

        bool UseFluentAssertions { get; }

        bool AutoDetectFrameworkTypes { get; }

        bool AllowGenerationWithoutTargetProject { get; }

        string TestProjectNaming { get; }

        string TestFileNaming { get; }

        string TestTypeNaming { get; }

        bool EmitUsingsOutsideNamespace { get; }

        bool PartialGenerationAllowed { get; }

        bool EmitTestsForInternals { get; }

        bool EmitSubclassForProtectedMethods { get; }

        bool AutomaticallyConfigureMocks { get; }

        string ArrangeComment { get; }

        string ActComment { get; }

        string AssertComment { get; }

        UserInterfaceModes UserInterfaceMode { get; }

        bool RememberManuallySelectedTargetProjectByDefault { get; }

        FallbackTargetFindingMethod FallbackTargetFinding { get; }
    }
}