using EnvDTE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitverse.Core.Options;

namespace Unitverse.Views
{
    public class GenerationDialogViewModel : INotifyPropertyChanged
    {
        private Project _sourceProject;
        private IUnitTestGeneratorOptions _projectOptions;

        public GenerationDialogViewModel(Project sourceProject, IUnitTestGeneratorOptions projectOptions)
        {
            _sourceProject = sourceProject;
            _projectOptions = projectOptions;

            Title = "Unitverse options for source project '" + sourceProject.Name + "'";

            Tabs.Add(new TabItem("Target Project", true, TabItemType.TargetProject));
            Tabs.Add(new TabItem("Generation Options", false, TabItemType.GenerationOptions));
            Tabs.Add(new TabItem("Strategy Options", false, TabItemType.StrategyOptions));
            Tabs.Add(new TabItem("Naming Options", false, TabItemType.NamingOptions));

            _selectedTab = Tabs.First();
        }

        public string Title { get; }

        private TabItem _selectedTab;

        public TabItem SelectedTab
        {
            get { return _selectedTab; }
            set 
            {
                if (_selectedTab != value)
                {
                    _selectedTab = value;
                    Tabs.ForEach(x => x.IsChecked = x == _selectedTab);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTab)));
                }
            }
        }


        public List<TabItem> Tabs { get; } = new List<TabItem>();
             
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
