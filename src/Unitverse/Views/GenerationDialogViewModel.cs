using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Unitverse.Core.Options;
using Unitverse.Core.Options.Editing;
using Unitverse.Helper;
using Unitverse.Options;

namespace Unitverse.Views
{
    public class GenerationDialogViewModel : INotifyPropertyChanged
    {
        private MutableGenerationOptions _originalGenerationOptions;
        private MutableGenerationOptions _generationOptions;
        private bool _allowDetachedGeneration;
        private readonly IUnitTestGeneratorOptions _projectOptions;

        public GenerationDialogViewModel(Project sourceProject, IUnitTestGeneratorOptions projectOptions, string resolvedTargetProjectName)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Title = "Unitverse options for source project '" + sourceProject.Name + "'";

            Tabs.Add(new TabItem("Target Project", true, TabItemType.TargetProject));
            Tabs.Add(new TabItem("Generation Options", false, TabItemType.GenerationOptions));
            Tabs.Add(new TabItem("Strategy Options", false, TabItemType.StrategyOptions));
            Tabs.Add(new TabItem("Naming Options", false, TabItemType.NamingOptions));

            _selectedTab = Tabs.First();
            _projectOptions = projectOptions;

            _generationOptions = new MutableGenerationOptions(projectOptions.GenerationOptions);
            var strategyOptions = new MutableStrategyOptions(projectOptions.StrategyOptions);
            var namingOptions = new MutableNamingOptions(projectOptions.NamingOptions);

            _originalGenerationOptions = new MutableGenerationOptions(_generationOptions);
            _allowDetachedGeneration = _originalGenerationOptions.AllowGenerationWithoutTargetProject;

            GenerationOptionsItems = EditableItemExtractor.ExtractFrom(new GenerationOptions(), _generationOptions, true, projectOptions.GetFieldSource).ToList();
            StrategyOptionsItems = EditableItemExtractor.ExtractFrom(new StrategyOptions(), strategyOptions, true, projectOptions.GetFieldSource).ToList();
            NamingOptionsItems = EditableItemExtractor.ExtractFrom(new NamingOptions(), namingOptions, true, projectOptions.GetFieldSource).ToList();

            SaveOptionItems = new[]
            {
                new ObjectItem("This generation only", SaveOption.ThisGeneration),
                new ObjectItem("This session", SaveOption.ThisSession),
                new ObjectItem("Configuration file", SaveOption.ConfigurationFile),
                new ObjectItem("Visual studio settings", SaveOption.VisualStudioConfiguration),
            };
            _selectedSaveOption = SaveOptionItems.First();

            Projects.Add(new ObjectItem("Generate detached test class(es)", null));
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
            foreach (var project in VsProjectHelper.FindProjects(sourceProject.DTE.Solution).Where(x => x.UniqueName != sourceProject.UniqueName).OrderBy(x => x.Name))
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
            {
                Projects.Add(new ObjectItem(project.Name, project));
            }

            _selectedProject = Projects.FirstOrDefault(x => string.Equals(x.Text, resolvedTargetProjectName, StringComparison.OrdinalIgnoreCase));
            var selectedProject = _selectedProject?.Value as Project;

            ResultingMapping = new ProjectMapping(sourceProject, selectedProject, selectedProject?.Name, new UnitTestGeneratorOptions(_generationOptions, namingOptions, strategyOptions, projectOptions.StatisticsCollectionEnabled, null, null, sourceProject.DTE?.Solution?.FileName ?? string.Empty, sourceProject.SafeFileName()));

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

        public IList<ObjectItem> SaveOptionItems { get; }

        public ObjectItem SelectedSaveOption
        {
            get { return _selectedSaveOption; }
            set
            {
                if (_selectedSaveOption != value)
                {
                    _selectedSaveOption = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSaveOption)));
                }
            }
        }

        public IList<DisplayItem> GenerationOptionsItems { get; }

        public IList<DisplayItem> StrategyOptionsItems { get; }

        public IList<DisplayItem> NamingOptionsItems { get; }

        public List<TabItem> Tabs { get; } = new List<TabItem>();

        public List<ObjectItem> Projects { get; } = new List<ObjectItem>();

        private ObjectItem _selectedProject;
        private ObjectItem _selectedSaveOption;

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

                    _generationOptions.AllowGenerationWithoutTargetProject = (ResultingMapping.TargetProject == null && _selectedProject != null) ? true : _allowDetachedGeneration;

                    ApplyTargetProjectFramework();

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedProject)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanRememberSelectedProject)));
                }
            }
        }

        public bool CanRememberSelectedProject => _selectedProject == null || _selectedProject.Value != null;

        private void ApplyTargetProjectFramework()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IGenerationOptions resolvedOptions;
            if (ResultingMapping.TargetProject != null)
            {
                resolvedOptions = OptionsResolver.DetectFrameworks(ResultingMapping.TargetProject, _originalGenerationOptions);
            }
            else
            {
                resolvedOptions = _originalGenerationOptions;
            }

            ApplyEnumOption(nameof(IGenerationOptions.FrameworkType), o => o.FrameworkType, _projectOptions, resolvedOptions);
            ApplyEnumOption(nameof(IGenerationOptions.MockingFrameworkType), o => o.MockingFrameworkType, _projectOptions, resolvedOptions);

            ApplyBoolOption(nameof(IGenerationOptions.UseFluentAssertions), o => o.UseFluentAssertions, _projectOptions, resolvedOptions);
            ApplyBoolOption(nameof(IGenerationOptions.UseShouldly), o => o.UseShouldly, _projectOptions, resolvedOptions);
            ApplyBoolOption(nameof(IGenerationOptions.UseAutoFixture), o => o.UseAutoFixture, _projectOptions, resolvedOptions);
            ApplyBoolOption(nameof(IGenerationOptions.UseAutoFixtureForMocking), o => o.UseAutoFixtureForMocking, _projectOptions, resolvedOptions);
        }

        private void ApplyEnumOption(string fieldName, Func<IGenerationOptions, object> getValue, IUnitTestGeneratorOptions options, IGenerationOptions resolvedOptions)
        {
            ApplyOption<EnumEditableItem>(fieldName, getValue, options, resolvedOptions, (item, resolvedValue) =>
            {
                item.SelectedItem = item.Items.FirstOrDefault(x => x.Value.ToString() == resolvedValue.ToString());
            });
        }

        private void ApplyBoolOption(string fieldName, Func<IGenerationOptions, object> getValue, IUnitTestGeneratorOptions options, IGenerationOptions resolvedOptions)
        {
            ApplyOption<BooleanEditableItem>(fieldName, getValue, options, resolvedOptions, (item, resolvedValue) =>
            {
                if (resolvedValue is bool b)
                {
                    item.Value = b;
                }
            });
        }

        private void ApplyOption<T>(string fieldName, Func<IGenerationOptions, object> getValue, IUnitTestGeneratorOptions options, IGenerationOptions resolvedOptions, Action<T, object> configure)
           where T : EditableItem
        {
            var item = GenerationOptionsItems.OfType<T>().FirstOrDefault(x => string.Equals(x.FieldName, fieldName, StringComparison.OrdinalIgnoreCase));
            if (item != null)
            {
                var resolvedValue = getValue(resolvedOptions);

                configure(item, resolvedValue);

                if (resolvedValue.ToString() != getValue(options.GenerationOptions).ToString())
                {
                    item.SetSourceState(new ConfigurationSource(ConfigurationSourceType.AutoDetection));
                }
                else
                {
                    item.SetSourceState(options.GetFieldSource(fieldName));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
