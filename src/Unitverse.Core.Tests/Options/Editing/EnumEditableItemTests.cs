namespace Unitverse.Core.Tests.Options.Editing
{
    using Unitverse.Core.Options.Editing;
    using System;
    using NUnit.Framework;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class EnumEditableItemTests
    {
        private EnumEditableItem _testClass;
        private string _text;
        private string _description;
        private string _fieldName;
        private object _value;
        private Action<object> _setValue;
        private Type _enumerationType;

        private enum TestEnum
        {
            One,
            Two,
            Three
        }

        [SetUp]
        public void SetUp()
        {
            _text = "TestValue148648164";
            _description = "TestValue937678236";
            _fieldName = "TestValue194831954";
            _value = new object();
            _setValue = x => { };
            _enumerationType = typeof(TestEnum);
            _testClass = new EnumEditableItem(_text, _description, _fieldName, _value, _setValue, _enumerationType, false, null);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new EnumEditableItem(_text, _description, _fieldName, _value, _setValue, _enumerationType, false, null);
            
            // Assert
            instance.Should().NotBeNull();
        }

        [Test]
        public void CannotConstructWithNullSetValue()
        {
            FluentActions.Invoking(() => new EnumEditableItem("TestValue910415667", "TestValue1550196320", "TestValue1171641232", new object(), default(Action<object>), typeof(string), false, null)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CannotConstructWithNullEnumerationType()
        {
            FluentActions.Invoking(() => new EnumEditableItem("TestValue168000583", "TestValue397121609", "TestValue1525245590", new object(), x => { }, default(Type), false, null)).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidText(string value)
        {
            FluentActions.Invoking(() => new EnumEditableItem(value, "TestValue349423377", "TestValue628294131", new object(), x => { }, typeof(string), false, null)).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidDescription(string value)
        {
            FluentActions.Invoking(() => new EnumEditableItem("TestValue764192942", value, "TestValue743926900", new object(), x => { }, typeof(string), false, null)).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidFieldName(string value)
        {
            FluentActions.Invoking(() => new EnumEditableItem("TestValue1822449456", "TestValue386046142", value, new object(), x => { }, typeof(string), false, null)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CanGetItemType()
        {
            // Assert
            _testClass.ItemType.As<object>().Should().Be(EditableItemType.Enum);
        }

        [Test]
        public void CanGetItems()
        {
            // Assert
            _testClass.Items.Should().BeAssignableTo<List<ObjectItem>>();
            _testClass.Items.Select(x => x.Text).Should().BeEquivalentTo(new[] { "One", "Two", "Three" });
        }

        [Test]
        public void CanSetAndGetSelectedItem()
        {
            _testClass.CheckProperty(x => x.SelectedItem, new ObjectItem("TestValue1051881166", new object()), new ObjectItem("TestValue476984587", new object()));
        }
    }
}