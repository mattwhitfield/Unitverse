namespace Unitverse.ExampleGenerator
{
    using Unitverse.Core.Options;

    public class DefaultNamingOptions : INamingOptions
    {
        public string CanConstructNamingPattern { get; set; } = "CanConstruct";

        public string CannotConstructWithNullNamingPattern { get; set; } = "CannotConstructWithNull{parameterName}";

        public string CannotConstructWithInvalidNamingPattern { get; set; } = "CannotConstructWithInvalid{parameterName}";

        public string CanGetNamingPattern { get; set; } = "CanGet{memberName}";

        public string CanSetAndGetNamingPattern { get; set; } = "CanSetAndGet{memberName}";

        public string CanSetNamingPattern { get; set; } = "CanSet{memberName}";

        public string IsInitializedCorrectlyNamingPattern { get; set; } = "{memberName}IsInitializedCorrectly";

        public string ImplementsIEnumerableNamingPattern { get; set; } = "ImplementsIEnumerable{typeParameters}";

        public string ImplementsIComparableNamingPattern { get; set; } = "ImplementsIComparable{typeParameters}";

        public string CanCallNamingPattern { get; set; } = "CanCall{memberName}";

        public string PerformsMappingNamingPattern { get; set; } = "{memberName}PerformsMapping";

        public string CannotCallWithNullNamingPattern { get; set; } = "CannotCall{memberName}WithNull{parameterName}";

        public string CannotCallWithInvalidNamingPattern { get; set; } = "CannotCall{memberName}WithInvalid{parameterName}";

        public string CanCallOperatorNamingPattern { get; set; } = "CanCall{memberName}Operator";

        public string CannotCallOperatorWithNullNamingPattern { get; set; } = "CannotCall{memberName}OperatorWithNull{parameterName}";

        public string TargetFieldName { get; set; } = "_testClass";

        public string DependencyFieldName { get; set; } = "_{parameterName:camel}";
    }
}