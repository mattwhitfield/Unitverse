namespace Unitverse.Core.Options
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class EditorConfigFieldMapper
    {
        private static readonly Dictionary<Type, Dictionary<string, TypeMemberSetter>> Cache = new Dictionary<Type, Dictionary<string, TypeMemberSetter>>();

        public static Dictionary<string, TypeMemberSetter> CreateMutatorSet<T>()
        {
            if (Cache.TryGetValue(typeof(T), out var mutatorSet))
            {
                return mutatorSet;
            }

            mutatorSet = new Dictionary<string, TypeMemberSetter>(StringComparer.OrdinalIgnoreCase);

            foreach (var member in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanWrite))
            {
                TypeMemberSetter setter = (instance, value) => TypeMemberMutator.Set(instance, member, value);
                mutatorSet[member.Name] = setter;
            }

            Cache[typeof(T)] = mutatorSet;
            return mutatorSet;
        }

        public static bool ApplyTo<T>(this Dictionary<string, string> values, T instance, Action<string>? onMemberSet = null)
            where T : class
        {
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var anySet = false;

            var mutatorSet = CreateMutatorSet<T>();
            foreach (var valuePair in values)
            {
                var cleanFieldName = valuePair.Key.Replace("_", string.Empty).Replace("-", string.Empty);
                if (mutatorSet.TryGetValue(cleanFieldName, out var mutator))
                {
                    mutator(instance, valuePair.Value);
                    anySet = true;

                    onMemberSet?.Invoke(cleanFieldName);
                }
            }

            return anySet;
        }
    }
}
