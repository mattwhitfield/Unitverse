namespace Unitverse.Core.Options.Editing
{
    using System;
    using System.ComponentModel;

    public abstract class EditableItem : DisplayItem
    {
        public EditableItem(string text, string description, string fieldName, bool showSourceIcon, string sourceFileName)
            : base(text, showSourceIcon, sourceFileName)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            if (string.IsNullOrWhiteSpace(fieldName))
            {
                throw new ArgumentNullException(nameof(fieldName));
            }

            Description = description;
            FieldName = fieldName;
        }

        public string Description { get; }

        public string FieldName { get; }
    }
}
