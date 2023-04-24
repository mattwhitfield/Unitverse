namespace Unitverse.Core.Options.Editing
{
    using System;
    using System.ComponentModel;

    public abstract class DisplayItem : ViewModelBase
    {
        private readonly bool _showSourceIcon;

        protected DisplayItem(string text, bool showSourceIcon, ConfigurationSource? configurationSource)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            Text = text.Replace("&&", "&");
            SourceFileName = configurationSource?.FileName ?? string.Empty;
            _showSourceIcon = showSourceIcon;
            SetSourceState(configurationSource);
        }

        public void SetSourceState(ConfigurationSource? source)
        {
            if (_showSourceIcon)
            {
                ShowVsConfigSource = source?.SourceType == ConfigurationSourceType.VisualStudio;
                ShowSessionConfigSource = source?.SourceType == ConfigurationSourceType.Session;
                ShowFileConfigSource = source?.SourceType == ConfigurationSourceType.ConfigurationFile;
                ShowAutoDetectionSource = source?.SourceType == ConfigurationSourceType.AutoDetection;

                RaisePropertyChanged(nameof(ShowVsConfigSource));
                RaisePropertyChanged(nameof(ShowSessionConfigSource));
                RaisePropertyChanged(nameof(ShowFileConfigSource));
                RaisePropertyChanged(nameof(ShowAutoDetectionSource));
            }
        }

        public bool ShowVsConfigSource { get; private set; }

        public bool ShowSessionConfigSource { get; private set; }

        public bool ShowFileConfigSource { get; private set; }

        public bool ShowAutoDetectionSource { get; private set; }

        public string? SourceFileName { get; }

        public abstract EditableItemType ItemType { get; }

        public string Text { get; }
    }
}
