namespace Unitverse.Tests.Common
{
    using Unitverse.Core.Options;

    public class DefaultStrategyOptions : IStrategyOptions
    {
        public bool ConstructorChecksAreEnabled => true;

        public bool InitializerChecksAreEnabled => true;

        public bool ConstructorParameterChecksAreEnabled => true;

        public bool InitializerPropertyChecksAreEnabled => true;

        public bool MethodCallChecksAreEnabled => true;

        public bool MappingMethodChecksAreEnabled => true;

        public bool MethodParameterChecksAreEnabled => true;

        public bool IndexerChecksAreEnabled => true;

        public bool PropertyChecksAreEnabled => true;

        public bool InitializedPropertyChecksAreEnabled => true;

        public bool OperatorChecksAreEnabled => true;

        public bool OperatorParameterChecksAreEnabled => true;

        public bool InterfaceImplementationChecksAreEnabled => true;
    }
}