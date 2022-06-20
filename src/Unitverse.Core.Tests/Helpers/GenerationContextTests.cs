namespace Unitverse.Core.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Helpers;
    using Unitverse.Core.Options;

    [TestFixture]
    public class GenerationContextTests
    {
        private GenerationContext _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new GenerationContext(Substitute.For<IGenerationOptions>());
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new GenerationContext(Substitute.For<IGenerationOptions>());
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CanCallAddEmittedType()
        {
            var typeInfo = TestSemanticModelFactory.Model.GetDeclaredSymbol(TestSemanticModelFactory.Class) as ITypeSymbol;
            _testClass.AddEmittedType(typeInfo);
            Assert.That(_testClass.EmittedTypes.Contains(typeInfo));
        }

        [Test]
        public void CannotCallAddEmittedTypeWithNullTypeInfo()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.AddEmittedType(default(ITypeSymbol)));
        }

        [Test]
        public void CanGetEmittedTypes()
        {
            Assert.That(_testClass.EmittedTypes, Is.InstanceOf<IEnumerable<ITypeSymbol>>());
        }

        [Test]
        public void CanSetAndGetMocksUsed()
        {
            Assert.That(_testClass.MocksUsed, Is.InstanceOf<bool>());
            _testClass.MocksUsed = true;
            Assert.That(_testClass.MocksUsed, Is.EqualTo(true));
        }
    }
}