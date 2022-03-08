namespace Unitverse.Core.Options.Editing
{
    public class HeaderDisplayItem : DisplayItem
    {
        public HeaderDisplayItem(string text)
            : base(text)
        {
        }

        public override EditableItemType ItemType => EditableItemType.Header;
    }
}
