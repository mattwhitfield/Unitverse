namespace Unitverse.Core.Options
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class SessionConfigStore
    {
        private static readonly Dictionary<string, string> SettingStore = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, string> TargetProjects = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public static void StoreSettings(Dictionary<string, string> settings)
        {
            foreach (var pair in settings)
            {
                SettingStore[pair.Key] = pair.Value;
            }
        }

        public static void RestoreSettings<T>(T target, Action<string> onMemberSet)
            where T : class
        {
            SettingStore.ApplyTo(target, onMemberSet);
        }

        public static Dictionary<string, string> ProjectMappings => TargetProjects;

        public static void SetTargetFor(string sourceProjectName, string targetProjectName)
        {
            TargetProjects[sourceProjectName] = targetProjectName;
        }

        public static void AddModifiedValuesToDictionary<T>(T modifiedSettings, T baseSettings, Dictionary<string, string> target)
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
