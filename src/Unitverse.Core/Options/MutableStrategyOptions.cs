namespace Unitverse.Core.Options
{
    using System;

    public class MutableStrategyOptions : IStrategyOptions
    {
        public MutableStrategyOptions(IStrategyOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            ConstructorChecksAreEnabled = options.ConstructorChecksAreEnabled;
            InitializerChecksAreEnabled = options.InitializerChecksAreEnabled;
            ConstructorParameterChecksAreEnabled = options.ConstructorParameterChecksAreEnabled;
            InitializerPropertyChecksAreEnabled = options.InitializerPropertyChecksAreEnabled;
            MethodCallChecksAreEnabled = options.MethodCallChecksAreEnabled;
            MappingMethodChecksAreEnabled = options.MappingMethodChecksAreEnabled;
            MethodParameterChecksAreEnabled = options.MethodParameterChecksAreEnabled;
            IndexerChecksAreEnabled = options.IndexerChecksAreEnabled;
            PropertyChecksAreEnabled = options.PropertyChecksAreEnabled;
            InitializedPropertyChecksAreEnabled = options.InitializedPropertyChecksAreEnabled;
            OperatorChecksAreEnabled = options.OperatorChecksAreEnabled;
            OperatorParameterChecksAreEnabled = options.OperatorParameterChecksAreEnabled;
            InterfaceImplementationChecksAreEnabled = options.InterfaceImplementationChecksAreEnabled;
        }

        public bool ConstructorChecksAreEnabled { get; set; }

        public bool InitializerChecksAreEnabled { get; set; }

        public bool ConstructorParameterChecksAreEnabled { get; set; }

        public bool InitializerPropertyChecksAreEnabled { get; set; }

        public bool MethodCallChecksAreEnabled { get; set; }

        public bool MappingMethodChecksAreEnabled { get; set; }

        public bool MethodParameterChecksAreEnabled { get; set; }

        public bool IndexerChecksAreEnabled { get; set; }

        public bool PropertyChecksAreEnabled { get; set; }

        public bool InitializedPropertyChecksAreEnabled { get; set; }

        public bool OperatorChecksAreEnabled { get; set; }

        public bool OperatorParameterChecksAreEnabled { get; set; }

        public bool InterfaceImplementationChecksAreEnabled { get; set; }
    }
}