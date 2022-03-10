using Microsoft.Win32;

namespace Unitverse.Helper
{
    internal class ZoomTracker
    {
        const string Key = @"SOFTWARE\Unitverse\UI";
        const string Value = "ZoomFactor";
        internal static void Save(int zoom)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(Key))
                {
                    key.SetValue(Value, zoom, RegistryValueKind.DWord);
                }
            }
            catch
            {
                // don't care about errors recording zoom
            }
        }

        internal static int Get()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(Key))
                {
                    var existing = key.GetValue(Value);
                    if (existing is int existingValue)
                    {
                        return existingValue;
                    }
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }
    }
}