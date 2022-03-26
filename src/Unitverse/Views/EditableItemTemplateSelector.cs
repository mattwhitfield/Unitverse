using System.Windows;
using System.Windows.Controls;
using Unitverse.Core.Options.Editing;

namespace Unitverse.Views
{
    public class EditableItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is DisplayItem displayItem)
            {
                switch (displayItem.ItemType)
                {
                    case EditableItemType.Header:
                        return HeaderItemTemplate;
                    case EditableItemType.String:
                        return StringItemTemplate;
                    case EditableItemType.Boolean:
                        return BooleanItemTemplate;
                    case EditableItemType.Enum:
                        return EnumItemTemplate;
                }    
            }

            return null;
        }

        public DataTemplate BooleanItemTemplate { get; set; }

        public DataTemplate EnumItemTemplate { get; set; }

        public DataTemplate HeaderItemTemplate { get; set; }

        public DataTemplate StringItemTemplate { get; set; }
    }
}
