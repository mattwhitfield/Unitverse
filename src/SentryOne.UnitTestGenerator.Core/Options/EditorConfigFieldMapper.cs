namespace Unitverse.Core.Options
{
    using System;
    using System.Collections.Generic;

    public static class EditorConfigFieldMapper
    {
        public static void ApplyTo<T>(this Dictionary<string, string> fileConfiguration, T target)
            where T : class
        {
            if (fileConfiguration == null)
            {
                throw new ArgumentNullException(nameof(fileConfiguration));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var mutator = new TypeMemberMutator(typeof(T));
            foreach (var property in fileConfiguration)
            {
                mutator.Set(target, property.Key, property.Value);
            }
        }
    }
}
