namespace Unitverse.Core.Options.Editing
{
    using System;
    using System.ComponentModel;

    public abstract class DisplayItem : INotifyPropertyChanged
    {
        protected DisplayItem(string text, bool showSourceIcon, string? sourceFileName)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            Text = text.Replace("&&", "&");
            SourceFileName = sourceFileName;
            if (showSourceIcon)
            {
                ShowVsConfigSource = string.IsNullOrWhiteSpace(sourceFileName);
                ShowFileConfigSource = !ShowVsConfigSource;
            }
        }

        public void SetSourceState(bool showAutoDetectionSource, bool showVsConfigSource)
        {
            if (ShowVsConfigSource || ShowFileConfigSource || ShowAutoDetectionSource)
            {
                ShowAutoDetectionSource = showAutoDetectionSource;
                ShowVsConfigSource = showVsConfigSource && !showAutoDetectionSource;
                ShowFileConfigSource = !showVsConfigSource && !showAutoDetectionSource;
                RaisePropertyChanged(nameof(ShowAutoDetectionSource));
                RaisePropertyChanged(nameof(ShowVsConfigSource));
                RaisePropertyChanged(nameof(ShowFileConfigSource));
            }
        }

        public bool ShowVsConfigSource { get; private set; }

        public bool ShowFileConfigSource { get; private set; }

        public bool ShowAutoDetectionSource { get; private set; }

        public string? SourceFileName { get; }

        public abstract EditableItemType ItemType { get; }

        public string Text { get; }

        protected void RaisePropertyChanged(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
