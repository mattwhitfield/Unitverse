namespace Unitverse.Core.Options.Editing
{
    using System;

    public class ObjectItem
    {
        public ObjectItem(string text, object value)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            Text = text;
            Value = value;
        }

        public string Text { get; }

        public object Value { get; }
    }
}
