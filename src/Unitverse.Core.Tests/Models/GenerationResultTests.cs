namespace Unitverse.Core.Tests.Models
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Unitverse.Core.Assets;
    using Unitverse.Core.Models;

    [TestFixture]
    public class GenerationResultTests
    {
        private GenerationResult _testClass;
        private string _fileContent;

        [SetUp]
        public void SetUp()
        {
            _fileContent = "TestValue1767881884";
            _testClass = new GenerationResult(_fileContent);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new GenerationResult(_fileContent);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void FileContentIsInitializedCorrectly()
        {
            Assert.That(_testClass.FileContent, Is.EqualTo(_fileContent));
        }

        [Test]
        public void CanGetRequiredAssets()
        {
            Assert.That(_testClass.RequiredAssets, Is.InstanceOf<IList<TargetAsset>>());
        }
    }
}