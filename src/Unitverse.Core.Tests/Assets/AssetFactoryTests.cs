namespace Unitverse.Core.Tests.Assets
{
    using NUnit.Framework;
    using Unitverse.Core.Assets;

    [TestFixture]
    public static class AssetFactoryTests
    {
        [Test]
        public static void CanCallCreate()
        {
            var assetType = TargetAsset.PropertyTester;
            var result = AssetFactory.Create(assetType);
            Assert.That(result, Is.InstanceOf<IAsset>());
        }
    }
}