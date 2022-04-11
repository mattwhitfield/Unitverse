namespace Unitverse.Core.Tests.Options.Editing
{
    using Unitverse.Core.Options.Editing;
    using System;
    using NUnit.Framework;
    using FluentAssertions;

    [TestFixture]
    public class EditableItemTests
    {
        private class TestEditableItem : EditableItem
        {
            public TestEditableItem(string text, string description, string fieldName) : base(text, description, fieldName, false, null)
            {
            }

            public override EditableItemType ItemType { get; }
        }

        private TestEditableItem _testClass;
        private string _text;
        private string _description;
        private string _fieldName;

        [SetUp]
        public void SetUp()
        {
            _text = "TestValue208749778";
            _description = "TestValue891155630";
            _fieldName = "TestValue1120685867";
            _testClass = new TestEditableItem(_text, _description, _fieldName);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new TestEditableItem(_text, _description, _fieldName);
            
            // Assert
            instance.Should().NotBeNull();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidText(string value)
        {
            FluentActions.Invoking(() => new TestEditableItem(value, "TestValue909945679", "TestValue861582538")).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidDescription(string value)
        {
            FluentActions.Invoking(() => new TestEditableItem("TestValue1557129476", value, "TestValue1255747775")).Should().Throw<ArgumentNullException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidFieldName(string value)
        {
            FluentActions.Invoking(() => new TestEditableItem("TestValue194899823", "TestValue1691620112", value)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void DescriptionIsInitializedCorrectly()
        {
            _testClass.Description.Should().Be(_description);
        }

        [Test]
        public void FieldNameIsInitializedCorrectly()
        {
            _testClass.FieldName.Should().Be(_fieldName);
        }
    }
}