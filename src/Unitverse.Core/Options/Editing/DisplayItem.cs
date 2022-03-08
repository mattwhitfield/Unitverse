namespace Unitverse.Core.Options.Editing
{
    using System;
    using System.ComponentModel;

    public abstract class DisplayItem : INotifyPropertyChanged
    {
        public DisplayItem(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            Text = text;
        }

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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
