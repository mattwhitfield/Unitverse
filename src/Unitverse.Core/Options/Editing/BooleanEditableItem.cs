namespace Unitverse.Core.Options.Editing
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class BooleanEditableItem : EditableItem
    {
        public BooleanEditableItem(string text, string description, string fieldName, bool value, Action<bool> setValue)
            : base(text + ":", description, fieldName)
        {
            _value = value;
            _setValue = setValue;
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
