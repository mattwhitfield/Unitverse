namespace Unitverse.Core.Assets
{
    using Unitverse.Core.Options;

    public interface IAsset
    {
        string AssetFileName { get; }

        string Content(string targetNamespace, TestFrameworkTypes testFrameworkTypes);
    }
}