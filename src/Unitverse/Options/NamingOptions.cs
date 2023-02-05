// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global - is set by VS
namespace Unitverse.Options
{
    using System.ComponentModel;
    using Microsoft.VisualStudio.Shell;
    using Unitverse.Core.Options;

    public class NamingOptions : DialogPage, INamingOptions
    {
        [Category("Constructors")]
        [DisplayName("CanConstruct")]
        [Description("Naming format for the main constructor test")]
        public string CanConstructNamingPattern { get; set; } = "CanConstruct";

        [Category("Constructors")]
        [DisplayName("CannotConstructWithNull")]
        [Description("Naming format for the constructor test that checks null guards")]
        public string CannotConstructWithNullNamingPattern { get; set; } = "CannotConstructWithNull{parameterName}";

        [Category("Constructors")]
        [DisplayName("CannotConstructWithInvalid")]
        [Description("Naming format for the constructor test that checks string null or white space guards")]
        public string CannotConstructWithInvalidNamingPattern { get; set; } = "CannotConstructWithInvalid{parameterName}";

        [Category("Initializers")]
        [DisplayName("CanInitialize")]
        [Description("Naming format for the main initializer test")]
        public string CanInitializeNamingPattern { get; set; } = "CanInitialize";

        [Category("Initializers")]
        [DisplayName("CannotInitializeWithNull")]
        [Description("Naming format for the initializer test that checks null guards")]
        public string CannotInitializeWithNullNamingPattern { get; set; } = "CannotInitializeWithNull{memberName}";

        [Category("Initializers")]
        [DisplayName("CannotInitializeWithInvalid")]
        [Description("Naming format for the initializer test that checks string null or white space guards")]
        public string CannotInitializeWithInvalidNamingPattern { get; set; } = "CannotInitializeWithInvalid{memberName}";

        [Category("Properties && Indexers")]
        [DisplayName("CanGet")]
        [Description("Naming format for read-only property tests")]
        public string CanGetNamingPattern { get; set; } = "CanGet{memberName}";

        [Category("Properties && Indexers")]
        [DisplayName("CanSetAndGet")]
        [Description("Naming format for read-write property tests")]
        public string CanSetAndGetNamingPattern { get; set; } = "CanSetAndGet{memberName}";

        [Category("Properties && Indexers")]
        [DisplayName("CanSet")]
        [Description("Naming format for read-only property tests")]
        public string CanSetNamingPattern { get; set; } = "CanSet{memberName}";

        [Category("Properties && Indexers")]
        [DisplayName("IsInitializedCorrectly")]
        [Description("Naming format for tests that ensure properties are set correctly by the constructor")]
        public string IsInitializedCorrectlyNamingPattern { get; set; } = "{memberName}IsInitializedCorrectly";

        [Category("Interfaces")]
        [DisplayName("ImplementsIEnumerable")]
        [Description("Naming format for the IEnumerable implementation test")]
        public string ImplementsIEnumerableNamingPattern { get; set; } = "ImplementsIEnumerable{typeParameters}";

        [Category("Interfaces")]
        [DisplayName("ImplementsIComparable")]
        [Description("Naming format for the IComparable implementation test")]
        public string ImplementsIComparableNamingPattern { get; set; } = "ImplementsIComparable{typeParameters}";

        [Category("Interfaces")]
        [DisplayName("ImplementsIEquatable")]
        [Description("Naming format for the IEquatable implementation test")]
        public string ImplementsIEquatableNamingPattern { get; set; } = "ImplementsIEquatable{typeParameters}";

        [Category("Methods")]
        [DisplayName("CanCall")]
        [Description("Naming format for the main method test")]
        public string CanCallNamingPattern { get; set; } = "CanCall{memberName}";

        [Category("Methods")]
        [DisplayName("PerformsMapping")]
        [Description("Naming format for mapping method tests")]
        public string PerformsMappingNamingPattern { get; set; } = "{memberName}PerformsMapping";

        [Category("Methods")]
        [DisplayName("CannotCallWithNull")]
        [Description("Naming format for the method test that checks null guards")]
        public string CannotCallWithNullNamingPattern { get; set; } = "CannotCall{memberName}WithNull{parameterName}";

        [Category("Methods")]
        [DisplayName("CannotCallWithInvalid")]
        [Description("Naming format for the method test that checks string null or white space guards")]
        public string CannotCallWithInvalidNamingPattern { get; set; } = "CannotCall{memberName}WithInvalid{parameterName}";

        [Category("Operators")]
        [DisplayName("CanCallOperator")]
        [Description("Naming format for the main operator test")]
        public string CanCallOperatorNamingPattern { get; set; } = "CanCall{memberName}Operator";

        [Category("Operators")]
        [DisplayName("CannotCallOperatorWithNull")]
        [Description("Naming format for the operator test that checks null guards")]
        public string CannotCallOperatorWithNullNamingPattern { get; set; } = "CannotCall{memberName}OperatorWithNull{parameterName}";

        [Category("Fields")]
        [DisplayName("Target Field Name")]
        [Description("Naming format for the field name used for the instance being tested")]
        public string TargetFieldName { get; set; } = "_testClass";

        [Category("Fields")]
        [DisplayName("Dependency Field Name")]
        [Description("Naming format for the field name used for dependencies")]
        public string DependencyFieldName { get; set; } = "_{parameterName:camel}";

        [Category("Fields")]
        [DisplayName("AutoFixture Field Name")]
        [Description("Naming format for the field name used for auto fixture")]
        public string AutoFixtureFieldName { get; set; } = "_fixture";
    }
}