namespace Unitverse.Core.Tests.Options.Editing
{
    using Unitverse.Core.Options.Editing;
    using System;
    using NUnit.Framework;
    using FluentAssertions;

    [TestFixture]
    public class BooleanEditableItemTests
    {
        private BooleanEditableItem _testClass;
        private string _text;
        private string _description;
        private string _fieldName;
        private bool _value;
        private Action<bool> _setValue;

        [SetUp]
        public void SetUp()
        {
            _text = "TestValue1943036796";
            _description = "TestValue1597670454";
            _fieldName = "TestValue1029729535";
            _value = false;
            _setValue = x => { };
            _testClass = new BooleanEditableItem(_text, _description, _fieldName, _value, _setValue, false, null);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new BooleanEditableItem(_text, _description, _fieldName, _value, _setValue, false, null);
            
            // Assert
            instance.Should().NotBeNull();
        }

        [Test]
        public void CannotConstructWithNullSetValue()
        {
            FluentActions.Invoking(() => new BooleanEditableItem("TestValue26046281", "TestValue994223939", "TestValue1320835479", false, default(Action<bool>), false, null)).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidText(string value)
        {
            FluentActions.Invoking(() => new BooleanEditableItem(value, "TestValue972041960", "TestValue1741199683", false, x => { }, false, null)).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidDescription(string value)
        {
            FluentActions.Invoking(() => new BooleanEditableItem("TestValue331535449", value, "TestValue1221071621", true, x => { }, false, null)).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidFieldName(string value)
        {
            FluentActions.Invoking(() => new BooleanEditableItem("TestValue770458689", "TestValue2127094014", value, false, x => { }, false, null)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CanGetItemType()
        {
            // Assert
            _testClass.ItemType.As<object>().Should().Be(EditableItemType.Boolean);
        }

        [Test]
        public void ValueIsInitializedCorrectly()
        {
            _testClass.Value.Should().Be(_value);
        }

        [Test]
        public void CanSetAndGetValue()
        {
            _testClass.CheckProperty(x => x.Value);
        }
    }
}