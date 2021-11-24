namespace Unitverse.Core.Options
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    public class TypeMemberMutator
    {
        private readonly Type _targetType;

        public TypeMemberMutator(Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            _targetType = targetType;
        }

        public void Set(object instance, string fieldName, string fieldValue)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (string.IsNullOrWhiteSpace(fieldName) || string.IsNullOrWhiteSpace(fieldValue))
            {
                return;
            }

            var cleanFieldName = fieldName.Replace("_", string.Empty).Replace("-", string.Empty);
            foreach (var member in _targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanWrite))
            {
                if (string.Equals(member.Name, cleanFieldName, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        member.SetValue(instance, Coerce(fieldValue, member.PropertyType));
                    }
                    catch (Exception e) when (e is TargetException || e is MethodAccessException || e is TargetInvocationException || e is InvalidCastException)
                    {
                    }

                    break;
                }
            }
        }

        private static object Coerce(object convertibleValue, Type targetType)
        {
            try
            {
                try
                {
                    return Convert.ChangeType(convertibleValue, targetType, CultureInfo.InvariantCulture);
                }
                catch (Exception e) when (e is InvalidCastException || e is FormatException || e is OverflowException)
                {
                }

                var descriptor = TypeDescriptor.GetConverter(targetType);
                return descriptor.ConvertFromString(convertibleValue.ToString());
            }
            catch
            {
                throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, "Could not interpret the value '{0}' as type '{1}'.", convertibleValue, targetType.FullName));
            }
        }
    }
}