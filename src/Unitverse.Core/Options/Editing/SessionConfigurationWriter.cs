namespace Unitverse.Core.Options.Editing
{
    using System.Collections.Generic;

    public class SessionConfigurationWriter : IConfigurationWriter
    {
        public void WriteSettings(Dictionary<string, string> settings, string? sourceProjectName, string? targetProjectName)
        {
            SessionConfigStore.StoreSettings(settings);

            if (!string.IsNullOrWhiteSpace(sourceProjectName) && !string.IsNullOrWhiteSpace(targetProjectName))
            {
#pragma warning disable CS8604 // Possible null reference argument - nope
                SessionConfigStore.SetTargetFor(sourceProjectName, targetProjectName);
#pragma warning restore CS8604 // Possible null reference argument.
            }
        }
    }
}
