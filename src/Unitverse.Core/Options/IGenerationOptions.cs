namespace Unitverse.Core.Options
{
    public interface IGenerationOptions
    {
        TestFrameworkTypes FrameworkType { get; }

        MockingFrameworkType MockingFrameworkType { get; }

        bool UseFluentAssertions { get; }

        bool UseShouldly { get; }

        bool UseAutoFixture { get; }

        bool UseAutoFixtureForMocking { get; }

        bool UseFieldForAutoFixture { get; }

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

        FallbackTargetFindingMethod FallbackTargetFinding { get; }

        bool PrefixFieldReferencesWithThis { get; }

        bool EmitXmlDocumentation { get; }

        bool UseMockBehaviorStrict { get; }

        bool CreateTargetAssets { get; }

        string TestTypeBaseClass { get; }

        string TestTypeBaseClassNamespace { get; }
#if VS2022

        bool GenerateFileScopedNamespaces { get; }
#endif

        bool PlaceSystemUsingDirectivesFirst { get; }

        bool SkipInternalTypesOnMultipleGeneration { get; }

        string DefaultFailureMessage { get; }

        bool EmitMultilinePocoInitializers { get; }

        bool UseFieldsForConstructorParameterTests { get; }
    }
}