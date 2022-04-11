namespace Unitverse.Core.Tests.Options.Editing
{
    using Unitverse.Core.Options.Editing;
    using System;
    using NUnit.Framework;
    using FluentAssertions;

    [TestFixture]
    public class StringEditableItemTests
    {
        private StringEditableItem _testClass;
        private string _text;
        private string _description;
        private string _fieldName;
        private string _value;
        private Action<string> _setValue;

        [SetUp]
        public void SetUp()
        {
            _text = "TestValue1885959811";
            _description = "TestValue1125935656";
            _fieldName = "TestValue2012998739";
            _value = "TestValue219560791";
            _setValue = x => { };
            _testClass = new StringEditableItem(_text, _description, _fieldName, _value, _setValue, false, null);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new StringEditableItem(_text, _description, _fieldName, _value, _setValue, false, null);
            
            // Assert
            instance.Should().NotBeNull();
        }

        [Test]
        public void CannotConstructWithNullSetValue()
        {
            FluentActions.Invoking(() => new StringEditableItem("TestValue531486156", "TestValue1498720570", "TestValue1541340860", "TestValue1026239667", default(Action<string>), false, null)).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidText(string value)
        {
            FluentActions.Invoking(() => new StringEditableItem(value, "TestValue1906171625", "TestValue1704187850", "TestValue96324715", x => { }, false, null)).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidDescription(string value)
        {
            FluentActions.Invoking(() => new StringEditableItem("TestValue1878384096", value, "TestValue1737143007", "TestValue817444129", x => { }, false, null)).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidFieldName(string value)
        {
            FluentActions.Invoking(() => new StringEditableItem("TestValue1985423896", "TestValue1184592193", value, "TestValue1512639607", x => { }, false, null)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CanGetItemType()
        {
            // Assert
            _testClass.ItemType.As<object>().Should().Be(EditableItemType.String);
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