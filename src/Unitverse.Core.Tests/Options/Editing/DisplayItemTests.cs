namespace Unitverse.Core.Tests.Options.Editing
{
    using Unitverse.Core.Options.Editing;
    using System;
    using NUnit.Framework;
    using FluentAssertions;
    using Unitverse.Core.Options;

    [TestFixture]
    public class DisplayItemTests
    {
        private class TestDisplayItem : DisplayItem
        {
            public TestDisplayItem(string text, bool showSourceIcon, ConfigurationSource source) : base(text, showSourceIcon, source)
            {
            }

            public void PublicRaisePropertyChanged(string propertyName)
            {
                base.RaisePropertyChanged(propertyName);
            }

            public override EditableItemType ItemType { get; }
        }

        private TestDisplayItem _testClass;
        private string _text;

        [SetUp]
        public void SetUp()
        {
            _text = "TestValue1727859796";
            _testClass = new TestDisplayItem(_text, false, null);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new TestDisplayItem(_text, false, null);
            
            // Assert
            instance.Should().NotBeNull();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidText(string value)
        {
            FluentActions.Invoking(() => new TestDisplayItem(value, false, null)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void CanCallRaisePropertyChanged()
        {
            // Arrange
            var propertyName = "TestValue1388136569";
            string received = null;

            _testClass.PropertyChanged += (sender, args) => received = args.PropertyName;

            // Act
            _testClass.PublicRaisePropertyChanged(propertyName);
            
            // Assert
            received.Should().Be(propertyName);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallRaisePropertyChangedWithInvalidPropertyName(string value)
        {
            FluentActions.Invoking(() => _testClass.PublicRaisePropertyChanged(value)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void TextIsInitializedCorrectly()
        {
            _testClass.Text.Should().Be(_text);
        }

        [Test]
        public void ShowVsConfigSourceIsInitializedCorrectly()
        {
            _testClass.ShowVsConfigSource.Should().Be(false);
            _testClass = new TestDisplayItem(_text, true, new ConfigurationSource(ConfigurationSourceType.VisualStudio));
            _testClass.ShowVsConfigSource.Should().Be(true);
        }

        [Test]
        public void ShowFileConfigSourceeIsInitializedCorrectly()
        {
            _testClass.ShowFileConfigSource.Should().Be(false);
            _testClass = new TestDisplayItem(_text, true, new ConfigurationSource(ConfigurationSourceType.ConfigurationFile, "some.file"));
            _testClass.ShowFileConfigSource.Should().Be(true);
        }

        [Test]
        public void ShowAutoDetectionSourceIsInitializedCorrectly()
        {
            _testClass.ShowAutoDetectionSource.Should().Be(false);
            _testClass = new TestDisplayItem(_text, true, new ConfigurationSource(ConfigurationSourceType.AutoDetection));
            _testClass.ShowAutoDetectionSource.Should().Be(true);
        }

        [Test]
        public void ShowSessionConfigSourceIsInitializedCorrectly()
        {
            _testClass.ShowSessionConfigSource.Should().Be(false);
            _testClass = new TestDisplayItem(_text, true, new ConfigurationSource(ConfigurationSourceType.Session));
            _testClass.ShowSessionConfigSource.Should().Be(true);
        }

        [Test]
        public void DoubleAmpersandInTextIsReplaced()
        {
            _text = "Something && Something Else";
            _testClass = new TestDisplayItem(_text, false, null);
            _testClass.Text.Should().Be("Something & Something Else");
        }
    }
}