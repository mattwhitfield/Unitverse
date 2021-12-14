namespace Unitverse.Core.Options
{
    public interface INamingOptions
    {
        string CanCallNamingPattern { get; }

        string CanConstructNamingPattern { get; }

        string CannotConstructWithNullNamingPattern { get; }

        string CannotConstructWithInvalidNamingPattern { get; }

        string CanGetNamingPattern { get; }

        string CanSetAndGetNamingPattern { get; }

        string CanSetNamingPattern { get; }

        string ImplementsIEnumerableNamingPattern { get; }

        string ImplementsIComparableNamingPattern { get; }

        string PerformsMappingNamingPattern { get; }

        string CannotCallWithNullNamingPattern { get; }

        string CannotCallWithInvalidNamingPattern { get; }

        string CanCallOperatorNamingPattern { get; }

        string CannotCallOperatorWithNullNamingPattern { get; }

        string IsInitializedCorrectlyNamingPattern { get; }

        string TargetFieldName { get; }

        string DependencyFieldName { get; }
    }
}
