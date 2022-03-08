namespace Unitverse.Core.Options.Editing
{
    using System.ComponentModel;

    public class TabItem : INotifyPropertyChanged
    {
        public TabItem(string text, bool isChecked, TabItemType itemType)
        {
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
