namespace Unitverse.Core.Options
{
    using System.ComponentModel;

    public enum FallbackTargetFindingMethod
    {
        [Description("None")]
        None = 0,

        [Description("Find the correct type name in any namespace")]
        TypeInAnyNamespace,

        [Description("Find the correct type name in the correct namespace")]
        TypeInCorrectNamespace,
    }
}