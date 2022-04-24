using Microsoft.Win32;
using Unitverse.Core.Helpers;

namespace Unitverse.Helper
{
    internal class StatisticsTracker
    {
        const string Key = @"SOFTWARE\Unitverse\Statistics";

        internal static bool Track(IGenerationStatistics statistics)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(Key))
                {
                    IncrementValue(key, "TypesConstructed", statistics.TypesConstructed);
                    IncrementValue(key, "ValuesGenerated", statistics.ValuesGenerated);
                    IncrementValue(key, "InterfacesMocked", statistics.InterfacesMocked);
                    IncrementValue(key, "TestClassesGenerated", statistics.TestClassesGenerated);
                    IncrementValue(key, "TestMethodsGenerated", statistics.TestMethodsGenerated, out var existingTestsGenerated, out var newTestsGenerated);
                    IncrementValue(key, "TestMethodsRegenerated", statistics.TestMethodsRegenerated);

                    return newTestsGenerated / 1000 > existingTestsGenerated / 1000;
                }
            }
            catch
            {
                // don't care about errors recording stats
                return false;
            }
        }

        internal static IGenerationStatistics Get()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(Key))
                {
                    return new GenerationStatistics
                    {
                        TestMethodsRegenerated = GetValue(key, "TestMethodsRegenerated"),
                        TestMethodsGenerated = GetValue(key, "TestMethodsGenerated"),
                        InterfacesMocked = GetValue(key, "InterfacesMocked"),
                        TestClassesGenerated = GetValue(key, "TestClassesGenerated"),
                        TypesConstructed = GetValue(key, "TypesConstructed"),
                        ValuesGenerated = GetValue(key, "ValuesGenerated")
                    };
                }
            }
            catch
            {
                return new GenerationStatistics();
            }
        }

        private static long GetValue(RegistryKey key, string name)
        {
            var existing = key.GetValue(name);
            if (existing is long existingValue)
            {
                return existingValue;
            }
            return 0;
        }

        private static void IncrementValue(RegistryKey key, string name, long value)
        {
            IncrementValue(key, name, value, out _, out _);
        }

        private static void IncrementValue(RegistryKey key, string name, long value, out long existingValue, out long newValue)
        {
            existingValue = GetValue(key, name);
            newValue = existingValue + value;
            key.SetValue(name, newValue, RegistryValueKind.QWord);
        }

        internal static void Reset()
        {
            try
            {
                Registry.CurrentUser.DeleteSubKeyTree(Key);
            }
            catch
            {
                // don't care about errors resetting stats
            }
        }
    }
}