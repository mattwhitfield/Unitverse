namespace Unitverse.Core.Tests.Options.Editing
{
    using Unitverse.Core.Options.Editing;
    using System;
    using NUnit.Framework;
    using FluentAssertions;

    [TestFixture]
    public class HeaderDisplayItemTests
    {
        private HeaderDisplayItem _testClass;
        private string _text;

        [SetUp]
        public void SetUp()
        {
            _text = "TestValue119783856";
            _testClass = new HeaderDisplayItem(_text);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new HeaderDisplayItem(_text);
            
            // Assert
            instance.Should().NotBeNull();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidText(string value)
        {
            FluentActions.Invoking(() => new HeaderDisplayItem(value)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CanGetItemType()
        {
            // Assert
            _testClass.ItemType.As<object>().Should().Be(EditableItemType.Header);
        }
    }
}