namespace Unitverse.Core.Options
{
    using System.ComponentModel;

    public enum UserInterfaceModes
    {
        [Description("Only when the Control key is pressed")]
        OnlyWhenControlPressed,
        [Description("Whenever a target project is not found or the Control key is pressed")]
        WhenTargetNotFound,
        [Description("Always")]
        Always,
    }
}
