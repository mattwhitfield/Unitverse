using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Unitverse.Core.Helpers;
using Unitverse.Core.Models;
using Unitverse.Core.Options;
using Unitverse.Core.Options.Editing;
using Unitverse.Options;

namespace Unitverse.Views
{
    public class NewProjectDialogViewModel : INotifyPropertyChanged
    {
        private readonly MutableGenerationOptions _generationOptions;

        public NewProjectDialogViewModel(Project sourceProject, IUnitTestGeneratorOptions projectOptions)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _generationOptions = new MutableGenerationOptions(projectOptions.GenerationOptions);
            var strategyOptions = new MutableStrategyOptions(projectOptions.StrategyOptions);
            var namingOptions = new MutableNamingOptions(projectOptions.NamingOptions);

            _name = projectOptions.GenerationOptions.GetTargetProjectNames(sourceProject.Name).FirstOrDefault();
            _location = new FileInfo(sourceProject.FileName).Directory.Parent.FullName;

            bool IsValidProperty(string propertyName)
            {
                return string.Equals(propertyName, nameof(IGenerationOptions.FrameworkType), StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(propertyName, nameof(IGenerationOptions.MockingFrameworkType), StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(propertyName, nameof(IGenerationOptions.UseFluentAssertions), StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(propertyName, nameof(IGenerationOptions.UseAutoFixture), StringComparison.OrdinalIgnoreCase);
            }

            GenerationOptionsItems = EditableItemExtractor.ExtractFrom(new GenerationOptions(), _generationOptions, true, projectOptions.GetFieldSourceFileName, IsValidProperty).ToList();
        }

        internal void CreateManifest()
        {
            throw new NotImplementedException();
        }

        public ProjectCreationManifest Manifest => new ProjectCreationManifest(_name, _location, _generationOptions);

        public IList<DisplayItem> GenerationOptionsItems { get; }


        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }
        }

        private string _location;

        public string Location
        {
            get { return _location; }
            set
            {
                if (_location != value)
                {
                    _location = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Location)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
