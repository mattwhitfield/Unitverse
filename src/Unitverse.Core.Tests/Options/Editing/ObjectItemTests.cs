namespace Unitverse.Core.Tests.Options.Editing
{
    using Unitverse.Core.Options.Editing;
    using System;
    using NUnit.Framework;
    using FluentAssertions;

    [TestFixture]
    public class ObjectItemTests
    {
        private ObjectItem _testClass;
        private string _text;
        private object _value;

        [SetUp]
        public void SetUp()
        {
            _text = "TestValue7921824";
            _value = new object();
            _testClass = new ObjectItem(_text, _value);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new ObjectItem(_text, _value);
            
            // Assert
            instance.Should().NotBeNull();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidText(string value)
        {
            FluentActions.Invoking(() => new ObjectItem(value, new object())).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void TextIsInitializedCorrectly()
        {
            _testClass.Text.Should().Be(_text);
        }

        [Test]
        public void ValueIsInitializedCorrectly()
        {
            _testClass.Value.Should().BeSameAs(_value);
        }
    }
}