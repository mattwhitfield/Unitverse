namespace Unitverse.Core.Options.Editing
{
    public class HeaderDisplayItem : DisplayItem
    {
        public HeaderDisplayItem(string text)
            : base(text, false, null)
        {
        }

        public override EditableItemType ItemType => EditableItemType.Header;
    }
}
