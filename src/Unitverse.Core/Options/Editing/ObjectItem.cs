namespace Unitverse.Core.Options.Editing
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class ObjectItem
    {
        public ObjectItem(string text, object value)
        {
            Text = text;
            Value = value;
        }

        public string Text { get; }

        public object Value { get; }
    }
}
