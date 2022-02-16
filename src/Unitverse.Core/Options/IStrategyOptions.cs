namespace Unitverse.Core.Options
{
    public interface IStrategyOptions
    {
        bool ConstructorChecksAreEnabled { get; }

        bool InitializerChecksAreEnabled { get; }

        bool ConstructorParameterChecksAreEnabled { get; }

        bool InitializerPropertyChecksAreEnabled { get; }

        bool MethodCallChecksAreEnabled { get; }

        bool MappingMethodChecksAreEnabled { get; }

        bool MethodParameterChecksAreEnabled { get; }

        bool IndexerChecksAreEnabled { get; }

        bool PropertyChecksAreEnabled { get; }

        bool InitializedPropertyChecksAreEnabled { get; }

        bool OperatorChecksAreEnabled { get; }

        bool OperatorParameterChecksAreEnabled { get; }

        bool InterfaceImplementationChecksAreEnabled { get; }
    }
}
