namespace Unitverse.Core.Options
{
    public interface INamingProvider
    {
        NameResolver CanCall { get; }

        NameResolver CanConstruct { get; }

        NameResolver CannotConstructWithNull { get; }

        NameResolver CannotConstructWithInvalid { get; }

        NameResolver CanGet { get; }

        NameResolver CanSetAndGet { get; }

        NameResolver CanSet { get; }

        NameResolver ImplementsIEnumerable { get; }

        NameResolver ImplementsIComparable { get; }

        NameResolver PerformsMapping { get; }

        NameResolver CannotCallWithNull { get; }

        NameResolver CannotCallWithInvalid { get; }

        NameResolver CanCallOperator { get; }

        NameResolver CannotCallOperatorWithNull { get; }

        NameResolver IsInitializedCorrectly { get; }

        NameResolver TargetFieldName { get; }

        NameResolver DependencyFieldName { get; }
    }
}
