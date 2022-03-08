namespace Unitverse.Core.Tests.Options.Editing
{
    using Unitverse.Core.Options.Editing;
    using System;
    using NUnit.Framework;
    using FluentAssertions;

    [TestFixture]
    public class TabItemTests
    {
        private TabItem _testClass;
        private string _text;
        private bool _isChecked;
        private TabItemType _itemType;

        [SetUp]
        public void SetUp()
        {
            _text = "TestValue1207706099";
            _isChecked = false;
            _itemType = TabItemType.NamingOptions;
            _testClass = new TabItem(_text, _isChecked, _itemType);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new TabItem(_text, _isChecked, _itemType);
            
            // Assert
            instance.Should().NotBeNull();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidText(string value)
        {
            FluentActions.Invoking(() => new TabItem(value, false, TabItemType.TargetProject)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void TextIsInitializedCorrectly()
        {
            _testClass.Text.Should().Be(_text);
        }

        [Test]
        public void ItemTypeIsInitializedCorrectly()
        {
            _testClass.ItemType.Should().Be(_itemType);
        }

        [Test]
        public void IsCheckedIsInitializedCorrectly()
        {
            _testClass.IsChecked.Should().Be(_isChecked);
        }

        [Test]
        public void CanSetAndGetIsChecked()
        {
            _testClass.CheckProperty(x => x.IsChecked);
        }
    }
}