using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using Unitverse.Core.Templating.Model;

namespace Unitverse.Views
{
    public class FilterExpressionDebuggerViewModel : INotifyPropertyChanged
    {
        public FilterExpressionDebuggerViewModel(IOwningType filterableModel)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
