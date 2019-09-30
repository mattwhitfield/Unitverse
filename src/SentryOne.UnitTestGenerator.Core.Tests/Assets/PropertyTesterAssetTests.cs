namespace SentryOne.UnitTestGenerator.Core.Tests.Assets
{
    using System;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Assets;
    using SentryOne.UnitTestGenerator.Core.Options;

    [TestFixture]
    public class PropertyTesterAssetTests
    {
        private PropertyTesterAsset _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new PropertyTesterAsset();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new PropertyTesterAsset();
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        [TestCase(TestFrameworkTypes.NUnit2)]
        [TestCase(TestFrameworkTypes.NUnit3)]
        [TestCase(TestFrameworkTypes.XUnit)]
        [TestCase(TestFrameworkTypes.MsTest)]
        public void CanCallContent(TestFrameworkTypes frameworkTypes)
        {
            var targetNamespace = "TestValue927446416";
            var result = _testClass.Content(targetNamespace, frameworkTypes);
            Assert.That(result, Contains.Substring(targetNamespace));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallContentWithInvalidTargetNamespace(string value)
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Content(value, default(TestFrameworkTypes)));
        }

        [Test]
        public void CanGetAssetFileName()
        {
            Assert.That(_testClass.AssetFileName, Is.EqualTo("PropertyTester.cs"));
        }
    }
}