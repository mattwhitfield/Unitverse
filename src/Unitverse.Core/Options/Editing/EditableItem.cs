namespace Unitverse.Core.Options.Editing
{
    using System.ComponentModel;

    public abstract class EditableItem : INotifyPropertyChanged
    {
        public EditableItem(string text, string description, string fieldName)
        {
            Text = text;
            Description = description;
            FieldName = fieldName;
        }

        public abstract EditableItemType ItemType { get; }

        public string Text { get; }

        public string Description { get; }
        public string FieldName { get; }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
