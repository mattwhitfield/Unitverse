namespace Unitverse.Core.Options.Editing
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class EnumEditableItem : EditableItem
    {
        public EnumEditableItem(string text, string description, string fieldName, object value, Action<object> setValue, Type enumerationType, bool showSourceIcon, ConfigurationSource? source)
            : base(text, description, fieldName, showSourceIcon, source)
        {
            var selectedValueName = value.ToString();

            foreach (var enumValue in Enum.GetValues(enumerationType))
            {
                var enumValueName = enumValue.ToString();
                var field = enumerationType.GetField(enumValueName);
                var enumValueText = Attribute.IsDefined(field, typeof(DescriptionAttribute)) ? ((DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))).Description : enumValueName;

                var item = new ObjectItem(enumValueText, enumValue);
                Items.Add(item);
                if (selectedValueName == enumValueName)
                {
                    _selectedItem = item;
                }
            }

            _setValue = setValue ?? throw new ArgumentNullException(nameof(setValue));
        }

        public override EditableItemType ItemType => EditableItemType.Enum;

        public List<ObjectItem> Items { get; } = new List<ObjectItem>();

        private ObjectItem? _selectedItem;

        public ObjectItem? SelectedItem
        {
            get
            {
                return _selectedItem;
            }

            set
            {
                if (value != _selectedItem)
                {
                    _selectedItem = value;
                    if (value != null)
                    {
                        _setValue(value.Value);
                    }

                    RaisePropertyChanged(nameof(SelectedItem));
                }
            }
        }

        private Action<object> _setValue;
    }
}
