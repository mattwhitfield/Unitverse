﻿namespace Unitverse.Options
{
    using System.ComponentModel;
    using Microsoft.VisualStudio.Shell;
    using Unitverse.Core.Options;
    using Unitverse.Core.Options.Editing;

    public class GenerationOptions : DialogPage, IGenerationOptions
    {

        [Category("Frameworks")]
        [DisplayName("Auto-detect frameworks")]
        [Description("Whether to detect the test, mocking and assertion frameworks based on the packages currently installed in the target project")]
        [ExcludedFromUserInterface]
        public bool AutoDetectFrameworkTypes { get; set; } = true;

        [Category("Frameworks")]
        [DisplayName("Test framework type")]
        [Description("The type of test framework to use")]
        public TestFrameworkTypes FrameworkType { get; set; } = TestFrameworkTypes.NUnit3;

        [Category("Frameworks")]
        [DisplayName("Mocking framework type")]
        [Description("The type of mocking framework to use")]
        public MockingFrameworkType MockingFrameworkType { get; set; } = MockingFrameworkType.NSubstitute;

        [Category("Test project")]
        [DisplayName("Allow generation without target project")]
        [Description("Tests will be generated as a file outside of the scope of a project if a target project is not available")]
        [ExcludedFromUserInterface]
        public bool AllowGenerationWithoutTargetProject { get; set; } = true;

        [Category("Naming")]
        [DisplayName("Project naming convention")]
        [Description("Format string that converts the source project name to the unit test project name")]
        [ExcludedFromUserInterface]
        public string TestProjectNaming { get; set; } = "{0}.Tests";

        [Category("Naming")]
        [DisplayName("File naming convention")]
        [Description("Format string that converts the source file name to the unit test file name")]
        public string TestFileNaming { get; set; } = "{0}Tests";

        [Category("Naming")]
        [DisplayName("Type naming convention")]
        [Description("Format string that converts the source type name to the unit test type name")]
        public string TestTypeNaming { get; set; } = "{0}Tests";

        [Category("Frameworks")]
        [DisplayName("Use Fluent Assertions")]
        [Description("Whether to use Fluent Assertions in preference to the test framework's built in assertion capabilities")]
        public bool UseFluentAssertions { get; set; } = false;

        [Category("Frameworks")]
        [DisplayName("Use Shouldly")]
        [Description("Whether to use Shouldly in preference to the test framework's built in assertion capabilities")]
        public bool UseShouldly { get; set; } = false;

        [Category("Frameworks")]
        [DisplayName("Use AutoFixture")]
        [Description("Whether to use AutoFixture when generating scalar values")]
        public bool UseAutoFixture { get; set; } = false;

        [Category("Frameworks")]
        [DisplayName("Use AutoFixture for mocking")]
        [Description("Whether to use AutoFixture when generating mocks ('Use AutoFixture' must be enabled)")]
        public bool UseAutoFixtureForMocking { get; set; } = false;

        [Category("Frameworks")]
        [DisplayName("Use field for AutoFixture")]
        [Description("Whether to use a class-level field for AutoFixture ('Use AutoFixture' must be enabled)")]
        public bool UseFieldForAutoFixture { get; set; } = false;

        [Category("Layout")]
        [DisplayName("Emit using directives outside namespace")]
        [Description("Whether to place the required using directives outside the namespace declaration. If using file scoped namespaces, enabling this option causes the using directives to be placed above the file scoped namespace declaration")]
        public bool EmitUsingsOutsideNamespace { get; set; } = false;

        [Category("Generation")]
        [DisplayName("Partial generation")]
        [Description("When enabled, any new test methods are allowed to be added to the target test class without causing an error")]
        public bool PartialGenerationAllowed { get; set; } = true;

        [Category("Generation")]
        [DisplayName("Emit tests for internal members")]
        [Description("Whether to emit tests for members marked as internal as well as public members")]
        public bool EmitTestsForInternals { get; set; } = false;

        [Category("Generation")]
        [DisplayName("Automatically configure mocks")]
        [Description("Whether to detect calls to injected mocks and automatically emit setup and verification calls")]
        public bool AutomaticallyConfigureMocks { get; set; } = true;

        [Category("Generation")]
        [DisplayName("Emit subclass for protected members")]
        [Description("Whether to emit a subclass that exposes protected methods to allow them to be tested")]
        public bool EmitSubclassForProtectedMethods { get; set; } = true;

        [Category("Comments")]
        [DisplayName("Arrange block comment")]
        [Description("The comment to leave before any arrange statements (leave blank to suppress)")]
        public string ArrangeComment { get; set; } = "Arrange";

        [Category("Comments")]
        [DisplayName("Act block comment")]
        [Description("The comment to leave before any act statements (leave blank to suppress)")]
        public string ActComment { get; set; } = "Act";

        [Category("Comments")]
        [DisplayName("Assert block comment")]
        [Description("The comment to leave before any assert statements (leave blank to suppress)")]
        public string AssertComment { get; set; } = "Assert";

        [Category("User Interface")]
        [DisplayName("Show user interface")]
        [Description("Whether to show the target project and options user interface during generation")]
        [ExcludedFromUserInterface]
        public UserInterfaceModes UserInterfaceMode { get; set; } = UserInterfaceModes.WhenTargetNotFound;

        [Category("Naming")]
        [DisplayName("Fallback type finding method")]
        [Description("The method to use when finding tests by the file name does not work")]
        public FallbackTargetFindingMethod FallbackTargetFinding { get; set; } = FallbackTargetFindingMethod.TypeInCorrectNamespace;

        [Category("StyleCop compatibility")]
        [DisplayName("Prefix field references with this")]
        [Description("True if field references should be prefixed with 'this.' because that StyleCop rule is set")]
        public bool PrefixFieldReferencesWithThis { get; set; } = false;

        [Category("StyleCop compatibility")]
        [DisplayName("Emit XML documentation")]
        [Description("True if XML documentation should be generated for test methods")]
        public bool EmitXmlDocumentation { get; set; } = false;

        [Category("Framework specifics")]
        [DisplayName("Use MockBehavior.Strict")]
        [Description("True if Moq mocks should be configured with MockBehavior.Strict")]
        public bool UseMockBehaviorStrict { get; set; } = false;

        [Category("Generation")]
        [DisplayName("Create target assets")]
        [Description("True if target assets (files that contain supporting functionality in a test project) should be created")]
        public bool CreateTargetAssets { get; set; } = true;

        [Category("Generation")]
        [DisplayName("Test type base class")]
        [Description("The name of the base class from which a test type should derive, or empty for no inheritance - if not a fully qualified name, test type base class namespace should be specified")]
        public string TestTypeBaseClass { get; set; } = string.Empty;

        [Category("Generation")]
        [DisplayName("Test type base class namespace")]
        [Description("The namespace of the base class from which a test type should derive which will be emitted in the list of using statements")]
        public string TestTypeBaseClassNamespace { get; set; } = string.Empty;
#if VS2022

        [Category("Layout")]
        [DisplayName("Generate file scoped namespaces")]
        [Description("When generating tests, emit a file that uses file scoped namespaces")]
        public bool GenerateFileScopedNamespaces { get; set; } = false;
#endif

        [Category("Layout")]
        [DisplayName("Place System usings first")]
        [Description("When sorting using directives, place the 'System' directives first")]
        public bool PlaceSystemUsingDirectivesFirst { get; set; } = true;

        [Category("Generation")]
        [DisplayName("Skip internal types on multiple generation")]
        [Description("Whether to skip internal types when generating for multiple files")]
        public bool SkipInternalTypesOnMultipleGeneration { get; set; } = false;

        [Category("Comments")]
        [DisplayName("Default failure message")]
        [Description("The message to use in failure asserts for skeleton tests")]
        public string DefaultFailureMessage { get; set; } = "Create or modify test";

        [Category("Generation")]
        [DisplayName("Emit multi-line initializers")]
        [Description("Whether to format type initializers over multiple lines for readability")] 
        public bool EmitMultilinePocoInitializers { get; set; } = true;

        [Category("Generation")]
        [DisplayName("Use fields for constructor parameter tests")]
        [Description("Whether to use fields for constructor parameter tests, or to create a new value for every test")]
        public bool UseFieldsForConstructorParameterTests { get; set; } = true;

        [Category("Generation")]
        [DisplayName("Use constructor for test class set up")]
        [Description("Whether to use constructor for test class set up instead of the framework set up method")]
        public bool UseConstructorForTestClassSetUp { get; set; } = false;

        [Category("Generation")]
        [DisplayName("Omit test class attribute")]
        [Description("Whether to omit the attribute used to decorate test classes")]
        public bool OmitTestClassAttribute { get; set; } = false;
        
        [Category("Generation")]
        [DisplayName("Use separate checks for null and empty")]
        [Description("Whether to emit separate methods for null and empty string checks")]
        public bool UseSeparateChecksForNullAndEmpty { get; set; } = false;

        [Category("Naming")]
        [DisplayName("Include source project as target folder")]
        [Description("Whether to include the source project as part of the target folder structure")]
        public bool IncludeSourceProjectAsFolder { get; set; } = false;
    }
}