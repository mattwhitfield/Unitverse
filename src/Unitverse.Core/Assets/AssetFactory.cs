namespace Unitverse.Core.Assets
{
    public static class AssetFactory
    {
        public static IAsset? Create(TargetAsset assetType)
        {
            return assetType == TargetAsset.PropertyTester ? new PropertyTesterAsset() : null;
        }
    }
}
