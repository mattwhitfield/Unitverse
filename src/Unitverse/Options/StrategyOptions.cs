// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global - is set by VS
namespace Unitverse.Options
{
    using System.ComponentModel;
    using Microsoft.VisualStudio.Shell;
    using Unitverse.Core.Options;

    internal class StrategyOptions : DialogPage, IStrategyOptions
    {
        [Category("Constructors")]
        [DisplayName("Basic Checks")]
        [Description("Whether to emit basic constructor checks (CanConstruct)")]
        public bool ConstructorChecksAreEnabled { get; set; } = true;

        [Category("Constructors")]
        [DisplayName("Parameter Checks")]
        [Description("Whether to emit null and string parameter checks for constructors (CannotConstructWith*)")]
        public bool ConstructorParameterChecksAreEnabled { get; set; } = true;

        [Category("Initializers")]
        [DisplayName("Basic Checks")]
        [Description("Whether to emit basic constructor checks (CanInitialize)")]
        public bool InitializerChecksAreEnabled { get; set; } = true;

        [Category("Initializers")]
        [DisplayName("Parameter Checks")]
        [Description("Whether to emit null and string parameter checks for initializers (CannotInitializeWith*)")]
        public bool InitializerPropertyChecksAreEnabled { get; set; } = true;

        [Category("Methods")]
        [DisplayName("Basic Checks")]
        [Description("Whether to emit tests that exercise methods (CanCall*)")]
        public bool MethodCallChecksAreEnabled { get; set; } = true;

        [Category("Methods")]
        [DisplayName("Mapping Method Checks")]
        [Description("Whether to emit tests that check mapping methods (*PerformsMapping)")]
        public bool MappingMethodChecksAreEnabled { get; set; } = true;

        [Category("Methods")]
        [DisplayName("Parameter Checks")]
        [Description("Whether to emit null and string parameter checks for methods (CannotCallWith*)")]
        public bool MethodParameterChecksAreEnabled { get; set; } = true;

        [Category("Indexers")]
        [DisplayName("Basic Checks")]
        [Description("Whether to emit tests to exercise indexers (CanGet/Set*)")]
        public bool IndexerChecksAreEnabled { get; set; } = true;

        [Category("Properties")]
        [DisplayName("Basic Checks")]
        [Description("Whether to emit tests to exercise properties (CanGet/Set*)")]
        public bool PropertyChecksAreEnabled { get; set; } = true;

        [Category("Properties")]
        [DisplayName("Initialized Property Checks")]
        [Description("Whether to emit tests that check that properties have been initialized based on constructor parameter names (*IsInitializedCorrectly)")]
        public bool InitializedPropertyChecksAreEnabled { get; set; } = true;

        [Category("Operators")]
        [DisplayName("Basic Checks")]
        [Description("Whether to emit tests to exercise operators (CanCallOperator*)")]
        public bool OperatorChecksAreEnabled { get; set; } = true;

        [Category("Operators")]
        [DisplayName("Parameter Checks")]
        [Description("Whether to emit null and string parameter checks for operators (CannotCallOperatorWith*)")]
        public bool OperatorParameterChecksAreEnabled { get; set; } = true;

        [Category("Interfaces")]
        [DisplayName("Implementation Checks")]
        [Description("Whether to emit checks that verify interface implementation (Implements*)")]
        public bool InterfaceImplementationChecksAreEnabled { get; set; } = true;
    }
}
