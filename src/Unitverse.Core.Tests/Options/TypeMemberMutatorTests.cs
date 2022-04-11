namespace Unitverse.Core.Tests.Options
{
    using Unitverse.Core.Options;
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

        [Test]
        public void CanCallSet()
        {
            var instance = new Target();
            TypeMemberMutator.Set(instance, typeof(Target).GetProperty(nameof(Target.FrameworkType)), nameof(TestFrameworkTypes.MsTest));
            TypeMemberMutator.Set(instance, typeof(Target).GetProperty(nameof(Target.TargetGuid)), "F371CA3F-3330-4975-9E13-2580CDDC89CD");
            TypeMemberMutator.Set(instance, typeof(Target).GetProperty(nameof(Target.TargetString)), "someString");
            TypeMemberMutator.Set(instance, typeof(Target).GetProperty(nameof(Target.TargetInt)), "13241");
            Assert.That(instance.FrameworkType, Is.EqualTo(TestFrameworkTypes.MsTest));
            Assert.That(instance.TargetGuid, Is.EqualTo(new Guid("F371CA3F-3330-4975-9E13-2580CDDC89CD")));
            Assert.That(instance.TargetString, Is.EqualTo("someString"));
            Assert.That(instance.TargetInt, Is.EqualTo(13241));
        }

        [Test]
        public void CannotCallSetWithNullInstance()
        {
            Assert.Throws<ArgumentNullException>(() => TypeMemberMutator.Set(default(object), typeof(Target).GetProperty(nameof(Target.FrameworkType)), "TestValue570844002"));
        }

        [Test]
        public void CannotCallSetWithNullMember()
        {
            Assert.Throws<ArgumentNullException>(() => TypeMemberMutator.Set(new object(), null, "TestValue325869245"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CanCallSetWithInvalidFieldValue(string value)
        {
            Assert.DoesNotThrow(() => TypeMemberMutator.Set(new object(), typeof(Target).GetProperty(nameof(Target.FrameworkType)), value));
        }
    }
}