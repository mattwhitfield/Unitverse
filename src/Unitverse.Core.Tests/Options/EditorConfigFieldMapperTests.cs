namespace Unitverse.Core.Tests.Options
{
    using System;
    using System.Collections.Generic;
    using NSubstitute;
    using NUnit.Framework;
    using Unitverse.Core.Options;
    using FluentAssertions;
    using System.Reflection;
    using System.Linq;

    [TestFixture]
    public static class EditorConfigFieldMapperTests
    {
        [Test]
        public static void CanCallApplyTo()
        {
            var fileConfiguration = new Dictionary<string, string> { { "test_project_naming", "SomeProject{0}" } };
            var target = new MutableGenerationOptions(Substitute.For<IGenerationOptions>());
            fileConfiguration.ApplyTo(target);
            Assert.That(target.TestProjectNaming, Is.EqualTo("SomeProject{0}"));
        }

        [Test]
        public static void CannotCallApplyToWithNullFileConfiguration()
        {
            var target = new MutableGenerationOptions(Substitute.For<IGenerationOptions>());
            Assert.Throws<ArgumentNullException>(() => default(Dictionary<string, string>).ApplyTo(target));
        }

        [Test]
        public static void CannotCallApplyToWithNullTarget()
        {
            Assert.Throws<ArgumentNullException>(() => new Dictionary<string, string>().ApplyTo(default(MutableGenerationOptions)));
        }

        [Test]
        public static void CanCallCreateMutatorSet()
        {
            // Act
            var result = EditorConfigFieldMapper.CreateMutatorSet<MutableGenerationOptions>();
            var second = EditorConfigFieldMapper.CreateMutatorSet<MutableGenerationOptions>();

            // Assert
            foreach (var member in typeof(MutableGenerationOptions).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanWrite))
            {
                result.Should().ContainKey(member.Name);
            }

            second.Should().BeSameAs(result);
        }
    }
}