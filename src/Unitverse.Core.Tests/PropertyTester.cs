namespace Unitverse.Core.Tests
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;
    using NUnit.Framework;

    public static class PropertyTester
    {
        public static void CheckProperty<TContainer>(this TContainer propertyContainer, Expression<Func<TContainer, string>> property)
            where TContainer : INotifyPropertyChanged
        {
            CheckProperty(propertyContainer, property, "stringValue1", "stringValue2");
        }

        public static void CheckProperty<TContainer>(this TContainer propertyContainer, Expression<Func<TContainer, bool>> property)
            where TContainer : INotifyPropertyChanged
        {
            CheckProperty(propertyContainer, property, true, false);
        }

        public static void CheckProperty<TContainer>(this TContainer propertyContainer, Expression<Func<TContainer, Guid>> property)
            where TContainer : INotifyPropertyChanged
        {
            CheckProperty(propertyContainer, property, new Guid("87B1902E-6CE3-4465-920F-2314F4196534"), new Guid("CC791033-8A6A-47A7-BC3F-ECF397D0977E"));
        }

        public static void CheckProperty<TContainer>(this TContainer propertyContainer, Expression<Func<TContainer, int>> property)
            where TContainer : INotifyPropertyChanged
        {
            CheckProperty(propertyContainer, property, 64354, 234624476);
        }

        public static void CheckProperty<TContainer>(this TContainer propertyContainer, Expression<Func<TContainer, short>> property)
            where TContainer : INotifyPropertyChanged
        {
            CheckProperty(propertyContainer, property, (short)1234, (short)3526);
        }

        public static void CheckProperty<TContainer>(this TContainer propertyContainer, Expression<Func<TContainer, byte>> property)
            where TContainer : INotifyPropertyChanged
        {
            CheckProperty(propertyContainer, property, (byte)12, (byte)53);
        }

        public static void CheckProperty<TContainer>(this TContainer propertyContainer, Expression<Func<TContainer, decimal>> property)
            where TContainer : INotifyPropertyChanged
        {
            CheckProperty(propertyContainer, property, 12.13m, 53.53m);
        }

        public static void CheckProperty<TContainer>(this TContainer propertyContainer, Expression<Func<TContainer, long>> property)
            where TContainer : INotifyPropertyChanged
        {
            CheckProperty(propertyContainer, property, 7544563756573356L, 343765427624562L);
        }

        public static void CheckProperty<TContainer>(this TContainer propertyContainer, Expression<Func<TContainer, DateTime>> property)
            where TContainer : INotifyPropertyChanged
        {
            CheckProperty(propertyContainer, property, new DateTime(2001, 1, 1), new DateTime(2001, 1, 2));
        }

        public static void CheckProperty<TContainer, TProperty>(this TContainer propertyContainer, Expression<Func<TContainer, TProperty>> property, TProperty value1, TProperty value2)
            where TContainer : INotifyPropertyChanged
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            // get the property
            if (!(property.Body is MemberExpression memberExpression))
            {
                throw new ArgumentException("The lambda expression 'property' should point to a valid property");
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("The lambda expression 'property' should point to a valid property");
            }

            // check we found the property and that it's the right type
            Assert.That(propertyInfo.PropertyType, Is.EqualTo(typeof(TProperty)));

            var getMethod = propertyInfo.GetGetMethod();
            var setMethod = propertyInfo.GetSetMethod();

            if (getMethod == null || setMethod == null)
            {
                throw new ArgumentException("The lambda expression 'property' should point to a valid property that contains a getter and setter");
            }

            // check can get and set
            Assert.DoesNotThrow(() => getMethod.Invoke(propertyContainer, new object[] { }));
            Assert.DoesNotThrow(() => setMethod.Invoke(propertyContainer, new object[] { value1 }));

            // check we get property changed event for the correct property when setting to a different value
            var propertyChanged = false;
            propertyContainer.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == propertyInfo.Name)
                {
                    propertyChanged = true;
                }
            };

            Assert.That(getMethod.Invoke(propertyContainer, new object[] { }), Is.EqualTo(value1));

            setMethod.Invoke(propertyContainer, new object[] { value2 });
            Assert.True(propertyChanged);

            Assert.That(getMethod.Invoke(propertyContainer, new object[] { }), Is.EqualTo(value2));

            // check we don't get property changed when setting to the same value
            propertyChanged = false;
            setMethod.Invoke(propertyContainer, new object[] { value2 });
            Assert.False(propertyChanged);
        }
    }
}
