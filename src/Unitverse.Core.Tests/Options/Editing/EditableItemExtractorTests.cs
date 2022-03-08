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
            result.Should().Contain(x => x.ItemType == EditableItemType.String && x.Text == "Act block comment" && x is StringEditableItem && ((StringEditableItem)x).Description == "The comment to leave before any act statements (leave blank to suppress)" && ((StringEditableItem)x).Value == "freddo");

            foreach (var property in typeof(IGenerationOptions).GetProperties())
            {
                if (property.Name == nameof(IGenerationOptions.AllowGenerationWithoutTargetProject) ||
                    property.Name == nameof(IGenerationOptions.AutoDetectFrameworkTypes) ||
                    property.Name == nameof(IGenerationOptions.TestProjectNaming) ||
                    property.Name == nameof(IGenerationOptions.RememberManuallySelectedTargetProjectByDefault) ||
                    property.Name == nameof(IGenerationOptions.UserInterfaceMode))
                {
                    // ignore properties that are excluded from the UI
                    continue;
                }

                var propertyName = property.Name;
                if (property.PropertyType == typeof(string))
                {
                    result.Should().Contain(x => x.ItemType == EditableItemType.String && x is EditableItem && ((EditableItem)x).FieldName == propertyName, propertyName);
                }
                else if (property.PropertyType == typeof(bool))
                {
                    result.Should().Contain(x => x.ItemType == EditableItemType.Boolean && x is EditableItem && ((EditableItem)x).FieldName == propertyName, propertyName);
                }
                else if (property.PropertyType.IsEnum)
                {
                    result.Should().Contain(x => x.ItemType == EditableItemType.Enum && x is EditableItem && ((EditableItem)x).FieldName == propertyName, propertyName);
                }

                result.Should().NotContain(x => x is EditableItem && ((EditableItem)x).FieldName == nameof(IGenerationOptions.AllowGenerationWithoutTargetProject));
                result.Should().NotContain(x => x is EditableItem && ((EditableItem)x).FieldName == nameof(IGenerationOptions.AutoDetectFrameworkTypes));
                result.Should().NotContain(x => x is EditableItem && ((EditableItem)x).FieldName == nameof(IGenerationOptions.RememberManuallySelectedTargetProjectByDefault));
                result.Should().NotContain(x => x is EditableItem && ((EditableItem)x).FieldName == nameof(IGenerationOptions.UserInterfaceMode));
                result.Should().NotContain(x => x is EditableItem && ((EditableItem)x).FieldName == nameof(IGenerationOptions.TestProjectNaming));
            }
        }

        [Test]
        public void CannotCallExtractFromWithNullSource()
        {
            FluentActions.Invoking(() => EditableItemExtractor.ExtractFrom(default(object), new object()).ToList()).Should().Throw<ArgumentNullException>();
            FluentActions.Invoking(() => EditableItemExtractor.ExtractFrom(new object(), default(object)).ToList()).Should().Throw<ArgumentNullException>();
        }
    }
}