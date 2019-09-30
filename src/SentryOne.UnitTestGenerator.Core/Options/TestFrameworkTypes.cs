namespace SentryOne.UnitTestGenerator.Core.Options
{
    using System;
    using System.ComponentModel;

    [Flags]
    public enum TestFrameworkTypes
    {
        [Description("NUnit v2")]
        NUnit2 = 1,
        [Description("NUnit v3")]
        NUnit3 = 2,
        [Description("MSTest v2")]
        MsTest = 4,
        [Description("xUnit")]
        XUnit = 8,
    }
}