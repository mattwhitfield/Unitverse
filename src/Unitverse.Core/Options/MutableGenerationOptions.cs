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
            UseShouldly = options.UseShouldly;
            UseAutoFixture = options.UseAutoFixture;
            UseAutoFixtureForMocking = options.UseAutoFixtureForMocking;
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
            FallbackTargetFinding = options.FallbackTargetFinding;
            PrefixFieldReferencesWithThis = options.PrefixFieldReferencesWithThis;
            EmitXmlDocumentation = options.EmitXmlDocumentation;
            UseMockBehaviorStrict = options.UseMockBehaviorStrict;
            CreateTargetAssets = options.CreateTargetAssets;
            TestTypeBaseClass = options.TestTypeBaseClass;
            TestTypeBaseClassNamespace = options.TestTypeBaseClassNamespace;
#if VS2022
            GenerateFileScopedNamespaces = options.GenerateFileScopedNamespaces;
#endif
            PlaceSystemUsingDirectivesFirst = options.PlaceSystemUsingDirectivesFirst;
            UseFieldForAutoFixture = options.UseFieldForAutoFixture;
            DefaultFailureMessage = options.DefaultFailureMessage;
            SkipInternalTypesOnMultipleGeneration = options.SkipInternalTypesOnMultipleGeneration;
            EmitMultilinePocoInitializers = options.EmitMultilinePocoInitializers;
            UseFieldsForConstructorParameterTests = options.UseFieldsForConstructorParameterTests;
        }

        public TestFrameworkTypes FrameworkType { get; set; }

        public MockingFrameworkType MockingFrameworkType { get; set; }

        public bool AllowGenerationWithoutTargetProject { get; set; }

        public string TestProjectNaming { get; set; }

        public string TestFileNaming { get; set; }

        public string TestTypeNaming { get; set; }

        public bool UseFluentAssertions { get; set; }

        public bool UseShouldly { get; set; }

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

        public FallbackTargetFindingMethod FallbackTargetFinding { get; set; }

        public bool PrefixFieldReferencesWithThis { get; set; }

        public bool EmitXmlDocumentation { get; set; }

        public bool UseAutoFixtureForMocking { get; set; }

        public bool UseMockBehaviorStrict { get; set; }

        public bool CreateTargetAssets { get; set; }

        public string TestTypeBaseClass { get; set; }

        public string TestTypeBaseClassNamespace { get; set; }
#if VS2022

        public bool GenerateFileScopedNamespaces { get; set; }
#endif

        public bool PlaceSystemUsingDirectivesFirst { get; set; }

        public bool UseFieldForAutoFixture { get; set; }

        public bool SkipInternalTypesOnMultipleGeneration { get; set; }

        public string DefaultFailureMessage { get; set; }

        public bool EmitMultilinePocoInitializers { get; set; }

        public bool UseFieldsForConstructorParameterTests { get; set; }

        public bool UseConstructorForTestClassSetUp { get; set; }

        public bool OmitTestClassAttribute { get; set; }
    }
}