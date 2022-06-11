namespace Unitverse.Core.Options
{
    using System;

    public class MutableGenerationOptions : IGenerationOptions
    {
        public MutableGenerationOptions(IGenerationOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            FrameworkType = options.FrameworkType;
            MockingFrameworkType = options.MockingFrameworkType;
            AllowGenerationWithoutTargetProject = options.AllowGenerationWithoutTargetProject;
            TestProjectNaming = options.TestProjectNaming;
            TestFileNaming = options.TestFileNaming;
            TestTypeNaming = options.TestTypeNaming;
            UseFluentAssertions = options.UseFluentAssertions;
            UseAutoFixture = options.UseAutoFixture;
            AutoDetectFrameworkTypes = options.AutoDetectFrameworkTypes;
            EmitUsingsOutsideNamespace = options.EmitUsingsOutsideNamespace;
            PartialGenerationAllowed = options.PartialGenerationAllowed;
            EmitTestsForInternals = options.EmitTestsForInternals;
            AutomaticallyConfigureMocks = options.AutomaticallyConfigureMocks;
            EmitSubclassForProtectedMethods = options.EmitSubclassForProtectedMethods;
            ArrangeComment = options.ArrangeComment;
            ActComment = options.ActComment;
            AssertComment = options.AssertComment;
            UserInterfaceMode = options.UserInterfaceMode;
            RememberManuallySelectedTargetProjectByDefault = options.RememberManuallySelectedTargetProjectByDefault;
            FallbackTargetFinding = options.FallbackTargetFinding;
            PrefixFieldReferencesWithThis = options.PrefixFieldReferencesWithThis;
            EmitXmlDocumentation = options.EmitXmlDocumentation;
        }

        public TestFrameworkTypes FrameworkType { get; set; }

        public MockingFrameworkType MockingFrameworkType { get; set; }

        public bool AllowGenerationWithoutTargetProject { get; set; }

        public string TestProjectNaming { get; set; }

        public string TestFileNaming { get; set; }

        public string TestTypeNaming { get; set; }

        public bool UseFluentAssertions { get; set; }

        public bool UseAutoFixture { get; set; }

        public bool AutoDetectFrameworkTypes { get; set; }

        public bool EmitUsingsOutsideNamespace { get; set; }

        public bool PartialGenerationAllowed { get; set; }

        public bool EmitTestsForInternals { get; set; }

        public bool EmitSubclassForProtectedMethods { get; set; }

        public bool AutomaticallyConfigureMocks { get; set; }

        public string ArrangeComment { get; set; }

        public string ActComment { get; set; }

        public string AssertComment { get; set; }

        public UserInterfaceModes UserInterfaceMode { get; set; }

        public bool RememberManuallySelectedTargetProjectByDefault { get; set; }

        public FallbackTargetFindingMethod FallbackTargetFinding { get; set; }

        public bool PrefixFieldReferencesWithThis { get; set; }

        public bool EmitXmlDocumentation { get; set; }
    }
}