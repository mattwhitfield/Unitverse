namespace SentryOne.UnitTestGenerator.Core.Tests.Options
{
    using SentryOne.UnitTestGenerator.Core.Options;
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class TypeMemberMutatorTests
    {
        private class Target
        {
            public TestFrameworkTypes FrameworkType { get; set; }

            public Guid TargetGuid { get; set; }

            public string TargetString { get; set; }

            public int TargetInt { get; set; }
        }

        private TypeMemberMutator _testClass;
        private Type _targetType;

        [SetUp]
        public void SetUp()
        {
            _targetType = typeof(Target);
            _testClass = new TypeMemberMutator(_targetType);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new TypeMemberMutator(_targetType);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullTargetType()
        {
            Assert.Throws<ArgumentNullException>(() => new TypeMemberMutator(default(Type)));
        }

        [Test]
        public void CanCallSet()
        {
            var instance = new Target();
            _testClass.Set(instance, nameof(Target.FrameworkType), nameof(TestFrameworkTypes.MsTest));
            _testClass.Set(instance, nameof(Target.TargetGuid), "F371CA3F-3330-4975-9E13-2580CDDC89CD");
            _testClass.Set(instance, nameof(Target.TargetString), "someString");
            _testClass.Set(instance, nameof(Target.TargetInt), "13241");
            Assert.That(instance.FrameworkType, Is.EqualTo(TestFrameworkTypes.MsTest));
            Assert.That(instance.TargetGuid, Is.EqualTo(new Guid("F371CA3F-3330-4975-9E13-2580CDDC89CD")));
            Assert.That(instance.TargetString, Is.EqualTo("someString"));
            Assert.That(instance.TargetInt, Is.EqualTo(13241));
        }

        [Test]
        public void CannotCallSetWithNullInstance()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Set(default(object), "TestValue901544463", "TestValue570844002"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CanCallSetWithInvalidFieldName(string value)
        {
            Assert.DoesNotThrow(() => _testClass.Set(new object(), value, "TestValue325869245"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CanCallSetWithInvalidFieldValue(string value)
        {
            Assert.DoesNotThrow(() => _testClass.Set(new object(), "TestValue606729554", value));
        }
    }
}