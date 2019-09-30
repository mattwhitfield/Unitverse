namespace SentryOne.UnitTestGenerator.Core.Tests.Options
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using SentryOne.UnitTestGenerator.Core.Options;

    [TestFixture]
    public class UnitTestGeneratorOptionsTests
    {
        private UnitTestGeneratorOptions _testClass;
        private IGenerationOptions _generationOptions;
        private IVersioningOptions _versioningOptions;

        [SetUp]
        public void SetUp()
        {
            _generationOptions = Substitute.For<IGenerationOptions>();
            _versioningOptions = Substitute.For<IVersioningOptions>();
            _testClass = new UnitTestGeneratorOptions(_generationOptions, _versioningOptions);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new UnitTestGeneratorOptions(_generationOptions, _versioningOptions);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullGenerationOptions()
        {
            Assert.Throws<ArgumentNullException>(() => new UnitTestGeneratorOptions(default(IGenerationOptions), Substitute.For<IVersioningOptions>()));
        }

        [Test]
        public void CannotConstructWithNullVersioningOptions()
        {
            Assert.Throws<ArgumentNullException>(() => new UnitTestGeneratorOptions(Substitute.For<IGenerationOptions>(), default(IVersioningOptions)));
        }

        [Test]
        public void GenerationOptionsIsInitializedCorrectly()
        {
            Assert.That(_testClass.GenerationOptions, Is.EqualTo(_generationOptions));
        }

        [Test]
        public void VersioningOptionsIsInitializedCorrectly()
        {
            Assert.That(_testClass.VersioningOptions, Is.EqualTo(_versioningOptions));
        }
    }
}