namespace Unitverse.Core.Options
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    public delegate bool TypeMemberSetter(object instance, string fieldValue);

    public static class TypeMemberMutator
    {
        public static bool Set(object instance, PropertyInfo member, string fieldValue)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (member is null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            if (string.IsNullOrWhiteSpace(fieldValue))
            {
                return false;
            }

            try
            {
                member.SetValue(instance, Coerce(fieldValue, member.PropertyType));
                return true;
            }
            catch (Exception e) when (e is TargetException || e is MethodAccessException || e is TargetInvocationException || e is InvalidCastException)
            {
            }

            return false;
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