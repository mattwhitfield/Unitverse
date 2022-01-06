using Microsoft.Win32;
using Unitverse.Core.Helpers;

namespace Unitverse.Helper
{
    internal class StatisticsTracker
    {
        const string Key = @"SOFTWARE\Unitverse\Statistics";

        internal static void Track(IGenerationStatistics statistics)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(Key);

                IncrementValue(key, "TypesConstructed", statistics.TypesConstructed);
                IncrementValue(key, "ValuesGenerated", statistics.ValuesGenerated);
                IncrementValue(key, "InterfacesMocked", statistics.InterfacesMocked);
                IncrementValue(key, "TestClassesGenerated", statistics.TestClassesGenerated);
                IncrementValue(key, "TestMethodsGenerated", statistics.TestMethodsGenerated);
                IncrementValue(key, "TestMethodsRegenerated", statistics.TestMethodsRegenerated);
            }
            catch
            {
                // don't care about errors recording stats
            }
        }

        internal static IGenerationStatistics Get()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(Key);
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
            var existingValue = GetValue(key, name);
            key.SetValue(name, existingValue + value, RegistryValueKind.QWord);
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