namespace Unitverse.Core.Options.Editing
{
    using System;

    public class StringEditableItem : EditableItem
    {
        public StringEditableItem(string text, string description, string fieldName, string value, Action<string> setValue, bool showSourceIcon, ConfigurationSource? source)
            : base(text, description, fieldName, showSourceIcon, source)
        {
            _value = value;
            _setValue = setValue ?? throw new ArgumentNullException(nameof(setValue));
        }

        public override EditableItemType ItemType => EditableItemType.String;

        private string _value;

        public string Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (value != _value)
                {
                    _value = value;
                    _setValue(value);
                    RaisePropertyChanged(nameof(Value));
                }
            }
        }

        private Action<string> _setValue;
    }
}
