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
        private INamingOptions _namingOptions;

        [SetUp]
        public void SetUp()
        {
            _generationOptions = Substitute.For<IGenerationOptions>();
            _namingOptions = Substitute.For<INamingOptions>();
            _testClass = new UnitTestGeneratorOptions(_generationOptions, _namingOptions);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new UnitTestGeneratorOptions(_generationOptions, _namingOptions);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullGenerationOptions()
        {
            Assert.Throws<ArgumentNullException>(() => new UnitTestGeneratorOptions(default(IGenerationOptions), Substitute.For<INamingOptions>()));
        }

        [Test]
        public void CannotConstructWithNullNamingOptions()
        {
            Assert.Throws<ArgumentNullException>(() => new UnitTestGeneratorOptions(Substitute.For<IGenerationOptions>(), default(INamingOptions)));
        }

        [Test]
        public void GenerationOptionsIsInitializedCorrectly()
        {
            Assert.That(_testClass.GenerationOptions, Is.EqualTo(_generationOptions));
        }
    }
}