using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unitverse.Core.Options;
using Unitverse.Core.Options.Editing;
using Unitverse.Options;

namespace Unitverse.Views
{
    public class ConfigEditorControlViewModel : INotifyPropertyChanged
    {
        public ConfigEditorControlViewModel(IUnitTestGeneratorPackage package, string fileName, Action onModified)
        {
            var baseOptions = package.Options;
            var projectOptions = UnitTestGeneratorOptionsFactory.Create(fileName, baseOptions.GenerationOptions, baseOptions.NamingOptions, baseOptions.StrategyOptions, baseOptions.StatisticsCollectionEnabled);

            Tabs.Add(new TabItem("Generation Options", false, TabItemType.GenerationOptions));
            Tabs.Add(new TabItem("Strategy Options", false, TabItemType.StrategyOptions));
            Tabs.Add(new TabItem("Naming Options", false, TabItemType.NamingOptions));

            _selectedTab = Tabs.First();

            var generationOptions = new MutableGenerationOptions(projectOptions.GenerationOptions);
            var strategyOptions = new MutableStrategyOptions(projectOptions.StrategyOptions);
            var namingOptions = new MutableNamingOptions(projectOptions.NamingOptions);

            GenerationOptionsItems = EditableItemExtractor.ExtractFrom(new GenerationOptions(), generationOptions).ToList();
            StrategyOptionsItems = EditableItemExtractor.ExtractFrom(new StrategyOptions(), strategyOptions).ToList();
            NamingOptionsItems = EditableItemExtractor.ExtractFrom(new NamingOptions(), namingOptions).ToList();

            Options = new UnitTestGeneratorOptions(generationOptions, namingOptions, strategyOptions, false);

            foreach (var item in GenerationOptionsItems.Concat(StrategyOptionsItems).Concat(NamingOptionsItems))
            {
                item.PropertyChanged += Item_PropertyChanged;
            }

            _onModified = onModified;
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _onModified();
        }

        private readonly Action _onModified;

        public UnitTestGeneratorOptions Options { get; }


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

        public IList<DisplayItem> GenerationOptionsItems { get; }

        public IList<DisplayItem> StrategyOptionsItems { get; }

        public IList<DisplayItem> NamingOptionsItems { get; }

        public List<TabItem> Tabs { get; } = new List<TabItem>();

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
