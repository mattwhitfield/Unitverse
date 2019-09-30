namespace SentryOne.UnitTestGenerator.Core.Assets
{
    using SentryOne.UnitTestGenerator.Core.Options;

    public interface IAsset
    {
        string AssetFileName { get; }

        string Content(string targetNamespace, TestFrameworkTypes testFrameworkTypes);
    }
}