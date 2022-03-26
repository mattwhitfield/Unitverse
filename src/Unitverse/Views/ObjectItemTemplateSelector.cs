using System.Windows;
using System.Windows.Controls;
using Unitverse.Core.Options.Editing;

namespace Unitverse.Views
{
    public class ObjectItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ObjectItem objectItem && objectItem.Value is null)
            {
                return DetachedTemplate;
            }

            return ProjectTemplate;
        }

        public DataTemplate ProjectTemplate { get; set; }

        public DataTemplate DetachedTemplate { get; set; }
    }
}
