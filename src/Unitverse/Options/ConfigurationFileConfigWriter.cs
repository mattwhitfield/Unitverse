using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Unitverse.Core;
using Unitverse.Core.Options;
using Unitverse.Core.Options.Editing;

namespace Unitverse.Options
{
    public class ConfigurationFileConfigWriter : IConfigurationWriter
    {
        public void WriteSettings(Dictionary<string, string> settings, string sourceProjectName, string targetProjectName)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select a folder in which to save the options";

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    var targetPath = Path.Combine(fbd.SelectedPath, CoreConstants.ConfigFileName);
                    if (File.Exists(targetPath))
                    {
                        if (MessageBox.Show("The file '" + targetPath + "' already exists. Would you like to over-write it?", "File exists", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                        {
                            return;
                        }
                    }

                    ConfigExporter.WriteSettings(targetPath, settings, sourceProjectName, targetProjectName);
                }
            }
        }
    }
}
