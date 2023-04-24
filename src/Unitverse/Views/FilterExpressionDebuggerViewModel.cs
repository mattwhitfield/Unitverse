using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unitverse.Core.Options.Editing;
using Unitverse.Core.Templating.Model;

namespace Unitverse.Views
{
    public class FilterExpressionDebuggerViewModel : ViewModelBase
    {
        private Dictionary<string, object> _filterTarget = new Dictionary<string, object>();

        public FilterExpressionDebuggerViewModel(IOwningType filterableModel)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var generationItems = filterableModel.Methods.OfType<ITemplateTarget>().Concat(
                filterableModel.Properties).Concat(
                filterableModel.Constructors).Where(x => x.ShouldGenerate).ToList();

            if (generationItems.Count == 1)
            {
                Models.Add(new ObjectViewModel(generationItems[0], "model"));
                _filterTarget["model"] = generationItems[0];
            }

            Models.Add(new ObjectViewModel(filterableModel, "owningType"));
            _filterTarget["owningType"] = filterableModel;
        }

        public ObservableCollection<ObjectViewModel> Models { get; } = new ObservableCollection<ObjectViewModel>();

        private string _filterExpression;

        public string FilterExpression
        {
            get { return _filterExpression; }
            set
            {
                if (_filterExpression != value)
                {
                    _filterExpression = value;
                    RaisePropertyChanged(nameof(FilterExpression));
                }
            }
        }

        public ObservableCollection<PropertyViewModel> Properties { get; } = new ObservableCollection<PropertyViewModel>();
    }
}
