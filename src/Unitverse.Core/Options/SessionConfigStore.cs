namespace Unitverse.Core.Options
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class SessionConfigStore
    {
        private static readonly Dictionary<string, string> SettingStore = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public static void StoreSettings<T>(T baseSettings, T modifiedSettings)
            where T : class
        {
            AddModifiedValuesToDictionary(baseSettings, modifiedSettings, SettingStore);
        }

        public static void RestoreSettings<T>(T target)
            where T : class
        {
            SettingStore.ApplyTo(target);
        }

        public static void AddModifiedValuesToDictionary<T>(T baseSettings, T modifiedSettings, Dictionary<string, string> target)
            where T : class
        {
            foreach (var member in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanRead))
            {
                var baseValue = member.GetValue(baseSettings)?.ToString();
                var modifiedValue = member.GetValue(modifiedSettings)?.ToString();

                if (modifiedValue != baseValue && modifiedValue != null)
                {
                    target[member.Name] = modifiedValue;
                }
            }
        }
    }
}
