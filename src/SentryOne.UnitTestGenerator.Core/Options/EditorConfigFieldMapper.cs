namespace SentryOne.UnitTestGenerator.Core.Options
{
    using System;
    using System.Collections.Generic;
    using EditorConfig.Core;

    public static class EditorConfigFieldMapper
    {
        public static void ApplyTo<T>(this Dictionary<string, string> fileConfiguration, T target)
        {
            if (fileConfiguration == null)
            {
                throw new ArgumentNullException(nameof(fileConfiguration));
            }

            var mutator = new TypeMemberMutator(typeof(T));
            foreach (var property in fileConfiguration)
            {
                mutator.Set(target, property.Key, property.Value);
            }
        }
    }
}
