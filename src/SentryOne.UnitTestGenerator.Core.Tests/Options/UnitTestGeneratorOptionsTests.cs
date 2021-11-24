namespace Unitverse.Core.Tests.Options
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Options;

    [TestFixture]
    public class UnitTestGeneratorOptionsTests
    {
        private UnitTestGeneratorOptions _testClass;
        private IGenerationOptions _generationOptions;

        [SetUp]
        public void SetUp()
        {
            _generationOptions = Substitute.For<IGenerationOptions>();
            _testClass = new UnitTestGeneratorOptions(_generationOptions);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new UnitTestGeneratorOptions(_generationOptions);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullGenerationOptions()
        {
            Assert.Throws<ArgumentNullException>(() => new UnitTestGeneratorOptions(default(IGenerationOptions)));
        }

        [Test]
        public void GenerationOptionsIsInitializedCorrectly()
        {
            Assert.That(_testClass.GenerationOptions, Is.EqualTo(_generationOptions));
        }
    }
}