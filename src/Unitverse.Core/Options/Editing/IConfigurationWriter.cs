namespace Unitverse.Core.Options.Editing
{
    using System.Collections.Generic;

    public interface IConfigurationWriter
    {
        void WriteSettings(Dictionary<string, string> settings, string? sourceProjectName, string? targetProjectName);
    }
}
