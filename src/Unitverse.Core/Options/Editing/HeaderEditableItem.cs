namespace Unitverse.Core.Options.Editing
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class HeaderEditableItem : EditableItem
    {
        public HeaderEditableItem(string text)
            : base(text, string.Empty, string.Empty)
        {
        }

        public override EditableItemType ItemType => EditableItemType.Header;
    }
}
