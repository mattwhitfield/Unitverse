namespace Unitverse.Core.Options.Editing
{
    using System;
    using System.ComponentModel;

    public class TabItem : ViewModelBase
    {
        public TabItem(string text, bool isChecked, TabItemType itemType)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            Text = text;
            _isChecked = isChecked;
            ItemType = itemType;
        }

        private bool _isChecked;

        public string Text { get; }

        public TabItemType ItemType { get; }

        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }

            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    RaisePropertyChanged(nameof(IsChecked));
                }
            }
        }
    }
}
