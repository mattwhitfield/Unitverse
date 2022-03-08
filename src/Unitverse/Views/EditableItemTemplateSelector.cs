using System.Windows;
using System.Windows.Controls;
using Unitverse.Core.Options.Editing;

namespace Unitverse.Views
{
    public class EditableItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is StringEditableItem)
            {
                return StringItemTemplate;
            }

            if (item is BooleanEditableItem)
            {
                return BooleanItemTemplate;
            }

            if (item is HeaderEditableItem)
            {
                return HeaderItemTemplate;
            }

            if (item is EnumEditableItem)
            {
                return EnumItemTemplate;
            }

            return null;
        }

        public DataTemplate BooleanItemTemplate { get; set; }

        public DataTemplate EnumItemTemplate { get; set; }

        public DataTemplate HeaderItemTemplate { get; set; }

        public DataTemplate StringItemTemplate { get; set; }

    }
}
