namespace Unitverse.Core.Options.Editing
{
    using System;

    public class BooleanEditableItem : EditableItem
    {
        public BooleanEditableItem(string text, string description, string fieldName, bool value, Action<bool> setValue, bool showSourceIcon, ConfigurationSource? source)
            : base(text, description, fieldName, showSourceIcon, source)
        {
            _value = value;
            _setValue = setValue ?? throw new ArgumentNullException(nameof(setValue));
        }

        public override EditableItemType ItemType => EditableItemType.Boolean;

        private bool _value;

        public bool Value
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

        private Action<bool> _setValue;
    }
}
