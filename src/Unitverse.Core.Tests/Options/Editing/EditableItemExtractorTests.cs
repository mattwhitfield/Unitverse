namespace Unitverse.Core.Tests.Options.Editing
{
    using Unitverse.Core.Options.Editing;
    using System;
    using NUnit.Framework;
    using FluentAssertions;
    using System.Linq;
    using Unitverse.Core.Options;

    [TestFixture]
    public class EditableItemExtractorTests
    {
        [Test]
        public void CanCallExtractFrom()
        {
            // Arrange
            var source = new GenerationOptions();
            source.ActComment = "freddo";
            var modifiableSource = new MutableGenerationOptions(source);

            // Act
            var result = EditableItemExtractor.ExtractFrom(source, modifiableSource).ToList();
            
            // Assert
            result.Should().Contain(x => x.ItemType == EditableItemType.String && x.Text == "Act block comment" && x.Description == "The comment to leave before any act statements (leave blank to suppress)" && x is StringEditableItem && ((StringEditableItem)x).Value == "freddo");
        }

        [Test]
        public void CannotCallExtractFromWithNullSource()
        {
            FluentActions.Invoking(() => EditableItemExtractor.ExtractFrom(default(object), new object())).Should().Throw<ArgumentNullException>();
            FluentActions.Invoking(() => EditableItemExtractor.ExtractFrom(new object(), default(object))).Should().Throw<ArgumentNullException>();
        }
    }
}