namespace Unitverse.Core.Options
{
    public interface INamingOptions
    {
        string CanCallNamingPattern { get; }

        string CanConstructNamingPattern { get; }

        string CannotConstructWithNullNamingPattern { get; }

        string CannotConstructWithInvalidNamingPattern { get; }

        string CanInitializeNamingPattern { get; }

        string CannotInitializeWithNullNamingPattern { get; }

        string CannotInitializeWithInvalidNamingPattern { get; }

        string CanGetNamingPattern { get; }

        string CanSetAndGetNamingPattern { get; }

        string CanSetNamingPattern { get; }

        string ImplementsIEnumerableNamingPattern { get; }

        string ImplementsIComparableNamingPattern { get; }

        string ImplementsIEquatableNamingPattern { get; }

        string PerformsMappingNamingPattern { get; }

        string CannotCallWithNullNamingPattern { get; }

        string CannotCallWithInvalidNamingPattern { get; }

        string CanCallOperatorNamingPattern { get; }

        string CannotCallOperatorWithNullNamingPattern { get; }

        string IsInitializedCorrectlyNamingPattern { get; }

        string TargetFieldName { get; }

        string DependencyFieldName { get; }

        string MockDependencyFieldName { get; }

        string AutoFixtureFieldName { get; }

        bool ForceAsyncSuffix { get; }
    }
}
