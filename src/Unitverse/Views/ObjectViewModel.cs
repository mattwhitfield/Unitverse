using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using Unitverse.Core.Options.Editing;
using Unitverse.Core.Templating.Model;

namespace Unitverse.Views
{
    public class ObjectViewModel : ViewModelBase
    {
        public ObjectViewModel(string name)
        {
            Name = name;
        }

        public ObjectViewModel(object target, string name = null)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Name = name;
            }
            else if (target is INameProvider np)
            {
                Name = np.Name;
            }
            else
            {
                Name = target.GetType().Name;
            }
                
            foreach (var property in target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)) 
            {
                var value = property.GetValue(target, null);

                if (!property.PropertyType.IsValueType &&
                    !typeof(string).IsAssignableFrom(property.PropertyType))
                {
                    var enumerableValue = value as IEnumerable;
                    if (enumerableValue != null)
                    {
                        var container = new ObjectViewModel(property.Name);
                        foreach (var child in enumerableValue)
                        {
                            container.Children.Add(new ObjectViewModel(child));
                        }
                        Children.Add(container);
                    }
                    else
                    {
                        Children.Add(new ObjectViewModel(value, property.Name));
                    }
                }
                else
                {
                    Properties.Add(new PropertyViewModel(property.Name, value));
                }
            }
        }

        public string Name { get; }

        public ObservableCollection<ObjectViewModel> Children { get; } = new ObservableCollection<ObjectViewModel>();

        public ObservableCollection<PropertyViewModel> Properties { get; } = new ObservableCollection<PropertyViewModel>();
    }

    public class PropertyViewModel : ViewModelBase
    {
        public PropertyViewModel(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public object Value { get; }
    }
}
