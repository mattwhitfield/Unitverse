namespace SentryOne.UnitTestGenerator.Core.Tests.Assets
{
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Assets;

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