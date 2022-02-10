namespace Unitverse.Core.Options
{
    using System;

    public class MutableNamingOptions : INamingOptions
    {
        public MutableNamingOptions(INamingOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            CanCallNamingPattern = options.CanCallNamingPattern;
            CanConstructNamingPattern = options.CanConstructNamingPattern;
            CannotConstructWithNullNamingPattern = options.CannotConstructWithNullNamingPattern;
            CannotConstructWithInvalidNamingPattern = options.CannotConstructWithInvalidNamingPattern;
            CanInitializeNamingPattern = options.CanInitializeNamingPattern;
            CannotInitializeWithNullNamingPattern = options.CannotInitializeWithNullNamingPattern;
            CannotInitializeWithInvalidNamingPattern = options.CannotInitializeWithInvalidNamingPattern;
            CanGetNamingPattern = options.CanGetNamingPattern;
            CanSetAndGetNamingPattern = options.CanSetAndGetNamingPattern;
            CanSetNamingPattern = options.CanSetNamingPattern;
            ImplementsIEnumerableNamingPattern = options.ImplementsIEnumerableNamingPattern;
            ImplementsIComparableNamingPattern = options.ImplementsIComparableNamingPattern;
            PerformsMappingNamingPattern = options.PerformsMappingNamingPattern;
            CannotCallWithNullNamingPattern = options.CannotCallWithNullNamingPattern;
            CannotCallWithInvalidNamingPattern = options.CannotCallWithInvalidNamingPattern;
            CanCallOperatorNamingPattern = options.CanCallOperatorNamingPattern;
            CannotCallOperatorWithNullNamingPattern = options.CannotCallOperatorWithNullNamingPattern;
            IsInitializedCorrectlyNamingPattern = options.IsInitializedCorrectlyNamingPattern;
            TargetFieldName = options.TargetFieldName;
            DependencyFieldName = options.DependencyFieldName;
        }

        public string CanCallNamingPattern { get; set; }

        public string CanConstructNamingPattern { get; set; }

        public string CannotConstructWithNullNamingPattern { get; set; }

        public string CannotConstructWithInvalidNamingPattern { get; set; }

        public string CanInitializeNamingPattern { get; set; }

        public string CannotInitializeWithNullNamingPattern { get; set; }

        public string CannotInitializeWithInvalidNamingPattern { get; set; }

        public string CanGetNamingPattern { get; set; }

        public string CanSetAndGetNamingPattern { get; set; }

        public string CanSetNamingPattern { get; set; }

        public string ImplementsIEnumerableNamingPattern { get; set; }

        public string ImplementsIComparableNamingPattern { get; set; }

        public string PerformsMappingNamingPattern { get; set; }

        public string CannotCallWithNullNamingPattern { get; set; }

        public string CannotCallWithInvalidNamingPattern { get; set; }

        public string CanCallOperatorNamingPattern { get; set; }

        public string CannotCallOperatorWithNullNamingPattern { get; set; }

        public string IsInitializedCorrectlyNamingPattern { get; set; }

        public string TargetFieldName { get; set; }

        public string DependencyFieldName { get; set; }
    }
}