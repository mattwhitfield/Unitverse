using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace Unitverse.Views
{
    public class TabItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Unitverse.Views.TabItem tabItem)
            {
                return tabItem.IsChecked ? CheckedTemplate : UncheckedTemplate;
            }

            return null;
        }

        public DataTemplate CheckedTemplate { get; set; }
        public DataTemplate UncheckedTemplate { get; set; }
    }
}