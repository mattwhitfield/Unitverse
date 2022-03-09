using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unitverse.Core.Options;
using Unitverse.Core.Options.Editing;
using Unitverse.Helper;
using Unitverse.Options;
using Unitverse.Core.Helpers;
using System.Windows.Data;
using System;

namespace Unitverse.Views
{
    public class GenerationDialogViewModel : INotifyPropertyChanged
    {
        private MutableGenerationOptions _generationOptions;

        public GenerationDialogViewModel(Project sourceProject, IUnitTestGeneratorOptions projectOptions)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Title = "Unitverse options for source project '" + sourceProject.Name + "'";

            Tabs.Add(new TabItem("Target Project", true, TabItemType.TargetProject));
            Tabs.Add(new TabItem("Generation Options", false, TabItemType.GenerationOptions));
            Tabs.Add(new TabItem("Strategy Options", false, TabItemType.StrategyOptions));
            Tabs.Add(new TabItem("Naming Options", false, TabItemType.NamingOptions));

            _selectedTab = Tabs.First();

            var generationOptions = new MutableGenerationOptions(projectOptions.GenerationOptions);
            var strategyOptions = new MutableStrategyOptions(projectOptions.StrategyOptions);
            var namingOptions = new MutableNamingOptions(projectOptions.NamingOptions);

            _generationOptions = new MutableGenerationOptions(generationOptions);

            GenerationOptionsItems = EditableItemExtractor.ExtractFrom(new GenerationOptions(), generationOptions).ToList();
            StrategyOptionsItems = EditableItemExtractor.ExtractFrom(new StrategyOptions(), strategyOptions).ToList();
            NamingOptionsItems = EditableItemExtractor.ExtractFrom(new NamingOptions(), namingOptions).ToList();

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
            foreach (var project in VsProjectHelper.FindProjects(sourceProject.DTE.Solution).Where(x => x.UniqueName != sourceProject.UniqueName).OrderBy(x => x.Name))
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
            {
                Projects.Add(new ObjectItem(project.Name, project)); 
            }

            var targetProjectName = projectOptions.GenerationOptions.GetTargetProjectName(sourceProject.Name);
            var sessionSelectedProject = TargetSelectionRegister.Instance.GetTargetFor(sourceProject.UniqueName);
            var resolvedTarget = string.IsNullOrWhiteSpace(sessionSelectedProject) ? targetProjectName : sessionSelectedProject;

            _selectedProject = Projects.FirstOrDefault(x => string.Equals(x.Text, resolvedTarget, System.StringComparison.OrdinalIgnoreCase));
            var selectedProject = _selectedProject?.Value as Project;

            _rememberProjectSelection = projectOptions.GenerationOptions.RememberManuallySelectedTargetProjectByDefault;

            ResultingMapping = new ProjectMapping(sourceProject, selectedProject, selectedProject?.Name, new UnitTestGeneratorOptions(generationOptions, namingOptions, strategyOptions, projectOptions.StatisticsCollectionEnabled));

            ApplyTargetProjectFramework();
        }

        public ProjectMapping ResultingMapping { get; }

        public string Title { get; }

        private bool _filterEmpty = true;

        public bool FilterEmpty
        {
            get
            {
                return _filterEmpty;
            }
            set
            {
                if (_filterEmpty != value)
                {
                    _filterEmpty = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FilterEmpty)));
                }
            }
        }

        private string _filterText = string.Empty;

        public string FilterText
        {
            get
            {
                return _filterText;
            }

            set
            {
                if (_filterText != value)
                {
                    _filterText = value;
                    var source = CollectionViewSource.GetDefaultView(Projects);
                    if (source.Filter == null)
                    {
                        source.Filter = Filter;
                    }

                    FilterEmpty = string.IsNullOrWhiteSpace(_filterText);

                    source.Refresh();
                }
            }
        }

        public bool Filter(object obj)
        {
            var model = obj as ObjectItem;
            if (model == null || string.IsNullOrWhiteSpace(_filterText))
            {
                return true;
            }

            return model.Text.IndexOf(_filterText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

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

        private bool _rememberProjectSelection;

        public bool RememberProjectSelection
        {
            get { return _rememberProjectSelection; }
            set
            {
                if (_rememberProjectSelection != value)
                {
                    _rememberProjectSelection = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RememberProjectSelection)));
                }
            }
        }

        public IList<DisplayItem> GenerationOptionsItems { get; }

        public IList<DisplayItem> StrategyOptionsItems { get; }

        public IList<DisplayItem> NamingOptionsItems { get; }

        public List<TabItem> Tabs { get; } = new List<TabItem>();

        public List<ObjectItem> Projects { get; } = new List<ObjectItem>();

        private ObjectItem _selectedProject;

        public ObjectItem SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                if (_selectedProject != value)
                {
                    _selectedProject = value;
                    ResultingMapping.TargetProject = value?.Value as Project;
                    ResultingMapping.TargetProjectName = ResultingMapping.TargetProject?.Name;

                    ApplyTargetProjectFramework();

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedProject)));
                }
            }
        }

        private void ApplyTargetProjectFramework()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (ResultingMapping.TargetProject != null)
            {
                var resolvedOptions = OptionsResolver.DetectFrameworks(ResultingMapping.TargetProject, _generationOptions);
                var testFrameworkItem = GenerationOptionsItems.OfType<EnumEditableItem>().FirstOrDefault(x => string.Equals(x.FieldName, nameof(IGenerationOptions.FrameworkType), System.StringComparison.OrdinalIgnoreCase));
                if (testFrameworkItem != null)
                {
                    testFrameworkItem.SelectedItem = testFrameworkItem.Items.FirstOrDefault(x => x.Value.ToString() == resolvedOptions.FrameworkType.ToString());
                }
                var mockingFrameworkItem = GenerationOptionsItems.OfType<EnumEditableItem>().FirstOrDefault(x => string.Equals(x.FieldName, nameof(IGenerationOptions.MockingFrameworkType), System.StringComparison.OrdinalIgnoreCase));
                if (mockingFrameworkItem != null)
                {
                    mockingFrameworkItem.SelectedItem = mockingFrameworkItem.Items.FirstOrDefault(x => x.Value.ToString() == resolvedOptions.MockingFrameworkType.ToString());
                }
                var fluentAssertionsItem = GenerationOptionsItems.OfType<BooleanEditableItem>().FirstOrDefault(x => string.Equals(x.FieldName, nameof(IGenerationOptions.UseFluentAssertions), System.StringComparison.OrdinalIgnoreCase));
                if (fluentAssertionsItem != null)
                {
                    fluentAssertionsItem.Value = resolvedOptions.UseFluentAssertions;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
