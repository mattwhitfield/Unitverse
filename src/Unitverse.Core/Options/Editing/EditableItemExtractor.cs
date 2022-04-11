namespace Unitverse.Core.Options.Editing
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    public static class EditableItemExtractor
    {
        public static IEnumerable<DisplayItem> ExtractFrom(object source, object modifiableInstance, bool skipExcluded, Func<string, string> sourceFileExtractor = null)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (modifiableInstance is null)
            {
                throw new ArgumentNullException(nameof(modifiableInstance));
            }

            var type = source.GetType();
            var categories = new Dictionary<string, List<EditableItem>>();
            var modifiableProperties = modifiableInstance.GetType().GetProperties().Where(x => x.CanWrite).ToDictionary(x => x.Name);
            foreach (var property in type.GetProperties().Where(x => x.CanWrite))
            {
                var propertyType = property.PropertyType;

                // skip if it's not in the modifiable instance, the type is not the same or it has [ExcludedFromUserInterface]
                if (!modifiableProperties.TryGetValue(property.Name, out var modifiableProperty) ||
                    propertyType != modifiableProperty.PropertyType ||
                    (skipExcluded && GetAttribute<ExcludedFromUserInterfaceAttribute>(property) != null))
                {
                    continue;
                }

                var category = GetAttribute<CategoryAttribute>(property)?.Category ?? null;

                if (!string.IsNullOrWhiteSpace(category))
                {
                    if (!categories.TryGetValue(category, out var list))
                    {
                        categories[category] = list = new List<EditableItem>();
                    }

                    var text = GetAttribute<DisplayNameAttribute>(property)?.DisplayName ?? property.Name;
                    var description = GetAttribute<DescriptionAttribute>(property)?.Description ?? property.Name;

                    var shouldShowSource = sourceFileExtractor != null;
                    var sourceFile = sourceFileExtractor?.Invoke(property.Name);

                    if (propertyType == typeof(string))
                    {
                        var value = (string)modifiableProperty.GetValue(modifiableInstance, null);
                        Action<string> setValue = s => modifiableProperty.SetValue(modifiableInstance, s, null);
                        list.Add(new StringEditableItem(text, description, modifiableProperty.Name, value, setValue, shouldShowSource, sourceFile));
                    }
                    else if (propertyType == typeof(bool))
                    {
                        var value = (bool)modifiableProperty.GetValue(modifiableInstance, null);
                        Action<bool> setValue = b => modifiableProperty.SetValue(modifiableInstance, b, null);
                        list.Add(new BooleanEditableItem(text, description, modifiableProperty.Name, value, setValue, shouldShowSource, sourceFile));
                    }
                    else if (propertyType.IsEnum)
                    {
                        var value = modifiableProperty.GetValue(modifiableInstance, null);
                        Action<object> setValue = o => modifiableProperty.SetValue(modifiableInstance, o, null);
                        list.Add(new EnumEditableItem(text, description, modifiableProperty.Name, value, setValue, propertyType, shouldShowSource, sourceFile));
                    }
                    else
                    {
                        throw new NotSupportedException("Property type " + propertyType.Name + " is not supported");
                    }
                }
            }

            foreach (var category in categories.OrderBy(x => x.Key))
            {
                yield return new HeaderDisplayItem(category.Key);

                foreach (var item in category.Value.OrderBy(x => x.Text))
                {
                    yield return item;
                }
            }
        }

        private static T GetAttribute<T>(MemberInfo member)
            where T : class
        {
            return Attribute.IsDefined(member, typeof(T)) ? (Attribute.GetCustomAttribute(member, typeof(T)) as T) : default;
        }
    }
}
