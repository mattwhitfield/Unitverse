namespace Unitverse.Core.Options
{
    using System.ComponentModel;

    public enum MockingFrameworkType
    {
        [Description("NSubstitute")]
        NSubstitute,
        [Description("Moq")]
        Moq,
        [Description("FakeItEasy")]
        FakeItEasy,
        [Description("Moq with AutoMock")]
        MoqAutoMock,
        [Description("JustMock")]
        JustMock,
    }
}