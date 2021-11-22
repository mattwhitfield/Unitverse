namespace SentryOne.UnitTestGenerator.Core.Helpers
{
    using System;
    using System.Collections.Generic;

    public static class EnumerableExtensions
    {
        public static void Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            foreach (var item in source)
            {
                action(item);
            }
        }
    }
}
